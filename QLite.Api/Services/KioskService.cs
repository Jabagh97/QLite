using Microsoft.EntityFrameworkCore;
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

        Task<List<ServiceType>> GetServiceTypes(Guid segmentId);
        List<Segment> GetSegments();

        List<Kiosk> GetKioskByHwID(string HwId);

        Task<Design> GetDesignByKiosk(string Step, string HwID);

    }
    public class KioskService : IKioskService
    {
        private readonly ApplicationDbContext _context;

        public KioskService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region New Ticket
        public async Task<Ticket> GetNewTicketAsync(TicketRequestDto req)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var svcType =  GetServiceType(req.ServiceTypeId);
                    var segment =  GetSegment(req.SegmentId);
                    var ticketPool =  GetTicketPool(req.ServiceTypeId, req.SegmentId);

                    if (svcType == null || segment == null || ticketPool == null)
                    {
                        throw new Exception("Failed to retrieve necessary data from the database");
                    }

                    ValidateTicketPool(ticketPool);
                    ValidateWaitingTicketCount(ticketPool, req.ServiceTypeId, req.SegmentId);

                    var retNumber = GenerateTicketNumber(ticketPool);

                    var newTicket = CreateNewTicket(svcType, segment, ticketPool, retNumber);

                    var newTicketState = CreateNewTicketState(newTicket, svcType, segment);

                    var waiting =  GetNumberOfWaitingTickets(req.ServiceTypeId, req.SegmentId);

                    newTicket.WaitingTickets = waiting;

                    await SaveTicketAsync(newTicket, newTicketState);

                    await dbContextTransaction.CommitAsync();

                    return newTicket;
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of failure
                    await dbContextTransaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        private ServiceType? GetServiceType(Guid serviceTypeId)
        {
            return _context.ServiceTypes.Find(serviceTypeId);
        }

        private Segment? GetSegment(Guid segmentId)
        {
            return _context.Segments.Find(segmentId);
        }

        private TicketPool GetTicketPool(Guid serviceTypeId, Guid segmentId)
        {
            var ticketPool = _context.TicketPools
                .FirstOrDefault(x => x.ServiceType == serviceTypeId && x.Segment == segmentId);

            if (ticketPool == null)
            {
                throw new Exception("Ticket pool not defined for this service type");
            }

            return ticketPool;
        }

        private void ValidateTicketPool(TicketPool ticketPool)
        {
            if (!(ticketPool.ServiceStartTime?.TimeOfDay < DateTime.Now.TimeOfDay &&
                  ticketPool.ServiceEndTime?.AddSeconds(-1).TimeOfDay > DateTime.Now.TimeOfDay &&
                  !(ticketPool.BreakStartTime?.TimeOfDay < DateTime.Now.TimeOfDay &&
                    ticketPool.BreakEndTime?.TimeOfDay > DateTime.Now.TimeOfDay)))
            {
                throw new Exception("Ticket pool is currently unavailable");
            }
        }

        private void ValidateWaitingTicketCount(TicketPool ticketPool, Guid serviceTypeId, Guid segmentId)
        {
            if (ticketPool.MaxWaitingTicketCountControlTime?.TimeOfDay > DateTime.Now.TimeOfDay)
            {
                int waitingTicketCount = GetNumberOfWaitingTickets(serviceTypeId, segmentId);
                if (ticketPool.MaxWaitingTicketCount != null && ticketPool.MaxWaitingTicketCount < waitingTicketCount)
                {
                    throw new Exception("MaximumWaitingTicket number is reached for this service type");
                }
            }
        }

        private int GetNumberOfWaitingTickets(Guid stId, Guid sgmId)
        {
            return _context.Tickets
                .Count(x => x.ServiceType == stId &&
                            x.Segment == sgmId &&
                            x.CurrentState == (int?)TicketStateEnum.Waiting &&
                            x.DayOfYear == DateTime.Today.DayOfYear &&
                            x.Year == DateTime.Today.Year);
        }

        private int GenerateTicketNumber(TicketPool ticketPool)
        {
            var lastTicketNumber = _context.Tickets
                .Where(t => t.ServiceType == ticketPool.ServiceType && t.Segment == ticketPool.Segment && t.TicketPool == ticketPool.Oid)
                .OrderByDescending(t => t.Number)
                .Select(t => t.Number)
                .FirstOrDefault();

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
                ModifiedDateUtc= DateTime.UtcNow,
                TicketPool = ticketPool.Oid,
                ServiceCode = ticketPool.ServiceCode,
                Branch= ticketPool.Branch,
                CopyNumber= ticketPool.CopyNumber,

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
                Branch= newTicket.Branch,
                CreatedDate = DateTime.Now,
                CreatedDateUtc = DateTime.UtcNow,
                ModifiedDate = DateTime.Now,
                ModifiedDateUtc = DateTime.UtcNow,
                TicketNavigation=newTicket,
            };
        }

        private async Task SaveTicketAsync(Ticket newTicket, TicketState newTicketState)
        {
            _context.Tickets.Add(newTicket);
            _context.TicketStates.Add(newTicketState);
            await _context.SaveChangesAsync();
        }


        #endregion


        public async Task<List<ServiceType>> GetServiceTypes(Guid segmentId)
        {
            var currentTime = DateTime.Now.TimeOfDay;

            var serviceTypes = await _context.ServiceTypes
                            .Where(st => st.Gcrecord == null && st.Parent == null)
                            .Include(st => st.TicketPools)
                            .Where(st => st.TicketPools.Any(tp => tp.Segment == segmentId  &&
                                                                   tp.NotAvailable != true ))
                            .OrderBy(st => st.SeqNo)
                            .ToListAsync();


            // Apply time-based filtering in memory
            serviceTypes =  serviceTypes.Where(st =>
                st.TicketPools.All(tp =>
                    tp.ServiceStartTime == null ||
                    (tp.ServiceStartTime.Value.TimeOfDay < currentTime &&
                     (tp.ServiceEndTime == null || tp.ServiceEndTime.Value.TimeOfDay > currentTime) &&
                     (tp.BreakStartTime == null || !(tp.BreakStartTime.Value.TimeOfDay < currentTime && tp.BreakEndTime.Value.TimeOfDay > currentTime)))))
                .ToList();

            return serviceTypes;
        }




        public List<Segment> GetSegments()
        {
            var segments = _context.Segments.Where(seg => seg.Gcrecord == null)
                 .ToList();

            return segments;
        }


        public List<Kiosk> GetKioskByHwID(string HwId)
        {
            var kiosk = _context.Kiosks.Where(k => k.Gcrecord == null && k.Active ==true && k.HwId == HwId)
                 .ToList();

            return kiosk;
        }

        public async Task<Design> GetDesignByKiosk(string Step, string HwID)
        {
            var kiosk = await _context.Kiosks.FirstOrDefaultAsync(k => k.HwId == HwID);

            if (kiosk == null)
            {
                return null;
            }
            if (!Enum.TryParse(Step, out WfStep wfStep))
            {
                // Handle invalid step value here, perhaps return null or throw an exception
                return null;
            }
            // Left join DesignTargets with Designs
            var targetsWithDesigns = await (
                from target in _context.DesignTargets
                join design in _context.Designs
                    on target.Design equals design.Oid into designGroup
                from d in designGroup.DefaultIfEmpty() // Left join
                where target.Kiosk == kiosk.Oid
                select new { Target = target, Design = d }
            ).ToListAsync();

            // Filter the results based on the provided Step
            var targetWithDesign = targetsWithDesigns.FirstOrDefault(td => td.Design != null && td.Design.WfStep == (int)wfStep);

            if (targetWithDesign != null)
            {
                return targetWithDesign.Design;
            }

            return null;
        }



    }
}
