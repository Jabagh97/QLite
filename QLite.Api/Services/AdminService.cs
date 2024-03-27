using Microsoft.EntityFrameworkCore;
using QLiteDataApi.Constants;

using QLiteDataApi.QueryFactory;
using QLite.Data;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using QLiteDataApi.Helpers;
using QLiteDataApi.Context;
using QLite.DesignComponents;
using Newtonsoft.Json;

namespace QLiteDataApi.Services
{
    public interface IAdminService
    {


        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection);

        Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping);

        object CreateModel(string user, Type modelType, Dictionary<string, object> formData);
        object UpdateModel(string user, Type modelType, Dictionary<string, object> formData);

        bool SoftDelete(Type modelType, Dictionary<string, object> formData);

        IQueryable GetTabData(Type innerType, Type innerViewType, string Oid, Type mainModelType);

        bool RemoveFromSubList(string tabName, Type modelType, string modelOid, List<string> Oids);

        List<Desk> GetAllDesks();


        Design GetDesign(Guid DesignID);

        bool SaveDesign(Guid DesignID, DesPageData desPageData, string designImage);


        Task<List<Segment>> GetSegmentList();

        Task<List<ServiceType>> GetServiceList();

        Task<List<Design>> GetDesignList();
        Task<string> GetDesignImageByID(Guid designID);
        Task<string> GetTicketStateReport(DateTime StartDate, DateTime EndDate);
        Task<List<Language>> GetLanguageList();
    }
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly IQueryFactory _queryFactory;

        public AdminService(ApplicationDbContext dbContext, IQueryFactory queryFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));

        }
        public IQueryable? GetTypedDbSet(Type modelType)
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
                Console.WriteLine($"{Errors.ErrorGettingDbSet} {modelType.Name}: {ex.Message}");

                // Rethrow the exception for better diagnosis
                throw new InvalidOperationException($"{Errors.ErrorGettingDbSet} {modelType.Name}: {ex.Message}", ex);
            }
        }


        #region GetData
        public async Task<List<Language>> GetLanguageList()
        {
            var Languages = await _dbContext.Languages.Where(d => d.Gcrecord == null).ToListAsync();

            return Languages;
        }
        public async Task<string> GetTicketStateReport(DateTime startDate, DateTime endDate)
        {
            // Fetch the necessary data, avoiding complex translations
            var tickets = await _dbContext.TicketStates
                                          .Where(t => t.StartTime >= startDate && (t.EndTime <= endDate || t.EndTime == null))
                                          .Include(t => t.DeskNavigation)
                                          .ToListAsync();

            // Perform the grouping and aggregation in-memory
            var reportData = tickets
                                .GroupBy(t => t.DeskNavigation != null ? t.DeskNavigation.Name : "Not Called")
                                .Select(g => new
                                {
                                    DeskName = g.Key,
                                    WaitingTickets = g.Count(t => t.TicketStateValue == 0),
                                    TransferedTickets = g.Count(t => t.TicketStateValue == 1),
                                    TicketsInProcess = g.Count(t => t.TicketStateValue == 2),
                                    Parked = g.Count(t => t.TicketStateValue == 3),
                                    ServedTickets = g.Count(t => t.TicketStateValue == 4),
                                    TotalTickets = g.Count(),
                                    WaitingTime = QueriesHelper.FormatTime(
                                        g.Where(t => t.TicketStateValue == 0)
                                            .Select(t => (t.EndTime ?? DateTime.Now) - t.StartTime)
                                            .DefaultIfEmpty() // To ensure there's at least one element
                                            .Average(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0)
                                    ),
                                    TransferTime = QueriesHelper.FormatTime(g.Where(t => t.TicketStateValue == 1)
                                        .Select(t => (t.EndTime ?? DateTime.Now) - t.StartTime)
                                        .DefaultIfEmpty()
                                        .Average(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0)
                                    ),
                                    NowInServiceTime = QueriesHelper.FormatTime(g.Where(t => t.TicketStateValue == 2)
                                        .Select(t => (t.EndTime ?? DateTime.Now) - t.StartTime)
                                        .DefaultIfEmpty()
                                        .Average(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0)
                                    ),
                                    ParkedTime = QueriesHelper.FormatTime(g.Where(t => t.TicketStateValue == 3)
                                        .Select(t => (t.EndTime ?? DateTime.Now) - t.StartTime)
                                        .DefaultIfEmpty()
                                        .Average(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0)
                                    ),
                                    ServedTime = QueriesHelper.FormatTime(g.Where(t => t.TicketStateValue == 4)
                                        .Select(t => (t.EndTime ?? DateTime.Now) - t.StartTime)
                                        .DefaultIfEmpty()
                                        .Average(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0)
                                    ),
                                    TotalTime = QueriesHelper.FormatTime(g.Select(t => (t.EndTime ?? DateTime.Now) - (t.StartTime ?? DateTime.Now))
                                        .Sum(ts => ((TimeSpan?)ts)?.TotalMinutes ?? 0))
                                })
                                .ToList();



            // Serialize the report data to JSON
            string reportJson = JsonConvert.SerializeObject(reportData);

            return reportJson;
        }



        public List<Desk> GetAllDesks()
        {
            var DeskList = _dbContext.Desks.Where(d => d.Gcrecord == null).ToList();

            return DeskList;
        }

        public IQueryable ApplySearchFilter(IQueryable data, string? searchValue, Type modelType, Type viewModelType)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var commonProperties = modelType.GetProperties()
                    .Where(m => m.PropertyType == typeof(string) && viewModelType.GetProperty(m.Name) != null)
                    .Select(m => m.Name);

                var filterExpression = string.Join(" OR ", commonProperties.Select(p => $"{p}.ToLower().Contains(@0)"));

                return data.Where(filterExpression, searchValue);
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


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection)
        {
            var dbSet = GetTypedDbSet(modelType);
            var data = dbSet;


            IQuery Model = _queryFactory.GetModel(modelType.Name);

            data = Model.SelectAndJoinQuery(data, modelType, viewModelType, _dbContext);


            data = ApplySearchFilter(data, searchValue, modelType, viewModelType);
            data = ApplySorting(data, sortColumn, sortColumnDirection);

            return data;
        }


        public IQueryable GetTabData(Type innerType, Type innerViewType, string Oid, Type mainModelType)
        {
            var dbSet = GetTypedDbSet(innerType);
            var data = dbSet;

            IQuery Model = _queryFactory.GetModel(innerType.Name);


            var optionalArguments = new Dictionary<string, object>
            {
                [mainModelType.Name] = Oid,
            };

            data = Model.SelectAndJoinQuery(data, innerType, innerViewType, _dbContext, optionalArguments);

            return data;
        }


        public Design GetDesign(Guid DesignID)
        {
            var Design = _dbContext.Designs.Where(d => d.Oid == DesignID && d.Gcrecord == null).FirstOrDefault();
            return Design;
        }

        public async Task<List<Design>> GetDesignList()
        {
            var Designs = await _dbContext.Designs.Where(d => d.Gcrecord == null).ToListAsync();
            return Designs;
        }
        public async Task<List<Segment>> GetSegmentList()
        {
            List<Segment> segments = await _dbContext.Segments
                                            .Where(s => s.Gcrecord == null)
                                            .ToListAsync();

            return segments;
        }

        public async Task<List<ServiceType>> GetServiceList()
        {
            var services = await _dbContext.ServiceTypes.Where(s => s.Gcrecord == null).ToListAsync();

            return services;
        }


        #endregion


        #region Create&EditData
        public Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var namesDictionary = new Dictionary<string, List<dynamic>>();

            var guidProperties = modelType.GetProperties().Where(p => p.PropertyType == typeof(Guid?));

            foreach (var guidProperty in guidProperties)
            {
                if (string.IsNullOrEmpty(guidProperty.Name) || !modelTypeMapping.TryGetValue(guidProperty.Name, out var relatedTypeTuple))
                {
                    continue; // Skip to the next property in case of an error
                }

                var names = GetRelatedEntityNames(relatedTypeTuple.Item1);

                namesDictionary[guidProperty.Name] = names;
            }

            return namesDictionary;
        }

        private List<dynamic> GetRelatedEntityNames(Type relatedEntityType)
        {
            var propertyName = Properties.Name;

            var relatedEntities = GetTypedDbSet(relatedEntityType);
            return relatedEntities?.Where($"{Properties.Gcrecord} == null").Select($"new ({propertyName} as {propertyName}, {Properties.Oid} as {Properties.Oid})").ToDynamicList()
                ?? new List<dynamic>();
        }

        public object CreateModel(string user, Type modelType, Dictionary<string, object> formData)
        {
            // Validate and create a model instance
            var modelInstance = Activator.CreateInstance(modelType);

            if (modelInstance == null)
            {
                throw new InvalidOperationException(Errors.NotCreated);
            }

            foreach (var property in formData)
            {
                var propertyInfo = modelType.GetProperty(property.Key);

                if (propertyInfo != null)
                {
                    // Convert the value to the property type
                    var convertedValue = QueriesHelper.ConvertToType(property.Value?.ToString(), propertyInfo.PropertyType);

                    // Set the property value
                    propertyInfo.SetValue(modelInstance, convertedValue);
                }
            }

            IQuery Model = _queryFactory.GetModel(modelType.Name);

            // Save the created model instance to your data store or perform any necessary operations
            Model.CreateInstance(user, _dbContext, modelInstance!);

            // Return the created model instance
            return modelInstance;
        }

        public object UpdateModel(string user, Type modelType, Dictionary<string, object> formData)
        {
            // Validate and update the model instance
            if (!formData.ContainsKey(Properties.Oid))
            {
                throw new ArgumentException(Errors.NullOid);
            }

            var idValue = formData[Properties.Oid].ToString();

            var dbSet = GetTypedDbSet(modelType);

            IQuery Model = _queryFactory.GetModel(modelType.Name);

            // Fetch the existing entity from your data store
            var existingEntity = Model.GetById(idValue, dbSet);

            if (existingEntity == null)
            {
                throw new ArgumentException(Errors.EntityNotFound);
            }

            foreach (var property in formData)
            {
                var propertyName = property.Key;
                var propertyInfo = modelType.GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    // Convert the value to the property type
                    var convertedValue = QueriesHelper.ConvertToType(property.Value?.ToString(), propertyInfo.PropertyType);

                    // Set the property value
                    propertyInfo.SetValue(existingEntity, convertedValue);
                }
            }

            // Save changes to your data store or perform any necessary operations
            Model.UpdateInstance(user, _dbContext, existingEntity);

            // Return the updated model instance
            return existingEntity;
        }
        public bool SaveDesign(Guid DesignID, DesPageData desPageData, string designImage)
        {
            var designEntity = _dbContext.Designs.FirstOrDefault(d => d.Oid == DesignID);

            if (designEntity != null)
            {
                var desPageDataTest = JsonConvert.SerializeObject(desPageData);

                designEntity.DesignData = desPageDataTest;
                designEntity.DesignImage = designImage;

                _dbContext.Update(designEntity);
                _dbContext.SaveChanges();

                return true; // Indicate that the design was saved successfully
            }

            return false; // Indicate that the design was not found
        }


        #endregion


        #region SoftDelete

        public bool SoftDelete(Type modelType, Dictionary<string, object> formData)
        {
            try
            {
                // Ensure the dictionary contains the primary key value
                if (!formData.TryGetValue(Properties.Oid, out var oidJsonElement) || oidJsonElement == null)
                {
                    throw new ArgumentException(Errors.NullOid);
                }

                // Extract primary key values
                var primaryKeyValues = System.Text.Json.JsonSerializer.Deserialize<List<string>>(oidJsonElement.ToString());

                // Get the DbSet for the specified model type
                var dbSet = GetTypedDbSet(modelType);

                foreach (var primaryKeyValue in primaryKeyValues)
                {
                    // Trim and replace as needed
                    var cleanedPrimaryKeyValue = primaryKeyValue?.Trim('[', ']').Replace("\"", "");

                    if (!string.IsNullOrEmpty(cleanedPrimaryKeyValue))
                    {
                        IQuery Model = _queryFactory.GetModel(modelType.Name);

                        // Find the entity by its primary key
                        var entity = Model.SoftDeleteInstance(_dbContext, dbSet, modelType, cleanedPrimaryKeyValue);

                    }
                }

                return true; // Soft delete successful
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return false; // Soft delete failed
            }
        }

        public bool RemoveFromSubList(string tabName, Type modelType, string modelOid, List<string> Oids)
        {
            IQuery Model = _queryFactory.GetModel(modelType.Name);

            bool allSuccess = true;

            foreach (var oid in Oids)
            {
                var result = Model.RemoveFromSubList(_dbContext, tabName, modelType, modelOid, oid);

                // Update the 'allSuccess' flag based on the result of each iteration
                allSuccess = allSuccess && result;
            }

            return allSuccess;
        }

        public async Task<string> GetDesignImageByID(Guid designID)
        {
            Design design = await _dbContext.Designs.Where(d => d.Oid == designID && d.Gcrecord == null).FirstOrDefaultAsync();


            return design.DesignImage;
        }

      




        #endregion



    }

}
