using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.Data.Models;
using QLiteDataApi.Context;
using Serilog;
using System.Net.Sockets;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    /// <summary>
    /// Provides services related to desk operations, including managing tickets and desk statuses.
    /// </summary>
    public class DeskService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public DeskService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Asynchronously retrieves tickets by state and optionally by desk ID.
        /// </summary>
        /// <param name="state">The state of the tickets to retrieve.</param>
        /// <param name="DeskID">The ID of the desk (optional).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the tickets.</returns>
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


        /// <summary>
        /// Finds the next ticket to be called based on macro rules, desk ID, and branch ID.
        /// </summary>
        /// <param name="branchId">The branch ID.</param>
        /// <param name="macroId">The macro ID.</param>
        /// <param name="DeskId">The desk ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ticket to be called.</returns>
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

        /// <summary>
        /// Finds a ticket based on macro rules defined for a given macro. This method selects a ticketing strategy
        /// (Proportional or Sequential) based on the macro type associated with the macroId.
        /// </summary>
        /// <param name="macroId">The unique identifier of the macro whose rules are to be evaluated.</param>
        /// <param name="branchId">Optional branch ID to further filter the tickets. Can be null.</param>
        /// <param name="deskId">The desk ID used to exclude tickets that are already associated with this desk.</param>
        /// <returns>A Task that, when awaited, returns a Ticket object that matches the macro rules. If no ticket is found, returns null.</returns>


        public async Task<Ticket> FindTicketByMacroAsync(Guid macroId, Guid? branchId, Guid deskId)
        {
            var currentTime = DateTime.Now;
            var oldestPossibleTicketTime = currentTime.AddHours(-8);
            var macroTypeKey = $"Macro_{macroId}";
            if (!_cache.TryGetValue(macroTypeKey, out int? macroType))
            {
                macroType = await _context.Macros
                                .Where(m => m.Gcrecord == null)
                                .Select(m => m.MacroType)
                                .FirstOrDefaultAsync();

                _cache.Set(macroTypeKey, macroType, new MemoryCacheEntryOptions()
                  .SetSize(1)
                  .SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
            }


            if (macroType.HasValue)
            {
                switch (macroType.Value)
                {
                    case (int)MacroType.Proportional:
                        return await ProccessProportional(macroId, branchId, deskId, oldestPossibleTicketTime);
                    case (int)MacroType.Sequential:
                        return await ProcessSequential(macroId, branchId, deskId);
                    default:
                        return null;
                }
            }

            return null; // If no ticket is found or macroType is null
        }

        /// <summary>
        /// Processes tickets based on a proportional strategy defined by macro rules. It attempts to find a ticket
        /// that matches any of the macro rules associated with the provided macroId. Each rule is checked in order,
        /// and the first matching ticket is selected.
        /// </summary>
        /// <param name="macroId">The unique identifier of the macro whose rules are to be applied.</param>
        /// <param name="branchId">Optional branch ID to further filter the tickets. Can be null.</param>
        /// <param name="deskId">The desk ID used to exclude tickets already associated with this desk.</param>
        /// <param name="oldestPossibleTicketTime">The oldest possible time a ticket can have been created and still be considered for selection.</param>
        /// <returns>A Task that, when awaited, returns the first Ticket object matching the rules, or null if no matching ticket is found.</returns>
        private async Task<Ticket> ProccessProportional(Guid macroId, Guid? branchId, Guid deskId, DateTime oldestPossibleTicketTime)
        {
            var macroRulesCacheKey = $"MacroRules_{macroId}";


            List<MacroRuleDto> macroRules = await GetMacroRulesAsync(macroId, macroRulesCacheKey);

            for (int i = 0; i < macroRules.Count; i++)
            {
                var macroRule = macroRules[i];
                var cacheKey = $"MacroCallCount_{macroRule.Oid}";

                if (!_cache.TryGetValue(cacheKey, out int callCount))
                {
                    callCount = 0; // Initialize if not present
                }

                if (callCount < macroRule.NumberOfTickets)
                {
                    var result = await FindTicketForRuleAsync(macroRule, branchId, deskId, oldestPossibleTicketTime);
                    if (result != null)
                    {
                        callCount++;
                        _cache.Set(cacheKey, callCount, new MemoryCacheEntryOptions()
                          .SetSize(1)
                          .SetAbsoluteExpiration(TimeSpan.FromHours(8)));

                        if (callCount >= macroRule.NumberOfTickets)
                        {
                            macroRules[i].Reset = true;
                            _cache.Set(macroRulesCacheKey, macroRules, new MemoryCacheEntryOptions()
                                    .SetSize(1)
                                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                        }
                        return result; // Return the found ticket
                    }
                }
            }

            // Reset call counts only after completing a full loop (round) without finding tickets

            for (int i = 0; i < macroRules.Count; i++)
            {
                var macroRule = macroRules[i];

                var cacheKey = $"MacroCallCount_{macroRule.Oid}";

                if (macroRule.Reset)
                {
                    var callCount = 0;

                    macroRules[i].Reset = false;
                    _cache.Set(macroRulesCacheKey, macroRules, new MemoryCacheEntryOptions()
                                   .SetSize(1)
                                   .SetAbsoluteExpiration(TimeSpan.FromHours(1)));


                    var result = await FindTicketForRuleAsync(macroRule, branchId, deskId, oldestPossibleTicketTime);
                    if (result != null)
                    {
                        callCount++; // Increment call count
                        _cache.Set(cacheKey, callCount, new MemoryCacheEntryOptions()
                          .SetSize(1)
                          .SetAbsoluteExpiration(TimeSpan.FromHours(8)));

                        if (callCount >= macroRule.NumberOfTickets)
                        {
                            macroRules[i].Reset = true;
                            _cache.Set(macroRulesCacheKey, macroRules, new MemoryCacheEntryOptions()
                                    .SetSize(1)
                                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                        }
                        return result; // Return the found ticket
                    }
                    else
                    {
                        _cache.Set(cacheKey, 0, new MemoryCacheEntryOptions()
                                                 .SetSize(1)
                                                 .SetAbsoluteExpiration(TimeSpan.FromHours(8)));
                    }
                }

            }

            return null;
        }


        /// <summary>
        /// Retrieves a list of macro rules for a given macroId from the cache. If the rules are not present in the cache,
        /// it fetches them from the database, orders them by sequence, and caches the result.
        /// </summary>
        /// <param name="macroId">The unique identifier of the macro for which rules are to be retrieved.</param>
        /// <param name="macroRulesCacheKey">The cache key used to store and retrieve the macro rules.</param>
        /// <returns>A Task that, when awaited, returns a list of MacroRuleDto objects. Returns null if no rules are found.</returns>

        private async Task<List<MacroRuleDto>> GetMacroRulesAsync(Guid macroId, string macroRulesCacheKey)
        {
            if (!_cache.TryGetValue(macroRulesCacheKey, out List<MacroRuleDto> macroRules))
            {
                macroRules = await _context.MacroRules
                    .Where(mr => mr.Macro == macroId && mr.Gcrecord == null)
                    .OrderBy(mr => mr.Sequence)
                    .Select(mr => new MacroRuleDto
                    {
                        Oid = mr.Oid,
                        NumberOfTickets = mr.NumberOfTickets,
                        Sequence = mr.Sequence,
                        ServiceType = mr.ServiceType,
                        Segment = mr.Segment
                    }).ToListAsync();

                if (macroRules.Count == 0)
                {
                    Log.Error("No Macro Rule Found");
                    return null;
                }

                _cache.Set(macroRulesCacheKey, macroRules, new MemoryCacheEntryOptions()
                  .SetSize(1)
                  .SetAbsoluteExpiration(TimeSpan.FromHours(1)));
            }
            return macroRules;

        }


        /// <summary>
        /// Attempts to find a single ticket that matches a specific macro rule. The method considers various conditions
        /// like service type, segment, branch, and ticket state to filter tickets. It also ensures that the ticket has not been
        /// processed for the specified desk and was created within a permissible time frame.
        /// </summary>
        /// <param name="macroRule">The macro rule to match tickets against.</param>
        /// <param name="branchId">Optional branch ID to further filter the tickets. Can be null.</param>
        /// <param name="deskId">The desk ID used to exclude tickets already associated with this desk.</param>
        /// <param name="oldestPossibleTicketTime">The oldest possible time a ticket can have been created and still be considered.</param>
        /// <returns>A Task that, when awaited, returns a Ticket object matching the rule, or null if no match is found.</returns>

        private async Task<Ticket> FindTicketForRuleAsync(MacroRuleDto macroRule, Guid? branchId, Guid deskId, DateTime oldestPossibleTicketTime)
        {
            // Consolidate the query logic here to fetch the first ticket that matches the macro rule
            var query = from t in _context.Tickets
                        join ts in _context.TicketStates on t.Oid equals ts.Ticket
                        where (t.ServiceType == macroRule.ServiceType) && (t.Segment == macroRule.Segment) &&
                              (t.Branch == branchId) &&
                              (t.CurrentState == (int)TicketStateEnum.Waiting || t.CurrentState == (int)TicketStateEnum.Waiting_T) &&
                              (t.Desk != deskId || t.Desk == null) &&
                              t.CreatedDate >= oldestPossibleTicketTime
                        orderby t.LastOprTime
                        select new
                        {
                            Ticket = new Ticket
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
                                TicketPool = t.TicketPool
                            }
                        };

            var result = await query.FirstOrDefaultAsync();
            return result?.Ticket;
        }


        /// <summary>
        /// Processes tickets based on a sequential strategy defined by macro rules. This method sequentially goes through each macro rule
        /// associated with the macroId and attempts to find a matching ticket that adheres to the rule's conditions, including service type,
        /// segment, and time constraints.
        /// </summary>
        /// <param name="macroId">The unique identifier of the macro whose rules are to be applied.</param>
        /// <param name="branchId">Optional branch ID to further filter the tickets. Can be null.</param>
        /// <param name="deskId">The desk ID used to exclude tickets already associated with this desk.</param>
        /// <returns>A Task that, when awaited, returns a Ticket object if a matching ticket is found, or null otherwise.</returns>
        public async Task<Ticket> ProcessSequential(Guid macroId, Guid? branchId, Guid deskId)
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

        /// <summary>
        /// Calls a ticket for service at a desk. Ends the current service if any and updates the ticket and desk states.
        /// </summary>
        /// <param name="callTicketDto">Data transfer object containing call ticket information.</param>
        /// <param name="autocall">Indicates whether the call is automatic.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the new ticket state.</returns>
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
                t.Year = DateTime.Today.Year;
                t.DayOfYear= DateTime.Today.DayOfYear;
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

                await _context.SaveChangesAsync();

            }

            return svc;
        }

        /// <summary>
        /// Ends the current service for a desk and updates the ticket state to final.
        /// </summary>
        /// <param name="DeskID">The desk ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the final ticket state.</returns>
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
                final = fin;
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


        /// <summary>
        /// Retrieves the current service for a desk.
        /// </summary>
        /// <param name="DeskID">The desk ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the current ticket state.</returns>
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

        /// <summary>
        /// Retrieves a specific ticket based on its ID.
        /// </summary>
        /// <param name="ticketId">The ID of the ticket to retrieve.</param>
        /// <returns>The ticket if found; otherwise, null.</returns>
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

        /// <summary>
        /// Parks a ticket, updating its state and associated desk status.
        /// </summary>
        /// <param name="parkTicketDto">The DTO containing information about the ticket to park.</param>
        /// <returns>The new ticket state after parking.</returns>
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

        /// <summary>
        /// Transfers a ticket from one desk to another, optionally changing its service type.
        /// </summary>
        /// <param name="transferTicket">The DTO containing information for the transfer operation.</param>
        /// <returns>The new ticket state after the transfer.</returns>
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

        /// <summary>
        /// Retrieves a specific desk by its ID.
        /// </summary>
        /// <param name="DeskID">The ID of the desk to retrieve.</param>
        /// <returns>The desk if found; otherwise, null.</returns>
        public async Task<Desk> GetDeskAsync(Guid DeskID)
        {
            var desk = await _context.Desks.FirstOrDefaultAsync(d => d.Oid == DeskID && d.Gcrecord == null);
            return desk;
        }

        /// <summary>
        /// Retrieves the list of macros available for a specific desk.
        /// </summary>
        /// <param name="DeskID">The ID of the desk.</param>
        /// <returns>A list of macros associated with the desk.</returns>
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

        /// <summary>
        /// Retrieves a list of all desks.
        /// </summary>
        /// <returns>A list of desks.</returns>
        public async Task<List<Desk>> GetDeskListAsync()
        {
            var desks = await _context.Desks.Where(d => d.Gcrecord == null).ToListAsync();
            return desks;
        }

        /// <summary>
        /// Retrieves the list of service types that can be transferred to by a specific desk.
        /// </summary>
        /// <param name="DeskID">The ID of the desk.</param>
        /// <returns>A list of transferable service types.</returns>
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

        /// <summary>
        /// Retrieves the list of service types that can be created by a specific desk.
        /// </summary>
        /// <param name="DeskID">The ID of the desk.</param>
        /// <returns>A list of creatable service types.</returns>
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

        /// <summary>
        /// Retrieves the average duration of service for a specific ticket.
        /// </summary>
        /// <param name="ticketid">The ID of the ticket.</param>
        /// <returns>The average duration of service for the ticket.</returns>
        public async Task<int> GetTicketDurationAsync(Guid ticketid)
        {
            int duration = 1;

            return duration;
        }


        /// <summary>
        /// Retrieves a list of all service types.
        /// </summary>
        /// <returns>A list of service types.</returns>
        public async Task<List<ServiceType>> GetServiceList()
        {

            var services = await _context.ServiceTypes.Where(s => s.Gcrecord == null).ToListAsync();

            return services;
        }

        /// <summary>
        /// Retrieves a list of all segments.
        /// </summary>
        /// <returns>A list of segments.</returns>
        public async Task<List<Segment>> GetSegmentList()
        {

            var segments = await _context.Segments.Where(s => s.Gcrecord == null).ToListAsync();

            return segments;
        }

        /// <summary>
        /// Sets the busy status for a desk, optionally ending the current service.
        /// </summary>
        /// <param name="DeskID">The ID of the desk.</param>
        /// <param name="Status">The new activity status for the desk.</param>
        /// <returns>The display number of the desk if successful; otherwise, null.</returns>
        public async Task<string> SetBusyStatus(Guid DeskID, DeskActivityStatus Status)
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
                desk.LastStateTime = currentTime;

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

        /// <summary>
        /// Retrieves the last status of a specific desk.
        /// </summary>
        /// <param name="deskId">The ID of the desk.</param>
        /// <returns>The last desk status.</returns>
        public async Task<DeskStatus> GetLastDeskStatus(Guid deskId)
        {
            var query = await _context.DeskStatuses
                .Where(ds => ds.Oid == deskId && ds.StateEndTime == null)
                .OrderByDescending(ds => ds.StateStartTime)
                .FirstOrDefaultAsync();

            return query;
        }


        /// <summary>
        /// Retrieves a list of all states for a specific ticket.
        /// </summary>
        /// <param name="ticketID">The ID of the ticket.</param>
        /// <returns>An object containing the list of ticket states.</returns>
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
