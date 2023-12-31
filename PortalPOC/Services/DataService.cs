﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using PortalPOC.Helpers;
using PortalPOC.Models;
using PortalPOC.QueryFactory;
using System.Linq.Dynamic.Core;

using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace PortalPOC.Services
{
    public class DataService : IDataService
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

        private readonly IQueryFactory _queryFactory;

        public DataService(QuavisQorchAdminEasyTestContext dbContext, IQueryFactory queryFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryFactory = queryFactory;
        }


        #region GetData
        public IQueryable? GetTypedDbSet(Type modelType)
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



        public IQueryable ApplySearchFilter(IQueryable data, string? searchValue, Type modelType, Type viewModelType)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var commonProperties = modelType.GetProperties()
                    .Where(m => m.PropertyType == typeof(string) && viewModelType.GetProperty(m.Name) != null)
                    .Select(m => m.Name);

                var filterExpression = string.Join(" OR ", commonProperties.Select(p => $"{p}.ToLower().Contains(@0)"));

                return data.Where(filterExpression, searchValue.ToLower());
            }

            return data;
        }




        public IQueryable ApplySorting(IQueryable data, string? sortColumn, string? sortColumnDirection)
        {
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                return data.OrderBy($"{sortColumn} {sortColumnDirection}");
            }

            return data;
        }


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var dbSet = GetTypedDbSet(modelType);

            var data = dbSet?.Where("Gcrecord == null");

            data = _queryFactory.SelectAndJoinQuery(data,modelType, viewModelType, _dbContext);


            data = ApplySearchFilter(data, searchValue, modelType, viewModelType);

          
            data = ApplySorting(data, sortColumn, sortColumnDirection);

            return data;
        }

        #endregion



        public Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var namesDictionary = new Dictionary<string, List<dynamic>>();

            var guidProperties = modelType.GetProperties().Where(p => p.PropertyType == typeof(Guid?));

            foreach (var guidProperty in guidProperties)
            {
                if (string.IsNullOrEmpty(guidProperty.Name) || !modelTypeMapping.TryGetValue(guidProperty.Name, out var relatedTypeTuple))
                {
                    return namesDictionary; // Early return in case of an error
                }

                var names = GetRelatedEntityNames(relatedTypeTuple.Item1);

                namesDictionary[guidProperty.Name] = names;
            }

            return namesDictionary;
        }

        private List<dynamic> GetRelatedEntityNames(Type relatedEntityType)
        {
            var relatedEntities = GetTypedDbSet(relatedEntityType);
            return relatedEntities?.Where("Gcrecord == null").Select("new (Name as Name, Oid as Oid)").ToDynamicList();
        }

        
    }

}
