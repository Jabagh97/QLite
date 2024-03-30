﻿
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using QLiteDataApi.Constants;
using QLite.Data;
using QLiteDataApi.Helpers;
using QLiteDataApi.Context;

namespace QLiteDataApi.QueryFactory
{
    /// <summary>
    /// Provides services for dynamically constructing and executing queries based on model and view model types,
    /// and performing CRUD operations on model instances within an ApplicationDbContext.
    /// </summary>

    public class DynamicQueriesService 
    {
        /// <summary>
        /// Dynamically constructs and executes a query based on specified model and view model types, 
        /// incorporating optional arguments for further filtering and uses Navigation Properties as left joins.
        /// </summary>
        /// <param name="data">The initial IQueryable data source.</param>
        /// <param name="modelType">The type of the model being queried.</param>
        /// <param name="viewModelType">The type of the view model that determines which properties to select.</param>
        /// <param name="_dbContext">The database context used for querying.</param>
        /// <param name="optionalArguments">Optional arguments for additional filtering.(for IColletions of the DbSet)</param>
        /// <returns>An IQueryable representing the dynamically constructed query.</returns>

        public IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, ApplicationDbContext _dbContext, Dictionary<string, object>? optionalArguments = null)
        {
            if (data == null)
            {
                return Enumerable.Empty<object>().AsQueryable();
            }

            IQueryable query = data;

            // Handle unique cases for specific model types (that does not have GcRecord)
            QueriesHelper.HandleUniqueCases(modelType, ref query);

            var modelProperties = modelType.GetProperties();
            var viewModelProperties = viewModelType?.GetProperties()?.Where(p => p.PropertyType != typeof(ICollection<object>));

            //Select only the properties we want to view from model 
            var selectProperties = modelProperties
                .Where(mp => viewModelProperties?.Any(vp => vp.Name == mp.Name || mp.Name == Properties.Oid) == true);

            //Left Join the Navigation properties (Entity will translate them to left joins)
            var leftJoinProperties = modelProperties
                .Where(mp => viewModelProperties?.Any(vp => mp.Name.Contains(vp.Name) && QueriesHelper.IsNavigationProperty(mp)) == true);


            // Select and combining Navigations but as the actual property from viewModel
            var combinedProperties = selectProperties
                .Select(sp => leftJoinProperties.FirstOrDefault(lp =>
                    lp.Name.Contains(sp.Name) && QueriesHelper.RemoveNavigationKeyword(lp.Name).Length == sp.Name.Length) ?? sp);


            var selectExpressions = combinedProperties.Select(property =>
            {
                var propertyToJoin = property.Name switch
                {
                    Properties.Kiosk => Properties.KappName,
                    _ => Properties.Name
                };
                var navigationProperty = property.Name.Contains(Properties.Navigation)
                    ? property.Name.Replace(Properties.Navigation, "")
                    : null;

                return !string.IsNullOrEmpty(navigationProperty)
                    ? $"{property.Name} != null ? {property.Name}.{propertyToJoin} : null as {navigationProperty}"
                    : $"{property.Name} as {property.Name}";
            });

            // optional arguments to WHERE clause (Extension)
            if (optionalArguments != null && optionalArguments.Any())
            {
                foreach (var condition in optionalArguments)
                {
                    query = query.Where($"{condition.Key} == @0", condition.Value);
                }
            }

            var selectProjection = string.Join(", ", selectExpressions);

            // Select and Left joins final result
            return query.Select($"new ({selectProjection})");
        }

        /// <summary>
        /// Creates an instance of a model with standard properties set and adds it to the specified DbContext.
        /// </summary>
        /// <param name="user">The user performing the operation.</param>
        /// <param name="_dbContext">The DbContext to which the model instance is added.</param>
        /// <param name="modelInstance">The model instance to add.</param>

        public void CreateInstance(string user, ApplicationDbContext _dbContext, object modelInstance)
        {
            // Ensure the modelInstance is not null
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance), Errors.NullModel);
            }

            modelInstance = QueriesHelper.SetStandardProperties(modelInstance, user, Operations.Create);


            // Add the model instance to the DbContext
            _dbContext.Add(modelInstance);

            // Save changes to the database
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Retrieves a model instance by its identifier.
        /// </summary>
        /// <param name="entityId">The identifier of the entity to retrieve.</param>
        /// <param name="dbSet">The IQueryable representing the DbSet to query.</param>
        /// <returns>The model instance if found; otherwise, null.</returns>

        public object GetById(string entityId, IQueryable dbSet)
        {

            // Find the entity by its ID
            var entity = dbSet.Where($"{Properties.Oid} == \"{entityId}\"").FirstOrDefault();

            return entity;
        }


        /// <summary>
        /// Updates an existing model instance within the specified DbContext.
        /// </summary>
        /// <param name="user">The user performing the update operation.</param>
        /// <param name="_dbContext">The DbContext where the model instance exists.</param>
        /// <param name="existingEntity">The existing model instance to update.</param>

        public void UpdateInstance(string user, ApplicationDbContext _dbContext, object existingEntity)
        {
            if (existingEntity == null)
            {
                throw new ArgumentNullException(nameof(existingEntity), Errors.NullEntity);
            }


            existingEntity = QueriesHelper.SetStandardProperties(existingEntity, user);

            _dbContext.Attach(existingEntity);


            // Set the entity state to Modified
            _dbContext.Entry(existingEntity).State = EntityState.Modified;

            // Save changes to the database
            _dbContext.SaveChanges();
        }


        /// <summary>
        /// Soft deletes a model instance by setting a 'GcRecord' property and saves changes to the DbContext.
        /// </summary>
        /// <param name="dbContext">The DbContext containing the entity to soft delete.</param>
        /// <param name="data">The IQueryable representing the DbSet to query for the entity.</param>
        /// <param name="modelType">The type of the model being soft deleted.</param>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        /// <returns>True if the soft delete operation was successful; otherwise, false.</returns>

        public bool SoftDeleteInstance(ApplicationDbContext dbContext, IQueryable data, Type modelType, string id)
        {
            try
            {
                // Ensure the modelType has an "Oid" property
                var oidProperty = modelType.GetProperty(Properties.Oid);
                if (oidProperty == null)
                {
                    throw new ArgumentException(Errors.NullOid);
                }

                // Find the entity by its primary key
                var entity = data.Where($"{Properties.Oid} == \"{id}\"").FirstOrDefault();

                if (entity == null)
                {
                    throw new ArgumentException(Errors.EntityNotFound);
                }

                // Traverse navigation properties and set foreign keys to null
                foreach (var property in modelType.GetProperties())
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        // This property is a collection navigation property
                        var relatedEntityType = property.PropertyType.GetGenericArguments()[0];
                        var foreignKeyProperty = relatedEntityType.GetProperty($"{modelType.Name}");

                        if (foreignKeyProperty != null)
                        {
                            // Set the foreign key property to null for each related entity
                            var relatedEntities = GetTypedDbSet(dbContext, relatedEntityType)?.Where($"{modelType.Name} == \"{id}\"");
                            foreach (var relatedEntity in relatedEntities)
                            {
                                foreignKeyProperty.SetValue(relatedEntity, null);
                            }
                        }
                    }
                }

                // Set the 'Gcrecord' property to some value
                var isDeletedProperty = modelType.GetProperty(Properties.Gcrecord);
                if (isDeletedProperty != null)
                {
                    isDeletedProperty.SetValue(entity, 12345);
                }
                else
                {
                    throw new InvalidOperationException($"{Errors.SoftDeleteNotSuppurted} {modelType.Name}. 'Gcrecord' property not found.");
                }

                // Update the entity state to Modified
                dbContext.Entry(entity).State = EntityState.Modified;

                // Save changes to the database
                dbContext.SaveChanges();

                return true; // Soft delete successful
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return false; // Soft delete failed
            }
        }


        /// <summary>
        /// Removes an entity from a collection property of a model instance and saves changes to the DbContext.
        /// </summary>
        /// <param name="dbContext">The DbContext containing the model instance.</param>
        /// <param name="tabName">The name of the collection property from which to remove the entity.</param>
        /// <param name="modelType">The type of the model containing the collection property.</param>
        /// <param name="modelOid">The identifier of the model instance.</param>
        /// <param name="Oid">The identifier of the entity to remove from the collection.</param>
        /// <returns>True if the entity was successfully removed; otherwise, false.</returns>

        public bool RemoveFromSubList(ApplicationDbContext dbContext, string tabName, Type modelType, string modelOid, string Oid)
        {
            try
            {
                var entity = GetTypedDbSet( dbContext , modelType)?
                                .Where($"{Properties.Oid} == \"{modelOid}\"")
                                .FirstOrDefault();

                var collectionProperty = modelType.GetProperty(tabName);
                var collectionDbSetProperty = modelType.GetProperty(tabName);

                Type? collectionType = collectionProperty?.PropertyType;

                if (entity == null || collectionProperty == null || collectionDbSetProperty == null || collectionType == null || !collectionType.IsGenericType)
                {
                    return false;
                }

                // Get the generic type argument
                Type? innerType = collectionType?.GetGenericArguments().FirstOrDefault();

                var collectionEntity = GetTypedDbSet(dbContext, innerType)?
                                        .Where($"{Properties.Oid} == \"{Oid}\" && {modelType.Name} == \"{modelOid}\"")
                                        .FirstOrDefault();

                if (collectionEntity == null)
                {
                    return false;
                }

                var collectionDbSet = collectionDbSetProperty.GetValue(entity) as IList;

                if (collectionDbSet == null)
                {
                    return false;
                }

                // Remove the entity from the collection
                collectionDbSet.Remove(collectionEntity);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error removing entity from collection: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Retrieves a typed DbSet based on the specified model type from the ApplicationDbContext.
        /// </summary>
        /// <param name="_dbContext">The ApplicationDbContext from which to retrieve the DbSet.</param>
        /// <param name="modelType">The type of the model for which to retrieve the DbSet.</param>
        /// <returns>An IQueryable representing the typed DbSet; otherwise, null if an error occurs.</returns>

        public IQueryable? GetTypedDbSet(ApplicationDbContext _dbContext, Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                var typedDbSet = setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);

                return typedDbSet as IQueryable;
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"{Errors.ErrorGettingDbSet} {modelType.Name}");

                // Rethrow the exception for better diagnosis
                throw new InvalidOperationException($"{Errors.ErrorGettingDbSet} {modelType.Name}", ex);
            }
        }

    }
}