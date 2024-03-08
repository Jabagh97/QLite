using Microsoft.EntityFrameworkCore;
using NPoco;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.Data.Models;
using QLiteDataApi.Context;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Services
{
    public interface IDeskService
    {
        object GetTicketsByState(TicketStateEnum state);
        TicketState CallTicket(Guid DeskID, Guid ticketID, Guid user, Guid Macro, bool autocall = false);
        TicketState GetMyCurrentService(Guid DeskID);
        TicketState EndCurrentService(Guid DeskID);
        int GetTicketDuration(Guid ticketid);
        TicketState TransferOperation(TransferTicketDto transferTicketDto);

        TicketState ParkOperation(ParkTicketDto parkTicketDto);

        Desk GetDesk(Guid DeskID);

        List<DeskMacroSchedule> GetMacros(Guid DeskID);

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

            //TODO: Filter By Desk
            var query = from t in _context.Tickets
                        where t.CurrentState == (int)state && t.ModifiedDate >= DateTime.UtcNow.AddMinutes(-480)
                        join tp in _context.TicketPools on t.TicketPool equals tp.Oid into tpJoin
                        from tp in tpJoin.DefaultIfEmpty()
                        select new
                        {
                            TicketNumber = tp.ServiceCode + t.Number,
                            Service = t.ServiceTypeName,
                            Segment = t.SegmentName,
                            Oid = t.Oid,
                        };

            var waitingTickets = query.ToList();

            var recordsTotal = waitingTickets.Count;
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = waitingTickets };

            return jsonData;
        }

        private Ticket FindTicketToCall(Guid? branchId, Guid macroId, Guid DeskId)
        {
            Macro macro = new Macro();
            if (macroId != Guid.Empty)
                macro = _context.Macros.Where(m => m.Oid == macroId && m.Gcrecord == null).First();
            //TODO:if Macro is null find Macro or force desk user to choose macro before showing the view 

            Ticket t = new Ticket();
            if (macro != null)
            {
                t = FindTicketByMacro(macroId, branchId, DeskId);
                if (t != null)
                {
                    //t.TicketState.Macro = macro?.Oid;
                    //t.TicketState.MacroObject = macro;

                    var ticketPool = _context.TicketPools.Where(tp=>tp.Oid == t.TicketPool).FirstOrDefault();
                    if (ticketPool != null)
                    {
                        t.ServiceCode = ticketPool.ServiceCode;
                    }
                }
            }
            //TODO:find ticket anyway even if no macro found ???????

            return t;
        }

        public Ticket FindTicketByMacro(Guid macroId, Guid? branchId, Guid deskId)
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
                                
                            },
                          
                        };


            var result = query.FirstOrDefault();

            if (result != null)
            {
                // Map properties to Ticket entity
                var mappedTicket = result.Ticket;


                return mappedTicket;
            }

            // Handle case when no matching records found
            return null;
        }




        public TicketState CallTicket(Guid DeskID, Guid ticketID, Guid user, Guid Macro, bool autocall = false)
        {

            Desk? d = _context.Desks.Find(DeskID);
            EndCurrentService(DeskID);

            d.CurrentTicketNumber = null;
            d.LastStateTime = DateTime.Now;

            Ticket t;
            TicketState svc = null;
            if (ticketID == Guid.Empty)

                t = FindTicketToCall(d.Branch, Macro, DeskID);

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


                TicketState currentState = t.TicketState;

                currentState.Desk = DeskID;
                currentState.User = user;
                currentState.EndTime = tm;
                currentState.ModifiedDate = DateTime.Now;
                currentState.ModifiedDateUtc = DateTime.UtcNow;

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
                    ServiceCode = t.ServiceCode,
                    CreatedDate = DateTime.Now,
                    CreatedDateUtc = DateTime.UtcNow,
                    ModifiedDate = DateTime.Now,
                    ModifiedDateUtc = DateTime.UtcNow,
                    DisplayNo = d.DisplayNo,


                    CallingRuleDescription = t.CallingRuleDescription,
                    Macro = Macro,
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
                    currentTicket.ModifiedDate = DateTime.Now;
                    currentTicket.ModifiedDateUtc = DateTime.UtcNow;

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
                           from tp in poolJoin.DefaultIfEmpty()
                           where t.CurrentState == 2 && tOpr.Desk == DeskID
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
                          .FirstOrDefault();

            return current;
        }


        public Ticket GetTicket(Guid ticketId)
        {
            var ticket = (from t in _context.Tickets
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
                          }
                          ).FirstOrDefault();



            return ticket;
        }

        public int GetTicketDuration(Guid ticketId)
        {
            int duration = 1;

            return duration;
        }

        public TicketState ParkOperation(ParkTicketDto parkTicketDto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                Ticket parkTicket = GetTicket(parkTicketDto.TicketId);

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
                    ModifiedDateUtc = DateTime.UtcNow,

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

                _context.SaveChanges();
                transaction.Commit();

                return parkTicketOpr;
            }
        }

        public TicketState TransferOperation(TransferTicketDto transferTicketDto)
        {
            throw new NotImplementedException();
        }


        public Desk GetDesk(Guid DeskID)

        {
            var desk = _context.Desks.Where(d => d.Oid == DeskID && d.Gcrecord == null).First();


            return desk;
        }

        public List<DeskMacroSchedule> GetMacros(Guid DeskID)

        {
            // Fetch the corresponding MacroName for each DeskMacroSchedule
            var macrosWithMacroNames = _context.DeskMacroSchedules
                .Where(dms => dms.Desk == DeskID && dms.Gcrecord == null)
                .Join(
                    _context.Macros,
                    dms => dms.Macro, // DeskMacroSchedule.Macro
                    macro => macro.Oid, // Macro.Oid
                    (dms, macro) => new { DeskMacroSchedule = dms, MacroName = macro.Name }
                )
                .ToList();

            // Update the MacroName property for each DeskMacroSchedule
            foreach (var item in macrosWithMacroNames)
            {
                item.DeskMacroSchedule.MacroName = item.MacroName;
            }

            // Now macrosWithMacroNames contains DeskMacroSchedule objects with updated MacroName property
            var macros = macrosWithMacroNames.Select(item => item.DeskMacroSchedule).ToList();


            return macros;
        }
    }
}
