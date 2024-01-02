
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Helpers;
using PortalPOC.Models;

using System.Linq.Dynamic.Core;
using System.Reflection;

namespace PortalPOC.QueryFactory
{

    public class QueryFactory : IQueryFactory
    {

        public IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext _dbContext)
        {
            if (data == null)
            {
                return Enumerable.Empty<object>().AsQueryable();
            }

            var query = data;
            var modelProperties = modelType.GetProperties();
            var viewModelProperties = viewModelType.GetProperties();

            var selectProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => vp.Name == mp.Name || mp.Name == "Oid"));

            var leftJoinProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => mp.Name.Contains(vp.Name)) && QueriesHelper.IsNavigationProperty(mp));

            var combinedProperties = selectProperties
                .Select(sp => leftJoinProperties.FirstOrDefault(lp => lp.Name.Contains(sp.Name)) ?? sp);

            var selectExpressions = combinedProperties.Select(property =>
            {
                var propertyToJoin = property.Name switch
                {
                    "KioskApplicationNavigation" => "KappName",
                    _ => "Name"
                };

                var navigationProperty = property.Name.Contains("Navigation")
                    ? property.Name.Replace("Navigation", "")
                    : null;

                return !navigationProperty.IsNullOrEmpty()
                    ? $"{property.Name} != null ? {property.Name}.{propertyToJoin} : null as {navigationProperty}"
                    : $"{property.Name} as {property.Name}";
            });

          
            var selectProjection = string.Join(", ", selectExpressions);

            return query.Select($"new ({selectProjection})");

          
        }

        public void CreateInstance(QuavisQorchAdminEasyTestContext _dbContext,object modelInstance)
        {
            // Ensure the modelInstance is not null
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance), "Model instance cannot be null.");
            }

            modelInstance = QueriesHelper.SetStandardProperties(modelInstance);

            // Add the model instance to the DbContext
            _dbContext.Add(modelInstance);

            // Save changes to the database
            _dbContext.SaveChanges();
        }

        public object GetById(string entityId, IQueryable dbSet)
        {
           
            // Find the entity by its ID
            var entity = dbSet.Where($"Oid == \"{entityId}\"").FirstOrDefault();

            return entity;
        }

        public void UpdateInstance(QuavisQorchAdminEasyTestContext _dbContext, object existingEntity)
        {
            if (existingEntity == null)
            {
                throw new ArgumentNullException(nameof(existingEntity), "Entity to update cannot be null.");
            }

         
          
            _dbContext.Attach(existingEntity);
         

            // Set the entity state to Modified
            _dbContext.Entry(existingEntity).State = EntityState.Modified;

            // Save changes to the database
            _dbContext.SaveChanges();
        }

        public bool SoftDeleteInstance(QuavisQorchAdminEasyTestContext _dbContext,IQueryable data, Type modelType, string ID)
        {
            try
            {
                // Ensure the modelType has an "Oid" property
                var oidProperty = modelType.GetProperty("Oid");
                if (oidProperty == null)
                {
                    throw new ArgumentException("Primary key 'Oid' not found on the model type.");
                }

                // Find the entity by its primary key
                var entity = data.Where($"Oid == \"{ID}\"").FirstOrDefault();

                if (entity == null)
                {
                    throw new ArgumentException($"Entity with ID {ID} not found.");
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

                           
                            var relatedEntities = GetTypedDbSet(_dbContext,relatedEntityType)?.Where($"{modelType.Name}  == \"{ID}\"");
                            foreach (var relatedEntity in relatedEntities)
                            {
                                foreignKeyProperty.SetValue(relatedEntity, null);
                            }
                        }
                    }
                }

                // Set the 'Gcrecord' property to bla bla bla
                var isDeletedProperty = modelType.GetProperty("Gcrecord");
                if (isDeletedProperty != null)
                {
                    isDeletedProperty.SetValue(entity, 12345);
                }
                else
                {
                    throw new InvalidOperationException($"Soft delete is not supported for {modelType.Name}. 'Gcrecord' property not found.");
                }

                // Update the entity state to Modified
                _dbContext.Entry(entity).State = EntityState.Modified;

                // Save changes to the database
                _dbContext.SaveChanges();

                return true; // Soft delete successful
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return false; // Soft delete failed
            }
        }
        public IQueryable? GetTypedDbSet(QuavisQorchAdminEasyTestContext _dbContext,Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                var typedDbSet = setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);

                // Log or print the DbSet information for debugging
                Console.WriteLine($"Typed DbSet for {modelType.Name}: {typedDbSet}");

                return typedDbSet as IQueryable;
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"Error getting DbSet for type {modelType.Name}: {ex.Message}");

                // Rethrow the exception for better diagnosis
                throw new InvalidOperationException($"Error getting DbSet for type {modelType.Name}", ex);
            }
        }


    }
}
