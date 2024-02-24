using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLiteDataApi.Context;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    public interface IDeskService
    {
        object GetTicketsByState(TicketStateEnum state);
        TicketState CallTicket(Guid DeskID, Guid ticketID, Guid user, bool autocall = false);
        TicketState GetMyCurrentService(Guid DeskID);
        TicketState EndCurrentService(Guid DeskID);

    }

    public class DeskService : IDeskService
    {

        private readonly ApplicationDbContext _context;

        public DeskService(ApplicationDbContext context)
        {
            _context = context;
        }


        public object GetTicketsByState(TicketStateEnum state)
        {

            var query = from t in _context.Tickets
                        where t.CurrentState == (int)state
                        select new
                        {
                            TicketNumber = t.Number,
                            Service = t.ServiceTypeName,
                            Segment = t.SegmentName,
                            Oid = t.Oid
                        };

            var waitingTickets = query.ToList();

            var recordsTotal = waitingTickets.Count;
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = waitingTickets };

            return jsonData;
        }


        public TicketState CallTicket(Guid DeskID, Guid ticketID, Guid user, bool autocall = false)
        {

            Desk? d = _context.Desks.Find(DeskID);
            EndCurrentService(DeskID);

            d.CurrentTicketNumber = null;
            d.LastStateTime = DateTime.Now;

            Ticket t;
            TicketState svc = null;
            if (ticketID == null)
                //t = FindTicketToCall(branchId, macroId);

                t = null;
            else
                t = GetTicket(ticketID);

            if (t != null)
            {

                d.CurrentTicketNumber = t.Number;
                d.LastStateTime = DateTime.Now;
                _context.Desks.Update(d);


                DateTime tm = DateTime.Now;
                t.CurrentDesk = DeskID;
                t.CurrentState = (int)TicketStateEnum.Service;
                t.LastOpr = (int)TicketOprEnum.Call;
                t.LastOprTime = tm;

                _context.Tickets.Update(t);


                TicketState currentState = _context.TicketStates
                    .Where(ts => ts.Ticket == ticketID)
                    .OrderByDescending(ts => ts.ModifiedDate)
                    .FirstOrDefault();
                currentState.Desk = DeskID;
                currentState.User = user;
                currentState.EndTime = tm;

                _context.TicketStates.Update(currentState);



                svc = new()
                {
                    TicketStateValue = (int)TicketStateEnum.Service,
                    Oid = Guid.NewGuid(),
                    Ticket = t.Oid,
                    Desk = DeskID,
                    // DeskName = QorchUserContext.DeskName,
                    User = user,
                    TicketNumber = t.Number,
                    Branch = t.Branch,
                    Segment = t.Segment,
                    SegmentName = t.SegmentName,
                    ServiceType = t.ServiceType,
                    ServiceTypeName = t.ServiceTypeName,
                    StartTime = tm,
                    //CallingRuleDescription = t.CallingRuleDescription,
                    // Macro = t.StateObj.Macro,
                    // MacroObj = t.StateObj.MacroObj,
                    TicketCallType = (int?)(autocall ? TicketCallType.Autocall : ticketID == Guid.Empty ? TicketCallType.Definitive : TicketCallType.Normal)
                };

                if (autocall)
                    svc.CallingRuleDescription += " (autocall)";
                _context.TicketStates.Add(svc);


                svc.TicketNavigation = t;

                _context.SaveChanges();
            }


            return svc;

        }

        public TicketState EndCurrentService(Guid DeskID)
        {
            TicketState current = GetMyCurrentService(DeskID);

            if (current != null)
            {
                DateTime t = DateTime.Now;
                current.EndTime = t;
                current.TicketOprValue = (int)TicketOprEnum.ServiceEnd;

                // Update the TicketState entity
                _context.TicketStates.Update(current);

                Ticket currentTicket = current.TicketNavigation;

                if (currentTicket != null)
                {
                    currentTicket.CurrentDesk = null;
                    currentTicket.LastOprTime = t;
                    currentTicket.LastOpr = (int)TicketOprEnum.ServiceEnd;
                    currentTicket.CurrentState = (int)TicketStateEnum.Final;

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


                    // User = QorchUserContext.UserId,

                    // Macro = currentTicket.Macro?.Macro,
                    //ServiceCode = currentTicket.ServiceCode,


                };

                _context.TicketStates.Add(fin);

            }
            else
            {
                // Handle case when current is null
            }

            // Save changes to the database
            _context.SaveChanges();

            return current;
        }

        public TicketState GetMyCurrentService(Guid DeskID)
        {
            var current = (from tOpr in _context.TicketStates
                           join t in _context.Tickets on tOpr.Ticket equals t.Oid
                           join tp in _context.TicketPools on t.TicketPool equals tp.Oid into poolJoin
                           from pool in poolJoin.DefaultIfEmpty()
                           where tOpr.Desk == DeskID
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
                               TicketNavigation = t // Set TicketNavigation to t
                           })
                          .SingleOrDefault();

            return current;
        }


        public Ticket GetTicket(Guid ticketId)
        {
            var ticket = _context.Tickets
                         .Where(t => t.Oid == ticketId)
                         .Join(
                             _context.TicketStates.Where(ts => ts.TicketOprValue == null),
                             t => t.Oid,
                             ts => ts.Ticket,
                             (t, ts) => t)
                         .FirstOrDefault();

            return ticket;
        }


    }
}
