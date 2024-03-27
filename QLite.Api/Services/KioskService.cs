using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.DesignComponents;
using QLiteDataApi.Context;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    public interface IKioskService
    {
        Task<Ticket> GetNewTicketAsync(TicketRequestDto req);

        Task<List<ServiceTypeDto>> GetServiceTypes(Guid segmentId);
        Task<List<SegmentDto>> GetSegments();

        Task<Kiosk> GetKioskByHwID(string HwId);

        Task<Design> GetDesignByKiosk(string Step, string HwID);
        Task<List<Resource>> GetResourceList();
        Task<List<Language>> GetLanguageList();
    }
    public class KioskService : IKioskService
    {
        private readonly IMemoryCache _cache;

        private readonly ApplicationDbContext _context;

        public KioskService(IMemoryCache cache, ApplicationDbContext context)
        {
            _cache = cache;

            _context = context;
        }

        #region New Ticket
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

        private void ValidateTicketPool(TicketPool ticketPool)
        {
            var now = DateTime.Now.TimeOfDay;
            bool isServiceTimeValid = ticketPool.ServiceStartTime?.TimeOfDay < now && ticketPool.ServiceEndTime?.TimeOfDay > now;
            bool isBreakTimeInvalid = ticketPool.BreakStartTime?.TimeOfDay < now && ticketPool.BreakEndTime?.TimeOfDay > now;

            if (!isServiceTimeValid || isBreakTimeInvalid)
            {
                throw new InvalidOperationException("Ticket pool is currently unavailable.");
            }
        }


        private void ValidateWaitingTicketCount(TicketPool ticketPool, int waitingTicketCount)
        {
            var now = DateTime.Now.TimeOfDay;
            if (ticketPool.MaxWaitingTicketCountControlTime?.TimeOfDay < now &&
                ticketPool.MaxWaitingTicketCount.HasValue &&
                ticketPool.MaxWaitingTicketCount < waitingTicketCount)
            {
                throw new InvalidOperationException("Maximum waiting ticket number is reached for this service type.");
            }
        }


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
                    throw new Exception("Ticket pool range exceeded");
                }
            }
            else if (retNumber < ticketPool.RangeStart)
            {
                retNumber = ticketPool.RangeStart;
            }

            return (int)retNumber;
        }
        private async Task<int> GetNumberOfWaitingTicketsAsync(Guid serviceTypeId, Guid segmentId)
        {
            return await _context.Tickets.CountAsync(x => x.ServiceType == serviceTypeId &&
                                                          x.Segment == segmentId &&
                                                          x.CurrentState == (int)TicketStateEnum.Waiting &&
                                                          x.DayOfYear == DateTime.Today.DayOfYear &&
                                                          x.Year == DateTime.Today.Year);
        }

        private async Task SaveTicketAsync(Ticket newTicket, TicketState newTicketState)
        {
            _context.Tickets.Add(newTicket);
            _context.TicketStates.Add(newTicketState);
            await _context.SaveChangesAsync();
        }


        #endregion


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
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetSize(1);

                // Save data in cache
                _cache.Set(cacheKey, serviceTypeInfos, cacheEntryOptions);
            }

            return serviceTypeInfos;
        }




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
                       .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                       .SetSize(1);

                // Save data in cache
                _cache.Set("SegmentsForKiosk", segments, cacheEntryOptions);
            }

            return segments;
        }


        public async Task<Kiosk> GetKioskByHwID(string HwId)
        {
            var cacheKey = $"Kiosk_HwID_{HwId}";

            if (!_cache.TryGetValue(cacheKey, out Kiosk kiosk))
            {
                kiosk = await _context.Kiosks.FirstOrDefaultAsync(k => k.Gcrecord == null && k.Active == true && k.HwId == HwId);

                if (kiosk != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1)).SetSize(1);

                    _cache.Set(cacheKey, kiosk, cacheEntryOptions);
                }
            }

            return kiosk;
        }



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

                if (!Enum.TryParse(Step, true, out WfStep wfStep)) // Case insensitive parsing
                {
                    // Optionally, log this error or handle it as needed
                    return null; // Handle invalid step value by returning null or consider logging
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
                        .SetSlidingExpiration(TimeSpan.FromMinutes(1)).SetSize(1);

                    // Save the design in cache
                    _cache.Set(cacheKey, _design, cacheEntryOptions);


                }
            }

            return _design;
        }

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
    }
}
