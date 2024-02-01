using NPoco;
using QLite.Data.Adapter.Common;
using QLite.Data.Dto;
using QLite.Data.Model;
using QLite.Data.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static QLite.Data.Dto.Enums;

namespace QLite.Data.Adapter
{
    public class MonitorDataAdapter : BaseDataAdapter
    {
        public MonitorDataAdapter(IDbConnectionManager cn)
         : base(cn)
        {
        }

        public Ticket GetTicket(string ticketId)
        {
            Sql sql = new Sql(@"select t.*, 'Bekleyen Listesinden Çağrılanlar' CallingRuleDescription, null npoco_StateObj, ts.* from ticket t
                                inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
                                where t.Oid = @0", ticketId);
            return SingleByCmd<Ticket>(sql);
        }
        
        public List<Desk> ListDesk4Branch(string branchOid)
        {
            return List<Desk>(x => x.Branch == branchOid && x.GCRecord == null);
        }
        
        public TicketState GetDeskCurrentService(string deskId)
        {
            Sql cmd = new Sql(@"select tOpr.*, null npoco_TicketObj, t.* from [TicketState] tOpr
                                inner join Ticket t on tOpr.Ticket = t.Oid
                                where tOpr.Desk=@0 and tOpr.TicketStateValue =@1 and tOpr.TicketOprValue is null", deskId, TicketStateEnum.Service);
            return SingleByCmd<TicketState>(cmd);

        }

        public TicketState GetLastTicketState(string ticketId)
        {
            Sql cmd = new Sql(@"select tOpr.* from [TicketState] tOpr
                                where tOpr.Ticket =@0 and tOpr.TicketOprValue is null limit 1", ticketId);
            return SingleByCmd<TicketState>(cmd);
        }

        public List<Ticket> ListWaitingTickets()
        {
            Sql cmd = new Sql("select t.*, s.*,b.*,sg.* from [Ticket] t inner join ServiceType s on t.ServiceType = s.Oid inner join Segment sg on t.Segment = sg.Oid inner join Branch b on t.Branch = b.Oid where cast(t.CreatedDate as Date) = cast(getdate() as Date) and t.CurrentState in(@0,@1,@2)", TicketStateEnum.Waiting, TicketStateEnum.Waiting_T , TicketStateEnum.Park);
            return ListByCmd<Ticket>(cmd);
        }

        public List<TicketState> ListServiceByDesk(string DeskOId)
        {
            //return List<Service>((x => x.Desk == DeskOId && x.CreatedDate >= DateTime.Now.Date && x.CreatedDate <= DateTime.Now.AddDays(1).Date));
            //select svc.*, svcT.*,sg.*,b.* from[Service] svc inner join ServiceType svcT on svc.ServiceType = svcT.Oid inner join Segment sg on svc.Segment = sg.Oid inner join Branch b on svc.Branch = b.Oid where svc.Desk = '" + DeskOId.ToString() + "' and svc.CreatedDate = cast(getdate() as Date)
            Sql cmd = new Sql(@"select svc.*, b.* from [TicketState] svc
                                inner join Branch b on svc.Branch = b.Oid
                                where svc.Desk = '" + DeskOId.ToString() + "' and cast(svc.CreatedDate as Date) = cast(getdate() as Date) and TicketStateValue=@0 and TicketOprValue = @1", TicketStateEnum.Service, TicketOprEnum.ServiceEnd);
            return ListByCmd<TicketState>(cmd);
        }

        public DeskActivityStatus GetDeskStatus(string deskId)
        {
            Desk d = SingleById<Desk>(deskId);
            return d.ActivityStatus;
        }

    }
}
