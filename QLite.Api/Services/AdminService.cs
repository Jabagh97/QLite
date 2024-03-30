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
    /// <summary>
    /// Provides administrative functionalities to manage application data, including languages, desks, designs, segments,....etc.
    /// Utilizes dynamic queries for data manipulation and supports CRUD operations.
    /// </summary>

    public class AdminService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly DynamicQueriesService _dynamicQueries;

        public AdminService(ApplicationDbContext dbContext, DynamicQueriesService dynamicQueries)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dynamicQueries = dynamicQueries ?? throw new ArgumentNullException(nameof(dynamicQueries));

        }

        /// <summary>
        /// Retrieves a typed DbSet based on the specified model type.
        /// </summary>
        /// <param name="modelType">The type of the model for which to retrieve the DbSet.</param>
        /// <returns>An IQueryable representing the typed DbSet; null if the operation fails.</returns>

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

        /// <summary>
        /// Retrieves a list of all languages from the database, excluding those marked as deleted.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Language entities.</returns>
        public async Task<List<Language>> GetLanguageList()
        {
            var Languages = await _dbContext.Languages.Where(d => d.Gcrecord == null).ToListAsync();

            return Languages;
        }

        /// <summary>
        /// Generates a report summarizing ticket states within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the report period.</param>
        /// <param name="endDate">The end date of the report period.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a JSON string representing the report data.</returns>
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

        /// <summary>
        /// Retrieves all desks from the database, excluding those marked as deleted.
        /// </summary>
        /// <returns>A list of Desk entities.</returns>
        public List<Desk> GetAllDesks()
        {
            var DeskList = _dbContext.Desks.Where(d => d.Gcrecord == null).ToList();

            return DeskList;
        }

        /// <summary>
        /// Applies a search filter to the given data based on the search value.
        /// </summary>
        /// <param name="data">The IQueryable data to filter.</param>
        /// <param name="searchValue">The search query value.</param>
        /// <param name="modelType">The type of the model being queried.</param>
        /// <param name="viewModelType">The type of the view model that determines searchable properties.</param>
        /// <returns>An IQueryable representing the filtered data.</returns>
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

        /// <summary>
        /// Applies sorting to the given data based on the specified sort column and direction.
        /// </summary>
        /// <param name="data">The IQueryable data to sort.</param>
        /// <param name="sortColumn">The column name to sort by.</param>
        /// <param name="sortColumnDirection">The sort direction ("asc" or "desc").</param>
        /// <returns>An IQueryable representing the sorted data.</returns>
        public IQueryable ApplySorting(IQueryable data, string? sortColumn, string? sortColumnDirection)
        {
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                return data.OrderBy($"{sortColumn} {sortColumnDirection}");
            }

            return data;
        }

        /// <summary>
        /// Retrieves data for a specific model, applying filters, sorting, and pagination.
        /// </summary>
        /// <param name="modelType">The type of the model.</param>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <returns>An IQueryable representing the filtered, sorted, and paginated data.</returns>
        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType)
        {
            var dbSet = GetTypedDbSet(modelType);
            var data = dbSet;

            data = _dynamicQueries.SelectAndJoinQuery(data, modelType, viewModelType, _dbContext);
            return data;
        }


        /// <summary>
        /// Retrieves data for a specific tab related to a main model entity, applying dynamic queries for additional filtering based on optional arguments.
        /// </summary>
        /// <param name="innerType">The type of the entities contained in the tab.</param>
        /// <param name="innerViewType">The view model type associated with the tab entities for determining which properties to include.</param>
        /// <param name="Oid">The unique identifier of the main model entity to which the tab data is related.</param>
        /// <param name="mainModelType">The type of the main model entity.</param>
        /// <returns>An IQueryable representing the dynamically constructed query for the tab data.</returns>
        public IQueryable GetTabData(Type innerType, Type innerViewType, string Oid, Type mainModelType)
        {
            var dbSet = GetTypedDbSet(innerType);
            var data = dbSet;

          
            var optionalArguments = new Dictionary<string, object>
            {
                [mainModelType.Name] = Oid,
            };

            data = _dynamicQueries.SelectAndJoinQuery(data, innerType, innerViewType, _dbContext, optionalArguments);

            return data;
        }

        /// <summary>
        /// Retrieves a specific design by its unique identifier, excluding those marked as deleted.
        /// </summary>
        /// <param name="DesignID">The unique identifier of the design to retrieve.</param>
        /// <returns>The Design entity if found; otherwise, null.</returns>
        public Design GetDesign(Guid DesignID)
        {
            var Design = _dbContext.Designs.Where(d => d.Oid == DesignID && d.Gcrecord == null).FirstOrDefault();
            return Design;
        }

        /// <summary>
        /// Asynchronously retrieves a list of all designs from the database, excluding those marked as deleted.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Design entities.</returns>
        public async Task<List<Design>> GetDesignList()
        {
            var Designs = await _dbContext.Designs.Where(d => d.Gcrecord == null).ToListAsync();
            return Designs;
        }

        /// <summary>
        /// Asynchronously retrieves a list of all segments from the database, excluding those marked as deleted.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of Segment entities.</returns>
        public async Task<List<Segment>> GetSegmentList()
        {
            List<Segment> segments = await _dbContext.Segments
                                            .Where(s => s.Gcrecord == null)
                                            .ToListAsync();

            return segments;
        }


        /// <summary>
        /// Asynchronously retrieves a list of all service types from the database, excluding those marked as deleted.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of ServiceType entities.</returns>
        public async Task<List<ServiceType>> GetServiceList()
        {
            var services = await _dbContext.ServiceTypes.Where(s => s.Gcrecord == null).ToListAsync();

            return services;
        }


        #endregion


        #region Create&EditData

        /// <summary>
        /// Retrieves a dictionary of GUID property names and their corresponding list of entity names and IDs, based on the specified model type.
        /// </summary>
        /// <param name="modelType">The model type to inspect for GUID properties.</param>
        /// <param name="modelTypeMapping">A dictionary mapping model names to their corresponding type and view model type tuples.</param>
        /// <returns>A dictionary where keys are property names and values are lists of dynamic objects containing entity names and their IDs.</returns>
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

        /// <summary>
        /// Retrieves a list of names and IDs for entities related to a specific GUID property.
        /// </summary>
        /// <param name="relatedEntityType">The entity type related to the GUID property.</param>
        /// <returns>A list of dynamic objects each containing the name and ID of a related entity.</returns>
        private List<dynamic> GetRelatedEntityNames(Type relatedEntityType)
        {
            var propertyName = Properties.Name;

            var relatedEntities = GetTypedDbSet(relatedEntityType);
            return relatedEntities?.Where($"{Properties.Gcrecord} == null").Select($"new ({propertyName} as {propertyName}, {Properties.Oid} as {Properties.Oid})").ToDynamicList()
                ?? new List<dynamic>();
        }


        /// <summary>
        /// Creates a new model instance based on the provided form data and saves it to the database.
        /// </summary>
        /// <param name="user">The user performing the create operation.</param>
        /// <param name="modelType">The type of the model to create.</param>
        /// <param name="formData">A dictionary containing the data for the new model instance.</param>
        /// <returns>The created model instance.</returns>

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



            _dynamicQueries.CreateInstance(user, _dbContext, modelInstance!);

            // Return the created model instance
            return modelInstance;
        }

        /// <summary>
        /// Updates an existing model instance with the provided form data and saves changes to the database.
        /// </summary>
        /// <param name="user">The user performing the update operation.</param>
        /// <param name="modelType">The type of the model to update.</param>
        /// <param name="formData">A dictionary containing the updated data for the model instance.</param>
        /// <returns>The updated model instance.</returns>
        public object UpdateModel(string user, Type modelType, Dictionary<string, object> formData)
        {
            // Validate and update the model instance
            if (!formData.ContainsKey(Properties.Oid))
            {
                throw new ArgumentException(Errors.NullOid);
            }

            var idValue = formData[Properties.Oid].ToString();

            var dbSet = GetTypedDbSet(modelType);

          

            // Fetch the existing entity from your data store
            var existingEntity = _dynamicQueries.GetById(idValue, dbSet);

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
            _dynamicQueries.UpdateInstance(user, _dbContext, existingEntity);

            // Return the updated model instance
            return existingEntity;
        }
        /// <summary>
        /// Saves or updates a design entity with the provided design data and image.
        /// </summary>
        /// <param name="DesignID">The unique identifier of the design to save or update.</param>
        /// <param name="desPageData">The design page data to be saved or updated.</param>
        /// <param name="designImage">The design image in base64 string format.</param>
        /// <returns>True if the design is successfully saved or updated; otherwise, false.</returns>
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

        /// <summary>
        /// Performs a soft delete operation on a model instance by marking it as deleted without physically removing it from the database.
        /// </summary>
        /// <param name="modelType">The type of the model to soft delete.</param>
        /// <param name="formData">A dictionary containing the primary key value of the model instance to delete.</param>
        /// <returns>True if the soft delete operation was successful; otherwise, false.</returns>

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
                        
                        // Find the entity by its primary key
                        var entity = _dynamicQueries.SoftDeleteInstance(_dbContext, dbSet, modelType, cleanedPrimaryKeyValue);

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
            

            bool allSuccess = true;

            foreach (var oid in Oids)
            {
                var result = _dynamicQueries.RemoveFromSubList(_dbContext, tabName, modelType, modelOid, oid);

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
