using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.Data.Models;
using QLiteDataApi.Context;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    public interface IDeskService
    {
        Task<object> GetTicketsByStateAsync(TicketStateEnum state, Guid DeskID = default);
        Task<TicketState> CallTicketAsync(CallTicketDto callTicketDto, bool autocall = false);
        Task<TicketState> GetMyCurrentServiceAsync(Guid DeskID);
        Task<TicketState> EndCurrentServiceAsync(Guid DeskID);
        Task<int> GetTicketDurationAsync(Guid ticketid);
        Task<TicketState> TransferOperationAsync(TransferTicketDto transferTicketDto);
        Task<TicketState> ParkOperationAsync(ParkTicketDto parkTicketDto);
        Task<Desk> GetDeskAsync(Guid DeskID);
        Task<List<DeskMacroSchedule>> GetMacrosAsync(Guid DeskID);
        Task<List<Desk>> GetDeskListAsync();
        Task<List<DeskTransferableService>> GetTransferableServiceListAsync(Guid DeskID);
        Task<List<DeskCreatableService>> GetCreatableServiceListAsync(Guid DeskID);

        Task<List<ServiceType>> GetServiceList();

        Task<List<Segment>> GetSegmentList();

        Task<string> SetBusyStatus(Guid Desk,DeskActivityStatus Status);

        Task<object> GetTicketStateListAsync(Guid TicketID);

    }


    public class DeskService : IDeskService
    {

        private readonly ApplicationDbContext _context;

        public DeskService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<object> GetTicketsByStateAsync(TicketStateEnum state, Guid DeskID = default)
        {
            IQueryable<Ticket> query = _context.Tickets
                .Where(t => t.CurrentState == (int)state && t.ModifiedDate >= DateTime.UtcNow.AddMinutes(-480));

            if (DeskID != Guid.Empty)
            {
                if ((int)state == 1)
                    query = query.Where(t => t.ToDesk == DeskID);
                else
                    query = query.Where(t => t.Desk == DeskID);
            }

            var waitingTickets = await query.Join(
                _context.TicketPools,
                t => t.TicketPool,
                tp => tp.Oid,
                (t, tp) => new
                {
                    TicketNumber = tp.ServiceCode + t.Number,
                    Service = t.ServiceTypeName,
                    Segment = t.SegmentName,
                    Oid = t.Oid,
                }).ToListAsync();

            // Get the total count asynchronously
            var recordsTotal = await query.CountAsync();

            // Combine the results into JSON data
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = waitingTickets };

            return jsonData;
        }

        private async Task<Ticket> FindTicketToCallAsync(Guid? branchId, Guid macroId, Guid DeskId)
        {
            Macro macro = new Macro();
            if (macroId != Guid.Empty)
                macro = await _context.Macros.FirstAsync(m => m.Oid == macroId && m.Gcrecord == null);
            //TODO:if Macro is null find Macro or force desk user to choose macro before showing the view 

            Ticket t = new Ticket();
            if (macro != null)
            {
                t = await FindTicketByMacroAsync(macroId, branchId, DeskId);
                if (t != null)
                {
                    //t.TicketState.Macro = macro?.Oid;
                    //t.TicketState.MacroObject = macro;

                    var ticketPool = await _context.TicketPools.FirstOrDefaultAsync(tp => tp.Oid == t.TicketPool);
                    if (ticketPool != null)
                    {
                        t.ServiceCode = ticketPool.ServiceCode;
                    }
                }
            }
            //TODO:find ticket anyway even if no macro found ???????

            return t;
        }

        public async Task<Ticket> FindTicketByMacroAsync(Guid macroId, Guid? branchId, Guid deskId)
        {
            var currentTime = DateTime.Now;
            var oldestPossibleTicketTime = currentTime.AddHours(-8);

            var query = from mr in _context.MacroRules
                        join t in _context.Tickets on new { mr.ServiceType, mr.Segment } equals new { t.ServiceType, t.Segment } into tickets
                        from ticket in tickets.DefaultIfEmpty()
                        join ts in _context.TicketStates on ticket.Oid equals ts.Ticket
                        where mr.Macro == macroId &&
                              (ticket.Branch == branchId) &&
                              (ticket.CurrentState == (int)TicketStateEnum.Waiting || ticket.CurrentState == (int)TicketStateEnum.Waiting_T) &&
                              (ticket.Desk != deskId || ticket.Desk == null) &&
                              (mr.Transfer != true || ticket.CurrentState == (int)TicketStateEnum.Waiting_T) &&
                              (mr.ToThisDesk == (int)MeNotothersAll.AllTerminals ||
                              (mr.ToThisDesk == (int)MeNotothersAll.OnlyForSelectedDesk && ticket.ToDesk == deskId) ||
                              (mr.ToThisDesk == (int)MeNotothersAll.NotSpecified && (ticket.ToDesk == deskId || ticket.ToDesk == null))) &&
                              ticket.CreatedDate >= oldestPossibleTicketTime &&
                              mr.Gcrecord == null
                        orderby mr.Sequence, ticket.LastOprTime
                        select new
                        {
                            Ticket = new Ticket
                            {
                                Oid = ticket.Oid,
                                CreatedBy = ticket.CreatedBy,
                                ModifiedBy = ticket.ModifiedBy,
                                CreatedDate = ticket.CreatedDate,
                                CreatedDateUtc = ticket.CreatedDateUtc,
                                ModifiedDate = ticket.ModifiedDate,
                                ModifiedDateUtc = ticket.ModifiedDateUtc,
                                CurrentDesk = ticket.Desk,
                                CurrentState = (int)TicketStateEnum.Service,
                                LastOpr = (int)TicketOprEnum.Call,
                                TicketState = ts,
                                Number = ticket.Number,
                                Branch = ticket.Branch,
                                Segment = ticket.Segment,
                                SegmentName = ticket.SegmentName,
                                ServiceType = ticket.ServiceType,
                                ServiceTypeName = ticket.ServiceTypeName,
                                TicketPool = ticket.TicketPool
                            }
                        };

            var result = await query.FirstOrDefaultAsync(); // Execute the query asynchronously

            if (result != null)
            {
                // Map properties to Ticket entity
                var mappedTicket = result.Ticket;

                return mappedTicket;
            }

            // Handle case when no matching records found
            return null;
        }

        public async Task<TicketState> CallTicketAsync(CallTicketDto callTicketDto, bool autocall = false)
        {
            Desk? d = await _context.Desks.FindAsync(callTicketDto.DeskID);
            await EndCurrentServiceAsync(callTicketDto.DeskID);

            d.CurrentTicketNumber = null;
            d.LastStateTime = DateTime.Now;

            Ticket t;
            TicketState svc = null;
            if (callTicketDto.TicketID == Guid.Empty)
                t = await FindTicketToCallAsync(d.Branch, callTicketDto.MacroID, callTicketDto.DeskID);
            else
                t = await GetTicketAsync(callTicketDto.TicketID);

            if (t != null)
            {
                d.CurrentTicketNumber = t.Number;
                d.LastStateTime = DateTime.Now;
                _context.Desks.Update(d);

                DateTime tm = DateTime.Now;
                t.CurrentDesk = callTicketDto.DeskID;
                t.CurrentState = (int)TicketStateEnum.Service;
                t.LastOpr = (int)TicketOprEnum.Call;
                t.LastOprTime = tm;
                t.Desk = callTicketDto.DeskID;
                _context.Tickets.Update(t);

                TicketState currentState = t.TicketState;

                currentState.Desk = callTicketDto.DeskID;
                currentState.User = callTicketDto.User;
                currentState.EndTime = tm;
                currentState.ModifiedDate = DateTime.Now;
                currentState.ModifiedDateUtc = DateTime.UtcNow;
                _context.TicketStates.Update(currentState);

                svc = new()
                {
                    TicketStateValue = (int)TicketStateEnum.Service,
                    Oid = Guid.NewGuid(),
                    Ticket = t.Oid,
                    Desk = callTicketDto.DeskID,
                    User = callTicketDto.User,
                    TicketNumber = t.Number,
                    Branch = t.Branch,
                    Segment = t.Segment,
                    SegmentName = t.SegmentName,
                    ServiceType = t.ServiceType,
                    ServiceTypeName = t.ServiceTypeName,
                    StartTime = tm,
                    ServiceCode = t.ServiceCode,
                    CreatedDate = DateTime.Now,
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDate = DateTime.Now,
                    ModifiedDateUtc = DateTime.UtcNow,
                    DisplayNo = d.DisplayNo,
                    CallingRuleDescription = t.CallingRuleDescription,
                    Macro = callTicketDto.MacroID,
                    TicketCallType = (int?)(autocall ? TicketCallType.Autocall : callTicketDto.TicketID == Guid.Empty ? TicketCallType.Definitive : TicketCallType.Normal)
                };

                if (autocall)
                    svc.CallingRuleDescription += " (autocall)";

                _context.TicketStates.Add(svc);
                svc.TicketNavigation = t;

                await _context.SaveChangesAsync();
            }

            return svc;
        }
        public async Task<TicketState> EndCurrentServiceAsync(Guid DeskID)
        {
            TicketState current = await GetMyCurrentServiceAsync(DeskID);

            TicketState final = new TicketState();
            if (current != null)
            {
                DateTime t = DateTime.Now;
                current.EndTime = t;
                current.TicketOprValue = (int)TicketOprEnum.ServiceEnd;
                current.EndTime = DateTime.Now;
                // Update the TicketState entity
                _context.TicketStates.Update(current);

                Ticket currentTicket = current.TicketNavigation;

                if (currentTicket != null)
                {
                    currentTicket.CurrentDesk = null;
                    currentTicket.LastOprTime = t;
                    currentTicket.LastOpr = (int)TicketOprEnum.ServiceEnd;
                    currentTicket.CurrentState = (int)TicketStateEnum.Final;
                    currentTicket.ModifiedDate = DateTime.Now;
                    currentTicket.ModifiedDateUtc = DateTime.UtcNow;
                    currentTicket.Desk = DeskID;
                    

                    // Update the Ticket entity
                    _context.Tickets.Update(currentTicket);
                }
                else
                {
                    // Handle case when currentTicket is null
                }

                // Insert a new TicketState entity
                TicketState fin = new TicketState
                {
                    Oid = Guid.NewGuid(),
                    Desk = DeskID,
                    Segment = currentTicket.Segment,
                    SegmentName = currentTicket.SegmentName,
                    ServiceType = currentTicket.ToServiceType == Guid.Empty ? current.ServiceType : currentTicket.ToServiceType,
                    ServiceTypeName = string.IsNullOrEmpty(currentTicket.ServiceTypeName) ? current.ServiceTypeName : currentTicket.ServiceTypeName,
                    Ticket = currentTicket.Oid,
                    TicketNumber = currentTicket.Number,
                    TicketStateValue = (int)TicketStateEnum.Final,
                    StartTime = t,
                    ServiceCode = currentTicket.ServiceCode,
                    CreatedDate = DateTime.Now,
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDate = DateTime.Now,
                    ModifiedDateUtc = DateTime.UtcNow,


                    // User = QorchUserContext.UserId,

                    // Macro = currentTicket.Macro?.Macro,
                    //ServiceCode = currentTicket.ServiceCode,


                };
                final =fin;
                _context.TicketStates.Add(fin);

            }
            else
            {
                // Handle case when current is null
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return final;
        }

        public async Task<TicketState> GetMyCurrentServiceAsync(Guid DeskID)
        {
            var current = await (from tOpr in _context.TicketStates
                                 join t in _context.Tickets on tOpr.Ticket equals t.Oid
                                 join tp in _context.TicketPools on t.TicketPool equals tp.Oid into poolJoin
                                 from tp in poolJoin.DefaultIfEmpty()
                                 where t.CurrentState == 2 && tOpr.Desk == DeskID && t.Desk == DeskID
                                     && tOpr.TicketStateValue == (int)TicketStateEnum.Service
                                     && tOpr.TicketOprValue == null
                                 select new TicketState
                                 {
                                     Oid = tOpr.Oid,
                                     CreatedBy = tOpr.CreatedBy,
                                     ModifiedBy = tOpr.ModifiedBy,
                                     CreatedDate = tOpr.CreatedDate,
                                     CreatedDateUtc = tOpr.CreatedDateUtc,
                                     ModifiedDate = tOpr.ModifiedDate,
                                     ModifiedDateUtc = tOpr.ModifiedDateUtc,
                                     Desk = tOpr.Desk,
                                     User = tOpr.User,
                                     Ticket = tOpr.Ticket,
                                     Branch = tOpr.Branch,
                                     TicketNumber = tOpr.TicketNumber,
                                     TicketStateValue = tOpr.TicketStateValue,
                                     TicketOprValue = tOpr.TicketOprValue,
                                     StartTime = tOpr.StartTime,
                                     EndTime = tOpr.EndTime,
                                     ServiceType = tOpr.ServiceType,
                                     Segment = tOpr.Segment,
                                     ServiceTypeName = tOpr.ServiceTypeName,
                                     SegmentName = tOpr.SegmentName,
                                     Macro = tOpr.Macro,
                                     CallingRuleDescription = tOpr.CallingRuleDescription,
                                     DeskAppType = tOpr.DeskAppType,
                                     TicketCallType = tOpr.TicketCallType,
                                     OptimisticLockField = tOpr.OptimisticLockField,
                                     Gcrecord = tOpr.Gcrecord,
                                     KioskAppId = tOpr.KioskAppId,
                                     TicketNavigation = t,
                                     ServiceCode = tp.ServiceCode,
                                     Note = t.TicketNote
                                 })
                                .FirstOrDefaultAsync();

            return current;
        }

        public async Task<Ticket> GetTicketAsync(Guid ticketId)
        {
            var ticket = await (from t in _context.Tickets
                                join ts in _context.TicketStates on t.Oid equals ts.Ticket into ticketStates
                                from ts in ticketStates.Where(ts => ts.TicketOprValue == null).DefaultIfEmpty()
                                join tp in _context.TicketPools on t.TicketPool equals tp.Oid into ticketPools
                                from tp in ticketPools.Where(tp => tp.Gcrecord == null).DefaultIfEmpty()
                                where t.Oid == ticketId
                                select new Ticket
                                {
                                    Oid = t.Oid,
                                    CreatedBy = t.CreatedBy,
                                    ModifiedBy = t.ModifiedBy,
                                    CreatedDate = t.CreatedDate,
                                    CreatedDateUtc = t.CreatedDateUtc,
                                    ModifiedDate = t.ModifiedDate,
                                    ModifiedDateUtc = t.ModifiedDateUtc,
                                    CurrentDesk = t.Desk,
                                    CurrentState = (int)TicketStateEnum.Service,
                                    LastOpr = (int)TicketOprEnum.Call,
                                    TicketState = ts,
                                    Number = t.Number,
                                    Branch = t.Branch,
                                    Segment = t.Segment,
                                    SegmentName = t.SegmentName,
                                    ServiceType = t.ServiceType,
                                    ServiceTypeName = t.ServiceTypeName,
                                    ServiceCode = tp.ServiceCode,
                                    TicketPool = tp.Oid
                                }).FirstOrDefaultAsync();

            return ticket;
        }
      

        public async Task<TicketState> ParkOperationAsync(ParkTicketDto parkTicketDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                Ticket parkTicket = await GetTicketAsync(parkTicketDto.TicketId);

                if (parkTicket.CurrentState != (int)Enums.TicketStateEnum.Service)
                    throw new Exception("Park Operation is not allowed for this ticket");

                DateTime currentTime = DateTime.Now;

                //TODO: Something is wrong here svc is not changed
                TicketState svc = parkTicket.TicketState;

                svc.EndTime = currentTime;
                svc.TicketOprValue = (int)TicketOprEnum.Park;
                _context.TicketStates.Update(svc);

                parkTicket.CurrentState = (int)TicketStateEnum.Park;
                parkTicket.LastOpr = (int)TicketOprEnum.Park;
                parkTicket.LastOprTime = currentTime;
                parkTicket.TicketNote = parkTicketDto.TicketNote;
                parkTicket.Desk = parkTicketDto.DeskID;

                _context.Tickets.Update(parkTicket);

                TicketState parkTicketOpr = new TicketState
                {
                    //User = QorchUserContext.UserId,
                    Desk = parkTicketDto.DeskID,

                    Oid = Guid.NewGuid(),
                    Ticket = parkTicket.Oid,
                    TicketStateValue = (int)TicketStateEnum.Park,
                    StartTime = currentTime,
                    ServiceType = parkTicket.ToServiceType == Guid.Empty ? svc.ServiceType : parkTicket.ToServiceType,
                    ServiceTypeName = string.IsNullOrEmpty(parkTicket.ServiceTypeName) ? svc.ServiceTypeName : parkTicket.ServiceTypeName,
                    Segment = parkTicket.Segment,
                    SegmentName = parkTicket.SegmentName,
                    //Branch = QorchUserContext.BranchId.Value,
                    //Macro = parkTicket.StateObj.Macro,
                    //MacroObj = parkTicket.StateObj.MacroObj,
                    TicketNumber = parkTicket.Number,
                    ServiceCode = parkTicket.ServiceCode,
                    CreatedDate = DateTime.Now,
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDate = DateTime.Now,
                    ModifiedDateUtc = DateTime.UtcNow
                };
                _context.TicketStates.Add(parkTicketOpr);
                parkTicketOpr.TicketNavigation = parkTicket;
                Desk desk = _context.Desks.SingleOrDefault(d => d.Oid == parkTicketDto.DeskID);

                if (desk != null)
                {
                    desk.CurrentTicketNumber = null;
                    desk.LastStateTime = DateTime.Now;
                    _context.Desks.Update(desk);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return parkTicketOpr;
            }
        }

        public async Task<TicketState> TransferOperationAsync(TransferTicketDto transferTicket)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                Desk fromDesk = await _context.Desks
                                              .Where(d => d.Oid == transferTicket.TransferFromDesk)
                                              .FirstAsync();

                if (fromDesk == null)
                {
                    // Handle case where the desk is not found
                    throw new Exception("Source desk not found");
                }

                Ticket ticket = await GetTicketAsync(transferTicket.TicketId);

                if (ticket == null)
                {
                    // Handle case where the ticket is not found
                    throw new Exception("Ticket not found");
                }


                DateTime tNow = DateTime.Now;
                TicketState current = ticket.TicketState;
                current.EndTime = tNow;
                current.TicketOprValue = (int)TicketOprEnum.Transfer;

                fromDesk.CurrentTicketNumber = null;
                fromDesk.LastStateTime = DateTime.Now;

                Guid? toDesk = transferTicket.TransferToDesk;
                Guid? toService = transferTicket.TransferServiceType;

                ticket.Desk = transferTicket.TransferFromDesk;

                ServiceType service = await _context.ServiceTypes.FirstOrDefaultAsync(sv => sv.Oid == toService);

                TicketState transferState = new TicketState
                {
                    Oid = Guid.NewGuid(),
                    Ticket = ticket.Oid,
                    TicketStateValue = (int)TicketStateEnum.Waiting_T,
                    StartTime = tNow,
                    Segment = ticket.Segment,
                    SegmentName = ticket.SegmentName,
                    Branch = fromDesk.Branch, // Assuming both desks have the same branch
                    Macro = ticket.TicketState?.Macro,
                    MacroObject = ticket.TicketState?.MacroObject,
                    TicketNumber = ticket.Number,
                    ServiceType = toService,
                    ServiceCode = ticket.ServiceCode,
                    Desk = toDesk,
                    ServiceTypeName = service?.Name
                };

                ticket.ToServiceType = toService;

                ticket.ToDesk = toDesk;
                ticket.LastOpr = (int)TicketOprEnum.Transfer;
                ticket.LastOprTime = tNow;
                ticket.CurrentState = (int)TicketStateEnum.Waiting_T;
                ticket.CurrentDesk = null;
                ticket.TicketNote = transferTicket.TicketNote;
                ticket.ServiceTypeName = service?.Name;

                // Add and update entities
                _context.Desks.Update(fromDesk);
                _context.TicketStates.Update(current);
                _context.TicketStates.Add(transferState);
                _context.Tickets.Update(ticket);

                // Setting up relationships
                transferState.TicketNavigation = ticket;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return transferState;
            }
        }

        public async Task<Desk> GetDeskAsync(Guid DeskID)
        {
            var desk = await _context.Desks.FirstOrDefaultAsync(d => d.Oid == DeskID && d.Gcrecord == null);
            return desk;
        }

        public async Task<List<DeskMacroSchedule>> GetMacrosAsync(Guid DeskID)
        {
            var macrosWithMacroNames = await _context.DeskMacroSchedules
                .Where(dms => dms.Desk == DeskID && dms.Gcrecord == null)
                .Join(
                    _context.Macros,
                    dms => dms.Macro,
                    macro => macro.Oid,
                    (dms, macro) => new { DeskMacroSchedule = dms, MacroName = macro.Name }
                )
                .ToListAsync();

            foreach (var item in macrosWithMacroNames)
            {
                item.DeskMacroSchedule.MacroName = item.MacroName;
            }

            var macros = macrosWithMacroNames.Select(item => item.DeskMacroSchedule).ToList();
            return macros;
        }

        public async Task<List<Desk>> GetDeskListAsync()
        {
            var desks = await _context.Desks.Where(d => d.Gcrecord == null).ToListAsync();
            return desks;
        }

        public async Task<List<DeskTransferableService>> GetTransferableServiceListAsync(Guid DeskID)
        {
            var transferableServices = await _context.DeskTransferableServices
                .Where(dts => dts.Gcrecord == null && dts.Desk == DeskID)
                .Select(dts => new DeskTransferableService
                {
                  
                    ServiceType = dts.ServiceType,
                    ServiceTypeNavigation = new ServiceType
                    {
                        Name = dts.ServiceTypeNavigation.Name
                    }
                })
                .ToListAsync();

            return transferableServices;
        }

        public async Task<List<DeskCreatableService>> GetCreatableServiceListAsync(Guid DeskID)
        {
            var createableServices = await _context.DeskCreatableServices
                .Where(dts => dts.Gcrecord == null && dts.Desk == DeskID)
                .Select(dts => new DeskCreatableService
                {
                   
                    ServiceType = dts.ServiceType,
                    ServiceTypeNavigation = new ServiceType
                    {
                        Name = dts.ServiceTypeNavigation.Name
                    }
                })
                .ToListAsync();

            return createableServices;
        }
        public async Task<int> GetTicketDurationAsync(Guid ticketid)
        {
            int duration = 1;

            return duration;
        }

        public async Task<List<ServiceType>> GetServiceList( )
        {

            var services = await  _context.ServiceTypes.Where(s=> s.Gcrecord == null).ToListAsync();

            return services;
        }

        public async Task<List<Segment>> GetSegmentList( )
        {

            var segments = await _context.Segments.Where(s => s.Gcrecord == null).ToListAsync();

            return segments;
        }

        public async Task<string> SetBusyStatus(Guid DeskID,DeskActivityStatus Status)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get current time
                var currentTime = DateTime.Now;

                // If the desk status is Paused, Closed, or Busy, end the current service
                if (Status == DeskActivityStatus.Paused || Status == DeskActivityStatus.Closed || Status == DeskActivityStatus.Busy)
                {
                    await EndCurrentServiceAsync(DeskID);
                }

                // Create a new DeskStatus entity
                var deskStatus = new DeskStatus
                {
                    Oid = Guid.NewGuid(),
                   // User = user,
                    Desk = DeskID,
                    DeskActivityStatus = (int)Status,
                    StateStartTime = currentTime
                };

                // Update the end time of the last desk status
                var deskLatestStatus = await GetLastDeskStatus(DeskID);
                if (deskLatestStatus != null)
                {
                    deskLatestStatus.StateEndTime = currentTime;
                    _context.Update(deskLatestStatus);
                }

                // Update the activity status of the desk
                var desk = await _context.Desks.SingleAsync(d => d.Oid == DeskID);

                desk.ActivityStatus = (int)Status;
                desk.LastStateTime= currentTime;

                _context.Update(desk);

                // Insert the new desk status
                _context.DeskStatuses.Add(deskStatus);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return desk.DisplayNo;
            }
            catch (Exception ex)
            {
                // Rollback transaction if an exception occurs
                await transaction.RollbackAsync();

                return null;
            }
           

        }
        public async Task<DeskStatus> GetLastDeskStatus(Guid deskId)
        {
            var query = await _context.DeskStatuses
                .Where(ds => ds.Oid == deskId && ds.StateEndTime == null)
                .OrderByDescending(ds => ds.StateStartTime)
                .FirstOrDefaultAsync();

            return query;
        }

        public async Task<object> GetTicketStateListAsync(Guid ticketID)
        {
            // Query the database to find the ticket with the given ID
            var ticket = await _context.Tickets
                                        .Include(t => t.TicketStates) // Include the TicketStates collection
                                            .ThenInclude(ts => ts.DeskNavigation) // Include DeskNavigation within TicketStates
                                        .FirstOrDefaultAsync(ts => ts.Oid == ticketID);

            if (ticket != null)
            {
                // Return the TicketStates collection of the found ticket
                var ticketStates = ticket.TicketStates
                                        .Select(ts => new 
                                        {
                                            Desk = ts.DeskNavigation != null ? ts.DeskNavigation.Name : null, 
                                            CallType = ts.TicketCallType.ToString(),
                                            StartTime = ts.StartTime.ToString(), 
                                            EndTime = ts.EndTime.ToString(), 

                                            Note = ts.Note,
                                        })
                                        .ToList();

                var recordsTotal = ticketStates.Count();

                var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = ticketStates };

                return jsonData;
            }
            else
            {
                // If no ticket is found, return an empty list
                return new { };
            }
        }


    }
}
