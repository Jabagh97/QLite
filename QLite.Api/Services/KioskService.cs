using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.DesignComponents;
using QLiteDataApi.Context;
using Serilog;
using System.Linq;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    public class KioskService 
    {
        private readonly IMemoryCache _cache;

        private readonly ApplicationDbContext _context;

        public KioskService(IMemoryCache cache, ApplicationDbContext context)
        {
            _cache = cache;

            _context = context;
        }

        #region New Ticket

        /// <summary>
        /// Generates a new ticket based on the provided request parameters, handling all related data creation and associations.
        /// </summary>
        /// <param name="req">The ticket request parameters.</param>
        /// <returns>The newly created ticket.</returns>
        public async Task<Ticket> GetNewTicketAsync(TicketRequestDto req)
        {
            // Initialize DbContext transaction at the beginning of the scope to ensure it covers all operations
            await using var dbContextTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Combined queries to minimize database round trips
                var (svcType, segment, ticketPool) = await GetTicketCreationDependenciesAsync(req.ServiceTypeId, req.SegmentId);

                ValidateTicketPool(ticketPool);
                int waitingTicketCount = await GetNumberOfWaitingTicketsAsync(req.ServiceTypeId, req.SegmentId);
                ValidateWaitingTicketCount(ticketPool, waitingTicketCount);

                var retNumber = await GenerateTicketNumberAsync(ticketPool);
                var newTicket = CreateNewTicket(svcType, segment, ticketPool, retNumber);
                var newTicketState = CreateNewTicketState(newTicket, svcType, segment);

                await SaveTicketAsync(newTicket, newTicketState);
                await dbContextTransaction.CommitAsync();

                newTicket.WaitingTickets = waitingTicketCount;

                return newTicket;
            }
            catch (Exception)
            {
                // Rollback transaction in case of failure
                await dbContextTransaction.RollbackAsync();
                throw; // Use throw; to preserve stack trace of the original exception
            }
        }

        /// <summary>
        /// Retrieves service type, segment, and ticket pool information necessary for creating a new ticket.
        /// </summary>
        /// <param name="serviceTypeId">The service type ID for the ticket.</param>
        /// <param name="segmentId">The segment ID for the ticket.</param>
        /// <returns>Tuple containing the service type, segment, and ticket pool.</returns>
        private async Task<(ServiceType, Segment, TicketPool)> GetTicketCreationDependenciesAsync(Guid serviceTypeId, Guid segmentId)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(serviceTypeId);
            var segment = await _context.Segments.FindAsync(segmentId);
            var ticketPool = await _context.TicketPools
                                            .FirstOrDefaultAsync(x => x.ServiceType == serviceTypeId && x.Segment == segmentId);

            if (serviceType == null || segment == null || ticketPool == null)
            {
                throw new InvalidOperationException("Failed to retrieve necessary data from the database");
            }

            return (serviceType, segment, ticketPool);
        }

        /// <summary>
        /// Creates a new ticket entity.
        /// </summary>
        /// <param name="svcType">The service type for the ticket.</param>
        /// <param name="segment">The segment for the ticket.</param>
        /// <param name="ticketPool">The ticket pool for the ticket.</param>
        /// <param name="retNumber">The ticket number.</param>
        /// <returns>The created ticket entity.</returns>
        private Ticket CreateNewTicket(ServiceType svcType, Segment segment, TicketPool ticketPool, int retNumber)
        {
            return new Ticket
            {
                Oid = Guid.NewGuid(),
                ServiceType = svcType.Oid,
                Segment = segment.Oid,
                Year = DateTime.Today.Year,
                DayOfYear = DateTime.Today.DayOfYear,
                Number = retNumber,
                ServiceTypeName = svcType?.Name,
                SegmentName = segment.Name,
                CurrentState = (int?)TicketStateEnum.Waiting,
                ToServiceType = ticketPool.ServiceType,
                LastOprTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDate = DateTime.Now,
                ModifiedDateUtc = DateTime.UtcNow,
                TicketPool = ticketPool.Oid,
                ServiceCode = ticketPool.ServiceCode,
                Branch = ticketPool.Branch,
                CopyNumber = ticketPool.CopyNumber,

            };
        }

        /// <summary>
        /// Creates a new ticket state for the given ticket.
        /// </summary>
        /// <param name="newTicket">The ticket to create the state for.</param>
        /// <param name="svcType">The service type associated with the ticket.</param>
        /// <param name="segment">The segment associated with the ticket.</param>
        /// <returns>The created ticket state.</returns>
        private TicketState CreateNewTicketState(Ticket newTicket, ServiceType svcType, Segment segment)
        {
            return new TicketState
            {
                Oid = Guid.NewGuid(),
                Ticket = newTicket.Oid,
                TicketStateValue = (int?)TicketStateEnum.Waiting,
                StartTime = DateTime.Now,
                TicketNumber = newTicket.Number,
                ServiceType = svcType?.Oid,
                Segment = segment.Oid,
                SegmentName = segment.Name,
                ServiceTypeName = svcType?.Name,
                Branch = newTicket.Branch,
                CreatedDate = DateTime.Now,
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDate = DateTime.Now,
                ModifiedDateUtc = DateTime.UtcNow,
                TicketNavigation = newTicket,
            };
        }

        /// <summary>
        /// Validates the ticket pool's availability based on the current time and the pool's defined service and break times.
        /// </summary>
        /// <param name="ticketPool">The ticket pool to validate.</param>
        private void ValidateTicketPool(TicketPool ticketPool)
        {
            var now = DateTime.Now.TimeOfDay;
            bool isServiceTimeValid = ticketPool.ServiceStartTime?.TimeOfDay < now && ticketPool.ServiceEndTime?.TimeOfDay > now;
            bool isBreakTimeInvalid = ticketPool.BreakStartTime?.TimeOfDay < now && ticketPool.BreakEndTime?.TimeOfDay > now;

            if (!isServiceTimeValid || isBreakTimeInvalid)
            {
                Log.Error("Ticket pool is currently unavailable.");

                throw new InvalidOperationException("Ticket pool is currently unavailable.");
            }
        }


        /// <summary>
        /// Validates the current count of waiting tickets against the ticket pool's maximum allowed waiting tickets.
        /// </summary>
        /// <param name="ticketPool">The ticket pool to check against.</param>
        /// <param name="waitingTicketCount">The current number of waiting tickets.</param>
        private void ValidateWaitingTicketCount(TicketPool ticketPool, int waitingTicketCount)
        {
            var now = DateTime.Now.TimeOfDay;
            if (ticketPool.MaxWaitingTicketCountControlTime?.TimeOfDay < now &&
                ticketPool.MaxWaitingTicketCount.HasValue &&
                ticketPool.MaxWaitingTicketCount < waitingTicketCount)
            {
                Log.Error("Maximum waiting ticket number is reached for this service type.");

                throw new InvalidOperationException("Maximum waiting ticket number is reached for this service type.");
            }
        }


        /// <summary>
        /// Generates the next ticket number within the range defined by the ticket pool.
        /// </summary>
        /// <param name="ticketPool">The ticket pool to generate the number for.</param>
        /// <returns>The generated ticket number.</returns>
        private async Task<int> GenerateTicketNumberAsync(TicketPool ticketPool)
        {
            var lastTicketNumber = await _context.Tickets
                .Where(t => t.ServiceType == ticketPool.ServiceType && t.Segment == ticketPool.Segment && t.TicketPool == ticketPool.Oid)
                .OrderByDescending(t => t.Number)
                .Select(t => t.Number)
                .FirstOrDefaultAsync();

            var retNumber = lastTicketNumber != null ? lastTicketNumber + 1 : ticketPool.RangeStart;

            if (retNumber > ticketPool.RangeEnd)
            {
                if (ticketPool.ResetOnRange ?? false)
                {
                    retNumber = ticketPool.RangeStart;
                }
                else
                {
                    Log.Error("Ticket pool range exceeded");

                    throw new Exception("Ticket pool range exceeded");
                }
            }
            else if (retNumber < ticketPool.RangeStart)
            {
                retNumber = ticketPool.RangeStart;
            }

            return (int)retNumber;
        }

        /// <summary>
        /// Retrieves the number of waiting tickets for a specific service type and segment.
        /// </summary>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="segmentId">The segment ID.</param>
        /// <returns>The count of waiting tickets.</returns>
        private async Task<int> GetNumberOfWaitingTicketsAsync(Guid serviceTypeId, Guid segmentId)
        {
            return await _context.Tickets.CountAsync(x => x.ServiceType == serviceTypeId &&
                                                          x.Segment == segmentId &&
                                                          x.CurrentState == (int)TicketStateEnum.Waiting &&
                                                          x.DayOfYear == DateTime.Today.DayOfYear &&
                                                          x.Year == DateTime.Today.Year);
        }

        /// <summary>
        /// Saves the newly created ticket and its initial state to the database.
        /// </summary>
        /// <param name="newTicket">The new ticket to save.</param>
        /// <param name="newTicketState">The initial state of the new ticket.</param>
        private async Task SaveTicketAsync(Ticket newTicket, TicketState newTicketState)
        {
            _context.Tickets.Add(newTicket);
            _context.TicketStates.Add(newTicketState);
            await _context.SaveChangesAsync();
        }


        #endregion

        /// <summary>
        /// Retrieves a list of service types available for a given segment, considering their availability based on the current time.
        /// </summary>
        /// <param name="segmentId">The segment ID to retrieve service types for.</param>
        /// <returns>A list of available service types for the segment.</returns>
        public async Task<List<ServiceTypeDto>> GetServiceTypes(Guid segmentId)
        {
            var cacheKey = $"ServicesForSegment_{segmentId}";

            if (!_cache.TryGetValue(cacheKey, out List<ServiceTypeDto> serviceTypeInfos))
            {
                var currentTime = DateTime.Now.TimeOfDay;

                // Fetch necessary data while still in IQueryable form, this includes TicketPools for time-based filtering
                var serviceTypes = await _context.ServiceTypes
                    .Where(st => st.Gcrecord == null && st.Parent == null)
                    .Include(st => st.TicketPools)
                    .Where(st => st.TicketPools.Any(tp => tp.Segment == segmentId && tp.NotAvailable != true))
                    .OrderBy(st => st.SeqNo)
                    .ToListAsync();

                // Apply time-based filtering in memory
                serviceTypeInfos = serviceTypes
                    .Where(st => st.TicketPools.All(tp =>
                        tp.ServiceStartTime == null ||
                        (tp.ServiceStartTime.Value.TimeOfDay < currentTime &&
                         (tp.ServiceEndTime == null || tp.ServiceEndTime.Value.TimeOfDay > currentTime) &&
                         (tp.BreakStartTime == null || !(tp.BreakStartTime.Value.TimeOfDay < currentTime && tp.BreakEndTime.Value.TimeOfDay > currentTime)))))
                    .Select(st => new ServiceTypeDto
                    {
                        Oid = st.Oid,
                        Name = st.Name
                    })
                    .ToList();

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                // Save data in cache
                _cache.Set(cacheKey, serviceTypeInfos, cacheEntryOptions);
            }

            return serviceTypeInfos;
        }


        /// <summary>
        /// Retrieves a list of all segments.
        /// </summary>
        /// <returns>A list of segments.</returns>
        public async Task<List<SegmentDto>> GetSegments()
        {
            // Try to get the cached segments
            if (!_cache.TryGetValue("SegmentsForKiosk", out List<SegmentDto> segments))
            {
                // Key not in cache, so get data from the database
                segments = await _context.Segments
                                          .Where(seg => seg.Gcrecord == null)
                                          .Select(seg => new SegmentDto
                                          {
                                              Name = seg.Name,
                                              Oid = seg.Oid
                                          })
                                          .ToListAsync();

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                       .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                       

                // Save data in cache
                _cache.Set("SegmentsForKiosk", segments, cacheEntryOptions);
            }

            return segments;
        }

        /// <summary>
        /// Retrieves information about a kiosk by its hardware ID (HwId).
        /// </summary>
        /// <param name="HwId">The hardware ID of the kiosk.</param>
        /// <returns>Kiosk information if found; otherwise, null.</returns>
        public async Task<KioskDto> GetKioskByHwID(string HwId)
        {
            var cacheKey = $"Kiosk_HwID_{HwId}";
            KioskDto kiosk;

            // Try to get the kiosk from the cache
            if (!_cache.TryGetValue(cacheKey, out kiosk))
            {
                try
                {
                    // Attempt to retrieve the kiosk from the database
                    kiosk = await _context.Kiosks
                                .Where(k => k.Gcrecord == null && k.Active == true && k.HwId == HwId)
                                .Select(k => new KioskDto
                                {
                                    Oid = k.Oid,
                                    Name = k.Name,
                                    HwId = k.HwId,
                                    KioskType = k.KioskType,
                                    Branch = k.Branch,
                                    WorkFlowType = k.WorkFlowType
                                })
                                .FirstAsync();

                    if (kiosk != null)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                        // Cache the kiosk
                        _cache.Set(cacheKey, kiosk, cacheEntryOptions);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred when retrieving the kiosk by HWID: {ex.Message}");

                    throw;
                }
            }

            return kiosk;
        }

        /// <summary>
        /// Retrieves the design associated with a kiosk for a specific step in the workflow.
        /// </summary>
        /// <param name="Step">The step in the workflow.</param>
        /// <param name="HwID">The hardware ID of the kiosk.</param>
        /// <returns>The design associated with the step and kiosk if found; otherwise, null.</returns>
        public async Task<Design> GetDesignByKiosk(string Step, string HwID)
        {
            // Define a unique cache key based on both Step and HwID
            var cacheKey = $"DesignByKiosk_Step_{Step}_HwID_{HwID}";

            if (!_cache.TryGetValue(cacheKey, out Design _design))
            {
                var kiosk = await _context.Kiosks.FirstOrDefaultAsync(k => k.HwId == HwID);

                if (kiosk == null)
                {
                    return null; // Early exit if kiosk is not found
                }

                if (!Enum.TryParse(Step, true, out Step wfStep))
                {
                    return null;
                }

                // Perform the query to get the design
                var targetWithDesign = await (
                    from target in _context.DesignTargets
                    join design in _context.Designs on target.Design equals design.Oid into designGroup
                    from d in designGroup.DefaultIfEmpty() // Left join
                    where target.Kiosk == kiosk.Oid && d != null && d.WfStep == (int)wfStep
                    select d
                ).FirstOrDefaultAsync();

                if (targetWithDesign != null)
                {
                    _design = targetWithDesign;

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                    // Save the design in cache
                    _cache.Set(cacheKey, _design, cacheEntryOptions);


                }
            }

            return _design;
        }

        /// <summary>
        /// Retrieves a list of resources.
        /// </summary>
        /// <returns>A list of resources.</returns>
        public async Task<List<Resource>> GetResourceList()
        {
            List<Resource> resources;
            string cacheKey = "resourceList";
            if (!_cache.TryGetValue(cacheKey, out resources))
            {
                resources = await _context.Resources.Where(r => r.Gcrecord == null).ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1)).SetSize(1);


                _cache.Set(cacheKey, resources, cacheEntryOptions);
            }

            return resources;
        }

        /// <summary>
        /// Retrieves a list of languages.
        /// </summary>
        /// <returns>A list of languages.</returns>
        public async Task<List<Language>> GetLanguageList()
        {
            List<Language> languages;
            string cacheKey = "languageList";
            if (!_cache.TryGetValue(cacheKey, out languages))
            {
                languages = await _context.Languages.Where(l => l.Gcrecord == null).ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1)).SetSize(1);


                _cache.Set(cacheKey, languages, cacheEntryOptions);
            }

            return languages;
        }

        /// <summary>
        /// Retrieves tickets that are currently in service for a specific kiosk.
        /// </summary>
        /// <param name="KioskHwID">The hardware ID of the kiosk.</param>
        /// <returns>A list of tickets currently in service.</returns>
        public async Task<List<TicketDto>> GetInServiceTickets(string KioskHwID)
        {
            // Get the kiosk
            var kiosk = await _context.Kiosks
                                      .Where(k => k.HwId == KioskHwID && k.Gcrecord == null)
                                      .FirstOrDefaultAsync();

            if (kiosk == null)
            {
                // If no kiosk is found, return an empty list or handle accordingly
                return new List<TicketDto>();
            }

            // Get desks associated with the kiosk
            var desks = await _context.Desks
                                      .Where(d => d.Kiosk == kiosk.Oid && d.Gcrecord == null)
                                      .ToListAsync();

            // Get the Oids of these desks to filter tickets
            var deskIds = desks.Select(d => d.Oid).ToList();

            // Now get Tickets where ticket's Desk is in deskIds
            var tickets = await _context.Tickets
                             .Where(t => t.Desk.HasValue && deskIds.Contains(t.Desk.Value) && t.CurrentState == (int)TicketStateEnum.Service && t.Gcrecord == null)
                             .Select(k => new TicketDto
                             {
                                 Oid = k.Oid,
                                 ServiceTypeName= k.ServiceTypeName,
                                 SegmentName = k.SegmentName,
                                 Number = k.Number,
                                 Desk = k.DeskNavigation.Name,
                                 ServiceCode= k.TicketPoolNavigation.ServiceCode,

                             })
                             .ToListAsync();


            return tickets;
        }

        /// <summary>
        /// Retrieves the default segment for the kiosks with only service type of WorkFlow.
        /// </summary>
        /// <returns>The default segment ID.</returns>
        public async Task<Guid> GetDefaultSegment()
        {
            var defaultSegment = await _context.Segments.Where(s => s.Default == true && s.Gcrecord == null).FirstOrDefaultAsync();

            if (defaultSegment == null)
                return Guid.Empty;
            return defaultSegment.Oid;
        }
    }
}
