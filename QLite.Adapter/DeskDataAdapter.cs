using NPoco;
using QLite.Data.Adapter.Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static QLite.Data.Dto.Enums;

namespace QLite.Data.Adapter
{
    public class DeskDataAdapter : BaseDataAdapter
    {
        private readonly static ConcurrentDictionary<string, object> lockerObjects = new();
        private static string Sound { get; set; }

        public DeskDataAdapter(IDbConnectionManager cn)
          : base(cn) { }


        private readonly static object callLock = new object();

        public TicketState CallOperation(string branchId, string ticketId, bool autocall = false, string macroId = null)
        {
            lock (callLock)
            {
                using var transaction = Cn.BeginTransactionScope();


                Desk d = Db.SingleOrDefaultById<Desk>(QorchUserContext.DeskId);
                EndCurrentService();
                d.SetCurrentTicketNumber(null);

                Ticket t;
                TicketState svc = null;
                if (ticketId == null)
                    t = FindTicketToCall(branchId, macroId);
                else
                    t = GetTicket(ticketId);
                if (t != null)
                {
                    d.SetCurrentTicketNumber(t.Number);
                    Update<Desk>(d);

                    DateTime tm = DateTime.Now;
                    t.CurrentDesk = QorchUserContext.DeskId;
                    t.CurrentState = TicketStateEnum.Service;
                    t.LastOpr = TicketOprEnum.Call;
                    t.LastOprTime = tm;
                    Update<Ticket>(t);

                    TicketState currentState = t.StateObj;
                    currentState.TicketOprValue = TicketOprEnum.Call;
                    currentState.Desk = QorchUserContext.DeskId;
                    currentState.User = QorchUserContext.UserId;
                    currentState.EndTime = tm;
                    Update<TicketState>(currentState);

                    string macroGuid = ticketId != null || string.IsNullOrEmpty(macroId) || !Guid.TryParse(macroId, out Guid macro) ? null : macro.ToString();

                    svc = new()
                    {
                        TicketStateValue = TicketStateEnum.Service,
                        Oid = Guid.NewGuid().ToString(),
                        Ticket = t.Oid,
                        Desk = QorchUserContext.DeskId,
                        DeskName = QorchUserContext.DeskName,
                        User = QorchUserContext.UserId,
                        TicketNumber = t.Number,
                        Branch = t.Branch,
                        Segment = t.Segment,
                        SegmentName = t.SegmentObj?.Name,
                        ServiceType = string.IsNullOrEmpty(t.ToServiceType) ? t.ToServiceType : t.ToServiceType,
                        ServiceTypeName = t.ServiceTypeName,
                        StartTime = tm,
                        CallingRuleDescription = t.CallingRuleDescription,
                        Macro = t.StateObj.Macro,
                        MacroObj = t.StateObj.MacroObj,
                        DeskAppType = QorchUserContext.DeskAppType,
                        TicketCallType = autocall ? TicketCallType.Autocall : !string.IsNullOrEmpty(ticketId) ? TicketCallType.Definitive : TicketCallType.Normal
                    };

                    if (autocall)
                        svc.CallingRuleDescription += " (autocall)";

                    Insert<TicketState>(svc);
                    svc.TicketObj = t;
                }
                //LoggerAdapter.Instance.Debug($"Ticket called by {QorchUserContext.Name} from desk {UserContext.DeskName} via description {svc.CallingRuleDescription}");
                transaction.Complete();
                return svc;
            }
        }

        private Ticket GetTicketByNumber(int ticketNumber)
        {
            Sql sql = new(
              @$"select t.*, 'Bekleyen Listesinden Çağrılanlar' CallingRuleDescription, null npoco_StateObj, ts.* 
                   from Ticket t inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null 
                   where t.Branch = @0 and t.CreatedDate >= {SqlClauses.OldestPossibleTicketTime} and t.Number = @1 order by t.CreatedDate desc limit 1", QorchUserContext.BranchId, ticketNumber);

            return SingleByCmd<Ticket>(sql, false);
        }

        public Ticket GetTicket(string ticketId)
        {
            Sql sql = new(
                @$"select t.*, 'Bekleyen Listesinden Çağrılanlar' CallingRuleDescription, null npoco_StateObj, ts.* 
                   from Ticket t inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null 
                   where t.Oid = @0", ticketId);

            return SingleByCmd<Ticket>(sql, false);
        }

        public void PlaySound()
        {
            try
            {
                string program = "vlc.exe";

                string path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "media", "call-service.wav");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    program = "cvlc";
                    //path = "/home/quavis/app/qlserver/wwwroot/media/call-service.wav";
                    path = Path.Combine("/", "home", "quavis", "app", "qlserver", "wwwroot", "media", "call-service.wav"); 
                }

                var pi = new ProcessStartInfo(path)
                {
                    Arguments = Path.GetFileName(path) + " --play-and-exit",
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    FileName = program,
                    Verb = "OPEN",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process p = new Process();
                p.StartInfo = pi;
                p.Start();
                p.WaitForExitAsync();
            }
            catch
            {
            }
        }

        private Ticket FindTicketToCall(string branchId, string macroId)
        {
            string macro = !string.IsNullOrEmpty(macroId) ? macroId : QorchUserContext.MacroUser;


            Ticket t;
            if (!string.IsNullOrEmpty(macro))
            {
                t = FindTicketByMacro(macro, branchId);
                if (t != null)
                {
                    t.StateObj.Macro = macro;
                    t.StateObj.MacroObj = GetMacro(macro);
                }
            }
            else
            {
                t = FindTicket();
            }
            return t;
        }

        public Macro GetMacro(string macroId)
        {
            Sql sql = new(@"select m.* from Macro m where m.Oid = @0 and m.GCRecord is null", macroId);
            return SingleByCmd<Macro>(sql, false);
        }


        public List<KappUser> GetUsersByDeskId(string deskid)
        {
            Sql cmd = new(
                @"select u.*
                  from KappUser u inner join [PermissionPolicyUser] ppu on ppu.Oid=u.Oid
                  where u.Desk = @0 and ppu.GcRecord is null", deskid);
            return ListByCmd<KappUser>(cmd);
        }

        public Desk GetFirstDeskToAutocall(DateTime? lastStateTimeGreaterThan)
        {
            var parameters = new
            {
                QorchUserContext.BranchId,
                LastStateTime = lastStateTimeGreaterThan ?? DateTime.Now.Date,
                OpenStatus = DeskActivityStatus.Open
            };
            var cmd = new Sql(@"select u.Oid ActiveUserOid,d.* 
                                from Desk d 
                                join KappUser u on d.Oid = u.Desk 
                                where d.CurrentTicketNumber is null and d.ActivityStatus = 1 
                                and d.Branch = @BranchId and d.LastStateTime > @LastStateTime and d.ActivityStatus = @OpenStatus
                                order by d.LastStateTime limit 1 ", parameters);
            return SingleByCmd<Desk>(cmd);
        }

        //public TicketState TransferOperation(string ticketId, string transferServiceType, string transferDesk, string transferServiceTypeName)
        public TicketState TransferOperation(TransferTicketDto transferTicketDto)
        {
            using var transaction = Cn.BeginTransactionScope();


            Desk d = Db.SingleOrDefaultById<Desk>(QorchUserContext.DeskId);

            Ticket ticket = null;
            if (!string.IsNullOrEmpty(transferTicketDto.TicketId))
                ticket = GetTicket(transferTicketDto.TicketId);
            else if (d.CurrentTicketNumber != null)
                ticket = GetTicketByNumber(d.CurrentTicketNumber.Value);

            if (ticket == null)
                throw new QorchException($"This ticket is ({transferTicketDto.TicketId} - {transferTicketDto.TicketNumber}) not found.", QorchErrorCodes.TicketNotFound);

            if (ticket != null && ticket.CurrentDesk != null && (transferTicketDto.TransferDesk != null && ticket.CurrentDesk == transferTicketDto.TransferDesk))
                throw new QorchException($"This ticket is ({ticket.Oid}) already being serviced on this desk.", QorchErrorCodes.TransferNotAllowedToSelf);

            if (ticket.CurrentState != Enums.TicketStateEnum.Service)
                throw new QorchException($"Transfer Operation is not allowed for this ticket ({ticket.Oid})", QorchErrorCodes.TransferNotAllowedForTicketState);


            #region find target
            string toDesk = transferTicketDto.TransferDesk;
            if (string.IsNullOrEmpty(transferTicketDto.TransferDesk) && transferTicketDto.TransferDeskNo != null)
                toDesk = GetDeskByNo(QorchUserContext.HwId, transferTicketDto.TransferDeskNo.Value)?.Oid;
            string toService = transferTicketDto.TransferServiceType;
            string toServiceName = transferTicketDto.TransferServiceTypeName;
            if (string.IsNullOrEmpty(transferTicketDto.TransferServiceType) && transferTicketDto.ServiceNo != null)
            {
                ServiceType service = GetServiceByOrder(transferTicketDto.ServiceNo.Value, ticket.Segment);
                toService = service?.Oid;
                toServiceName = service?.Name;
            }

            if (toDesk == null && toService == null)
            {
                throw new QorchException($"Desk-serviceType not fould to transfer ({ticket.Oid})", QorchErrorCodes.TransferNotAllowedToUnknownTarget);
            }
            #endregion


            DateTime tNow = DateTime.Now;
            TicketState current = ticket.StateObj;
            current.EndTime = tNow;
            current.TicketOprValue = Enums.TicketOprEnum.Transfer;

            d.SetCurrentTicketNumber(null);



            TicketState transferState = new()
            {
                Oid = Guid.NewGuid().ToString(),
                Ticket = ticket.Oid,
                TicketStateValue = Enums.TicketStateEnum.Waiting_T,
                StartTime = tNow,
                Segment = ticket.Segment,
                User = QorchUserContext.UserId,
                DeskName = QorchUserContext.DeskName,
                SegmentName = ticket.SegmentName,
                Branch = QorchUserContext.BranchId,
                Macro = ticket.StateObj.Macro,
                MacroObj = ticket.StateObj.MacroObj,
                TicketNumber = ticket.Number,
                ServiceType = transferTicketDto.TransferServiceType == string.Empty ? ticket.ServiceType : transferTicketDto.TransferServiceType,
                ServiceTypeName = string.IsNullOrEmpty(transferTicketDto.TransferServiceTypeName) ? ticket.ServiceTypeName : transferTicketDto.TransferServiceTypeName,
                DeskAppType = QorchUserContext.DeskAppType
            };

            ticket.ToServiceType = string.IsNullOrEmpty(toService) ? ticket.ServiceType : toService;
            ticket.ServiceTypeName = toServiceName ?? ticket.ServiceTypeName;
            if (toDesk != string.Empty)
            {
                transferState.Desk = toDesk;
                ticket.ToDesk = toDesk;
            }

            ticket.LastOpr = Enums.TicketOprEnum.Transfer;
            ticket.LastOprTime = DateTime.Now;
            ticket.CurrentState = Enums.TicketStateEnum.Waiting_T;
            ticket.CurrentDesk = null;
            ticket.TicketNote = transferTicketDto.TicketNote;

            Update(d);
            Update(current);
            Insert(transferState);
            Update(ticket);
            transferState.TicketObj = ticket;

            transaction.Complete();

            return transferState;
        }

        private ServiceType GetServiceByOrder(int serviceNo, string segmentId)
        {
            Sql cmd = new(@"select st.*, tp.NotAvailable, ServiceStartTime,ServiceEndTime,BreakStartTime,BreakEndTime, tp.Oid TicketPool
	                        from ServiceType st 
	                        inner join TicketPool tp on st.Oid = tp.ServiceType and tp.Segment = @1 
	                        and tp.Branch =@0
	                        left join Kiosk k on k.Oid = tp.Kiosk 
	                        where (tp.Kiosk is null or k.Oid = @2) 
	                        and st.GCRecord is null and st.Parent is null and tp.NotAvailable != 1 
                            and st.SeqNo=@3 and tp.GCRecord is null limit 1", QorchUserContext.BranchId, segmentId, QorchUserContext.AppId, serviceNo);
            var service = SingleByCmd<ServiceType>(cmd);
            return service;
        }

        public void SetDeskAutocallStatus(string deskId, bool status)
        {
            Desk desk = SingleById<Desk>(deskId);
            if (desk == null)
                return;
            desk.Autocall = status;
            Update(desk);
        }

        public Desk GetDeskByNo(string kioskHwId, int deskNo)
        {
            Sql cmd = new(
                @"select d.*, b.Macro MacroBranch from Desk d 
                  inner join Kiosk k on d.Pano = k.Oid and k.HwId = @0
                  inner join Branch b on b.Oid = d.Branch
                  where d.DisplayNo = @1 and d.GcRecord is null and k.GcRecord is null order by d.LastStateTime desc limit 1", kioskHwId, deskNo);

            return SingleByCmd<Desk>(cmd);
        }

        public Desk GetDesk(string deskId)
        {
            if (!string.IsNullOrEmpty(deskId))
            {
                Sql cmd = new(
                @"select d.*, b.Name BranchName
                  from Desk d 
                  inner join Branch b on d.Branch = b.Oid
                  where d.Oid = @0 and d.GcRecord is null limit 1", deskId);
                //return SingleById<Desk>(deskId.Value);
                return SingleByCmd<Desk>(cmd);
            }
            return null;
        }

        public List<ServiceType> ListServiceType()
        {
            Sql cmd = new("select st.* from ServiceType st where st.GCRecord is null");
            return ListByCmd<ServiceType>(cmd);
        }

        public List<Desk> ListDesk4Branch(string branchOid)
        {
            if (string.IsNullOrEmpty(branchOid))
                return List<Desk>(x => x.GCRecord == null);
            else
            {
                var parsed = Guid.TryParse(branchOid, out Guid branchId);
                if (!parsed)
                    return new List<Desk>();
                return List<Desk>(x => x.Branch == branchId.ToString() && x.GCRecord == null);
            }
        }

        public Desk GetDeskByMacId(string macId)
        {
            Sql cmd = new Sql($@"select d.*, b.*, p.HwId PanoHwId, b.Name BranchName from [Desk] d 
                                    inner join Branch b on d.Branch = b.Oid 
                                    inner join Kiosk p on p.Oid = d.Pano
                                    where b.GCRecord is null and d.GCRecord is null and p.GCRecord is null and d.MacId=@0", macId);
            return SingleByCmd<Desk>(cmd);
        }

        public TicketState CancelTicket(string ticketId)
        {
            using var transaction = Cn.BeginTransactionScope();

            Ticket ticket = GetTicket(ticketId);
            if (ticket == null)
                return null;
            DateTime t = DateTime.Now;
            TicketState current = ticket.StateObj;
            current.EndTime = t;
            current.TicketOprValue = Enums.TicketOprEnum.Cancel;

            TicketState ticketState = new()
            {
                Oid = Guid.NewGuid().ToString(),
                User = QorchUserContext.UserId,
                Ticket = ticket.Oid,
                TicketStateValue = Enums.TicketStateEnum.Final,
                StartTime = t,
                Segment = ticket.Segment,
                SegmentName = ticket.SegmentName,
                ServiceType = string.IsNullOrEmpty(ticket.ToServiceType) ? ticket.ServiceType : ticket.ToServiceType,
                ServiceTypeName = ticket.ServiceTypeName,
                Macro = ticket.StateObj.Macro,
                MacroObj = ticket.StateObj.MacroObj,
                Branch = QorchUserContext.BranchId,
                TicketNumber = ticket.Number,
                DeskAppType = QorchUserContext.DeskAppType
            };

            ticket.LastOpr = Enums.TicketOprEnum.Cancel;
            ticket.LastOprTime = t;
            ticket.CurrentState = Enums.TicketStateEnum.Final;
            ticket.CurrentDesk = null;

            Desk desk = Db.SingleOrDefaultById<Desk>(QorchUserContext.DeskId);
            desk.SetCurrentTicketNumber(null);

            Update(desk);
            Update(current);
            Insert(ticketState);
            ticketState.TicketObj = ticket;
            Update(ticket);

            transaction.Complete();
            return ticketState;
        }

        public TicketState SendToWaitingListFromCompleted(TicketStateDto ticketStateDto)
        {
            using var transaction = Cn.BeginTransactionScope();
            var ticket = GetTicket(ticketStateDto.TicketObj.Oid);
            if (ticket == null)
                return null;
            TicketState current = ticket.StateObj;
            DateTime t = DateTime.Now;
            current.TicketOprValue = TicketOprEnum.SendToWaiting;
            current.EndTime = t;
            Update(current);

            ticket.CurrentState = TicketStateEnum.Waiting;
            ticket.LastOpr = null;
            ticket.LastOprTime = t;
            Update<Ticket>(ticket);

            TicketState waitingTicketState = new()
            {
                Oid = Guid.NewGuid().ToString(),
                Ticket = ticket.Oid,
                User = QorchUserContext.UserId,
                TicketStateValue = TicketStateEnum.Waiting,
                TicketOprValue = null,
                StartTime = t,
                ServiceType = ticket.ToServiceType == string.Empty ? ticketStateDto.ServiceType : ticket.ToServiceType,
                ServiceTypeName = string.IsNullOrEmpty(ticket.ServiceTypeName) ? ticketStateDto.ServiceTypeName : ticket.ServiceTypeName,
                Segment = ticketStateDto.Segment,
                SegmentName = ticketStateDto.SegmentName,
                Macro = ticket.StateObj.Macro,
                MacroObj = ticket.StateObj.MacroObj,
                Branch = ticketStateDto.Branch,
                TicketNumber = ticketStateDto.TicketNumber,
                DeskAppType = QorchUserContext.DeskAppType
            };

            Insert(waitingTicketState);
            waitingTicketState.TicketObj = ticket;

            transaction.Complete();
            return waitingTicketState;
        }

        public TicketState ParkOperation(ParkTicketDto parkTicketDto)
        {
            using var transaction = Cn.BeginTransactionScope();

            Ticket parkTicket = GetTicket(parkTicketDto.TicketId);

            if (parkTicket.CurrentState != Enums.TicketStateEnum.Service)
                throw new Exception("Park Operation is not allowed for this ticket");

            DateTime t = DateTime.Now;

            TicketState svc = parkTicket.StateObj;
            svc.EndTime = t;
            svc.TicketOprValue = Enums.TicketOprEnum.Park;
            Update(svc);

            parkTicket.CurrentState = TicketStateEnum.Park;
            parkTicket.LastOpr = TicketOprEnum.Park;
            parkTicket.LastOprTime = t;
            parkTicket.TicketNote = parkTicketDto.TicketNote;

            //TODO:parktaki ticket'ın current desk i boş olmalı ? Ahmet doldurmuş ben boşalttım bi konuşalım.
            // 10.11.2021-Konuştuk doldurmaya karar verdik.
            //parkTicket.CurrentDesk = null;
            Update<Ticket>(parkTicket);

            TicketState parkTicketOpr = new()
            {
                User = QorchUserContext.UserId,
                Desk = QorchUserContext.DeskId.ToString(),
                DeskName = QorchUserContext.DeskName,
                Oid = Guid.NewGuid().ToString(),
                Ticket = parkTicket.Oid,
                TicketStateValue = Enums.TicketStateEnum.Park,
                StartTime = t,
                ServiceType = parkTicket.ToServiceType == string.Empty ? svc.ServiceType : parkTicket.ToServiceType,
                ServiceTypeName = string.IsNullOrEmpty(parkTicket.ServiceTypeName) ? svc.ServiceTypeName : parkTicket.ServiceTypeName,
                Segment = parkTicket.Segment,
                SegmentName = parkTicket.SegmentName,
                Branch = QorchUserContext.BranchId,
                Macro = parkTicket.StateObj.Macro,
                MacroObj = parkTicket.StateObj.MacroObj,
                TicketNumber = parkTicket.Number,
                DeskAppType = QorchUserContext.DeskAppType
            };
            Insert<TicketState>(parkTicketOpr);
            parkTicketOpr.TicketObj = parkTicket;
            Desk d = Db.SingleOrDefaultById<Desk>(QorchUserContext.DeskId);
            d.SetCurrentTicketNumber(null);
            Update(d);

            transaction.Complete();

            return parkTicketOpr;
        }

        //public void ClearSelectedMacro()
        //{
        //    Sql sql = new("update DeskMacroSchedule set selected = 0 where ([Desk] =@1 and [User] is null) or ([User]=@0 and [Desk] is null)", QorchUserContext.UserId, QorchUserContext.DeskId);
        //    Execute(sql);
        //}

        //public void SetSelectedMacro(string macroId)
        //{
        //    ClearSelectedMacro();
        //    Sql sql = new("update DeskMacroSchedule set selected = 1 where Macro =@0 ", macroId);
        //    Execute(sql);
        //}

        //public List<Macro> GetMacroByUserAndDesk()
        //{
        //    List<Macro> dms = new List<Macro>();
        //    if (string.IsNullOrEmpty(QorchUserContext.DeskId))
        //        return dms;
        //    Sql sql = new($@"select dms.*, null npoco_MacroObj, m.* 
        //                    from DeskMacroSchedule dms inner join Macro m on dms.Macro = m.Oid where
        //                    (dms.[User]=@0 or dms.[Desk] =@1) and (dms.[Pasif] = 0 or dms.[Pasif] is null)", QorchUserContext.UserId, QorchUserContext.DeskId);

        //    var dmsList = ListByCmd<DeskMacroSchedule>(sql);

        //    if (dmsList.Count != 0)
        //    {
        //        dms = dmsList.Select(x => x.MacroObj).ToList();

        //        return dms;
        //    }
        //    else
        //    {
        //        Sql sql2 = new($@"select dms.*, null npoco_MacroObj, m.* 
        //                    from DeskMacroSchedule dms inner join Macro m on dms.Macro = m.Oid where
        //                    dms.[Branch]=@0 and dms.[User] is null and dms.[Desk] is null and (dms.[Pasif] = 0 or dms.[Pasif] is null)", QorchUserContext.BranchId);

        //        var dmsList2 = ListByCmd<DeskMacroSchedule>(sql2);

        //        dms = dmsList2.Select(x => x.MacroObj).ToList();

        //        return dms;

        //    }

        //}

        //private Ticket GetTicketByMaxWait(Macro macro, string branchId)
        //{
        //    if (macro.MaxWaitingTime == 0)
        //        return null;

        //    string ToThisDeskCriteria = "";
        //    if (macro.ToThisDesk == MeNotothersAll.Me)
        //        ToThisDeskCriteria = $"and t.ToDesk = @4";
        //    if (macro.ToThisDesk == MeNotothersAll.Notothers)
        //        ToThisDeskCriteria = $"and (t.ToDesk = @4 or t.ToDesk is null)";

        //    string sqlTxt = $@"select top 1 t.*,  null npoco_ServiceTypeObj, st.*,  null npoco_SegmentObj, sgm.*, null npoco_StateObj, ts.*
        //                    from Ticket t
        //                    left join ServiceType st on st.Oid = t.ServiceType and st.GCRecord is null
        //                    left join Segment sgm on sgm.Oid = t.Segment and sgm.GCRecord is null
        //                    inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
        //                    where t.Branch = @0 and  (t.CurrentState=@2 or t.CurrentState=@3) and @1 < DATEDIFF(MINUTE, t.LastOprTime, GETDATE()) {ToThisDeskCriteria}
        //                    order by t.LastOprTime";
        //    //CreatedDate filtresini LastOprTime ile değiştirdim 
        //    Sql sql = new(sqlTxt, branchId, macro.MaxWaitingTime, TicketStateEnum.Waiting, TicketStateEnum.Waiting_T, QorchUserContext.DeskId);
        //    return SingleByCmd<Ticket>(sql);
        //}

        //public string MacroQueryWaitTime =>
        //        $@"select top 1 t.*, mr.Description CallingRuleDescription, null npoco_ServiceTypeObj, st.*,
        //           null npoco_SegmentObj, sgm.*, null npoco_StateObj, ts.* 
        //           from MacroRule mr
        //           inner join Ticket t on (mr.ServiceType = t.ServiceType or mr.ServiceType is null) and
        //                                  (mr.Segment = t.Segment or mr.Segment is null) and
        //                                  (mr.MinWaitingTime = 0 or mr.MinWaitingTime is null or mr.MinWaitingTime < DATEDIFF(MINUTE, t.CreatedDate, GETDATE())) and
        //                                  (mr.Transfer != 1 or t.CurrentState = @3) and
        //                                  (mr.ToThisDesk = {(int)MeNotothersAll.All} or (mr.ToThisDesk = {(int)MeNotothersAll.Me} and t.ToDesk = @4) or (mr.ToThisDesk = {(int)MeNotothersAll.Notothers} and (t.ToDesk = @4 or t.ToDesk is null)))
        //           left join ServiceType st on st.Oid = t.ServiceType and st.GCRecord is null
        //           left join Segment sgm on sgm.Oid = t.Segment and sgm.GCRecord is null
        //           inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
        //           where t.Branch = @1 and mr.Macro = @0 and (t.CurrentState=@2 or t.CurrentState=@3) and t.CreatedDate >= {SqlClauses.OldestPossibleTicketTime}"; //t.DayOfYear = datename(dy,GETDATE()) and t.Year = YEAR(GETDATE())"; }

        public string MacroQuerySeq =>
                $@"select t.* ,
                    CASE WHEN mr.MaxWaitingTime < (julianday('now','localtime') - julianday(t.LastOprTime)) THEN 0
                    WHEN mr.MaxWaitingTime >= (julianday('now','localtime') - julianday(t.LastOprTime)) THEN mr.Sequence 
                    END as Seq, 
                    CASE WHEN mr.MaxWaitingTime < (julianday('now','localtime') - julianday(t.LastOprTime)) THEN  ( 'Maksimum bekleme süresi olan ' || (mr.MaxWaitingTime || ' dk lık süreyi tamamlamıştır.'))
                    WHEN mr.MaxWaitingTime >= (julianday('now','localtime') - julianday(t.LastOprTime)) THEN mr.Description
                    END as CallingRuleDescription,
                    t.*, null npoco_ServiceTypeObj, st.*,
                    null npoco_SegmentObj, sgm.*, null npoco_StateObj, ts.* 
                    from MacroRule mr
                    inner join Ticket t on (mr.ServiceType = t.ServiceType or mr.ServiceType is null) and
                                            (mr.Segment = t.Segment or mr.Segment is null)  and
                                            (mr.Transfer != 1 or t.CurrentState = @3) and
                                            (mr.ToThisDesk = {(int)MeNotothersAll.All} or (mr.ToThisDesk = {(int)MeNotothersAll.Me} and t.ToDesk = @4) or (mr.ToThisDesk = {(int)MeNotothersAll.Notothers} and (t.ToDesk = @4 or t.ToDesk is null)))
                    left join ServiceType st on st.Oid = t.ServiceType and st.GCRecord is null
                    left join Segment sgm on sgm.Oid = t.Segment and sgm.GCRecord is null
                    inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
                    where t.Branch = @1 and mr.Macro = @0 and (t.CurrentState=@2 or t.CurrentState=@3) and mr.MinWaitingTime <= (julianday('now','localtime') - julianday(t.LastOprTime)) and mr.GCRecord is null 
                    and t.CreatedDate >= {SqlClauses.OldestPossibleTicketTime} order by Seq, t.LastOprTime limit 1"; // t.DayOfYear = datename(dy,GETDATE()) and t.Year = YEAR(GETDATE())"; }

        public Ticket FindTicketByMacro(string macroId, string branchId)
        {
            Sql sql = null;
            sql = new Sql(MacroQuerySeq, macroId, branchId, TicketStateEnum.Waiting, TicketStateEnum.Waiting_T, QorchUserContext.DeskId);

            return SingleByCmd<Ticket>(sql);
        }

        public Ticket FindTicket()
        {
            string toThisDeskCriteria = $"and (t.ToDesk = @3 or t.ToDesk is null)";
            Sql cmd = new(
                $@"SELECT t.*, 'En çok bekleyen' CallingRuleDescription, null npoco_ServiceTypeObj,
                   st.*, null npoco_SegmentObj, sgm.*, null npoco_StateObj, ts.* 
                   FROM Ticket t
                   LEFT OUTER JOIN ServiceType st ON t.ServiceType = st.Oid
                   LEFT OUTER JOIN Segment sgm ON t.Segment = sgm.Oid
                   inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
                   where t.Branch=@0 and (t.CurrentState=@1 or t.CurrentState=@2) 
                   and t.CreatedDate >= {SqlClauses.OldestPossibleTicketTime} {toThisDeskCriteria} 
                   order by t.CreatedDate limit 1", QorchUserContext.BranchId, TicketStateEnum.Waiting, TicketStateEnum.Waiting_T, QorchUserContext.DeskId);
            Ticket t = ListByCmd<Ticket>(cmd).FirstOrDefault();
            return t;
        }

        public TicketState EndCurrentService()
        {
            // İstekler üst üste geldiğinde `TicketOprValue`su null olan iki kayıt oluşabiliyor.
            // Bir ticket için `TicketOprValue`su null olan iki `TicketState` kaydı olamaz
            string lockKey = $"DESK/{QorchUserContext.DeskId}";
            object lockObject = lockerObjects.GetOrAdd(lockKey, _ => new object());
            lock (lockObject)
            {
                TicketState current = GetMyCurrentService();

                if (current != null)
                {
                    using var transaction = Cn.BeginTransactionScope();

                    DateTime t = DateTime.Now;
                    current.EndTime = t;
                    current.TicketOprValue = TicketOprEnum.ServiceEnd;
                    Update(current);

                    if (current.TicketObj != null)
                    {
                        Ticket ticket = current.TicketObj;
                        ticket.CurrentDesk = null;
                        ticket.LastOprTime = t;
                        ticket.LastOpr = TicketOprEnum.ServiceEnd;
                        ticket.CurrentState = TicketStateEnum.Final;
                        Update(ticket);

                        TicketState fin = new()
                        {
                            Oid = Guid.NewGuid().ToString(),
                            User = QorchUserContext.UserId,
                            Desk = QorchUserContext.DeskId,
                            Branch = ticket.Branch,
                            Segment = ticket.Segment,
                            SegmentName = ticket.SegmentName,
                            ServiceType = ticket.ToServiceType == string.Empty ? current.ServiceType : ticket.ToServiceType,
                            ServiceTypeName = string.IsNullOrEmpty(ticket.ServiceTypeName) ? current.ServiceTypeName : ticket.ServiceTypeName,
                            Macro = ticket.StateObj?.Macro,
                            MacroObj = ticket.StateObj?.MacroObj,
                            Ticket = ticket.Oid,
                            TicketNumber = ticket.Number,
                            TicketStateValue = TicketStateEnum.Final,
                            StartTime = t,
                            DeskAppType = QorchUserContext.DeskAppType
                        };

                        Insert(fin);
                    }

                    Desk desk = Db.SingleOrDefaultById<Desk>(QorchUserContext.DeskId);
                    desk.SetCurrentTicketNumber(null);
                    Update(desk);

                    transaction.Complete();
                }
                return current;
            }
        }

        public TicketState GetMyCurrentService()
        {
            Sql cmd = new(@"select tOpr.* , null npoco_TicketObj, t.* 
                            from [TicketState] tOpr
                            inner join Ticket t on tOpr.Ticket = t.Oid
                            where tOpr.Desk=@0 and tOpr.TicketStateValue=@1 
                            and tOpr.TicketOprValue is null ", QorchUserContext.DeskId, TicketStateEnum.Service);
            var current = SingleByCmd<TicketState>(cmd);
            return current;
        }

        public TicketState GetSelectedDeskTicket(string deskId)
        {
            Sql cmd = new(@"select tOpr.*, null npoco_TicketObj, t.* 
                            from [TicketState] tOpr
                            inner join Ticket t on tOpr.Ticket = t.Oid
                            where tOpr.Desk=@0 and tOpr.TicketStateValue =@1 and tOpr.TicketOprValue is null", deskId, TicketStateEnum.Service);
            var current = SingleByCmd<TicketState>(cmd);
            if (current != null)
            {
                current.TicketObj = GetTicket(current.Ticket);
            }
            return current;
        }

        public DeskStatus GetLastDeskStatus(string deskId, string? userId)
        {
            //Sql cmd = new(@"select top 1 dSts.* from [DeskStatus] dSts where [Desk]=@0 and [User]=@1 order by [CreatedDate] desc", deskId, userId);
            Sql cmd = new(@"select dSts.* from [DeskStatus] dSts where [Desk]=@0 and ([User]=@1 or @1 is null) and [StateEndTime] is null order by StateStartTime desc limit 1", deskId, userId);

            var latestDeskStatus = SingleByCmd<DeskStatus>(cmd);
            return latestDeskStatus;
        }





        public List<Branch> GetBranchDetails(string accountId)
        {
            List<Branch> data = new();
            Sql cmd = new(@"select b.*, p.Name as ProvinceName,  c.Name as CountryName, a.LogoS as AccountLogo  from [Branch] as b 
                            inner join [Country] as c on c.Oid=b.Country 
                            inner join [Account] as a on a.Oid=b.Account 
                            inner join [Province] as p on p.Oid=b.Province where b.Account=@0", accountId);
            data = ListByCmd<Branch>(cmd);
            return data;
        }


        public List<Macro> GetAllMacros()
        {
            Sql cmd = new(@"select * from Macro where GCRecord is null");
            return ListByCmd<Macro>(cmd);
        }


        public TicketState GetLastTicketState(string ticketId)
        {
            Sql cmd = new(@"select tOpr.*,d.Name DeskName, null npoco_TicketObj, t.* 
                            from TicketState tOpr inner join Ticket t on tOpr.Ticket = t.Oid
                            left join Desk d on d.Oid=tOpr.Desk
                            where tOpr.Ticket =@0 and tOpr.TicketOprValue is null limit 1", ticketId);
            var state = SingleByCmd<TicketState>(cmd);
            //state.TicketObj = GetTicket(state.Ticket);
            return state;
        }

        public int GetTicketDuration(string ticketId)
        {
            Sql cmd = new("select sum(datediff(second,StartTime,EndTime)) from TicketState where Ticket=@0 and EndTime is not null and TicketStateValue =@1", ticketId, TicketStateEnum.Service);
            int Duration = SingleByCmd<int>(cmd);
            return Duration;
        }

        public List<Ticket> ListWaitingTickets()
        {
            string sql =
             @$"select t.*, null npoco_StateObj, ts.*, d.Name as DeskName
                from Ticket t 
                inner join TicketState ts on ts.Ticket = t.Oid and ts.TicketOprValue is null
                left join Desk d on d.Oid = ts.Desk
                where t.ModifiedDate >= {SqlClauses.OldestPossibleTicketTime} 
                and t.CurrentState in(@0,@1,@2) and t.Branch=@3 ";
            object[] args =
            {
                TicketStateEnum.Waiting,
                TicketStateEnum.Waiting_T,
                TicketStateEnum.Park,
                QorchUserContext.BranchId ?? string.Empty
            };
            var res = List<Ticket>(sql, args);
            return res;
        }



        public List<TicketState> ListServicedByDesk(string DeskOId, int takeLast = 0)
        {
            //return List<Service>((x => x.Desk == DeskOId && x.CreatedDate >= DateTime.Now.Date && x.CreatedDate <= DateTime.Now.AddDays(1).Date));
            //select svc.*, svcT.*,sg.*,b.* from[Service] svc inner join ServiceType svcT on svc.ServiceType = svcT.Oid inner join Segment sg on svc.Segment = sg.Oid inner join Branch b on svc.Branch = b.Oid where svc.Desk = '" + DeskOId.ToString() + "' and svc.CreatedDate = cast(getdate() as Date)
            Sql cmd = new(@"select svc.*,  null npoco_TicketObj, t.*
                            from TicketState svc inner join Ticket t on svc.Ticket = t.Oid
                            where svc.Desk = @0 and cast(svc.ModifiedDate as Date) = cast(DATE() as Date) and TicketStateValue=@1 and TicketOprValue = @2",
                            DeskOId, TicketStateEnum.Service, TicketOprEnum.ServiceEnd);
            var serviced = ListByCmd<TicketState>(cmd);

            foreach (var item in serviced)
            {
                //item.TicketObj = GetTicket(item.Ticket);
                //if (item.TicketObj == null)
                //    CommonContext.EmseLogger.Warning("GetTicket return null");
                item.Duration = (item.EndTime - item.StartTime)?.ToString(@"h\:mm\:ss");
            }
            if (takeLast > 0)
            {
                return serviced.TakeLast(takeLast).ToList();
            }
            return serviced;
        }

        public DeskActivityStatus GetDeskStatus(string deskId)
        {
            Desk d = SingleById<Desk>(deskId);
            return d.ActivityStatus;
        }

        public KappUser GetUser(string OId)
        {
            Sql cmd = new(
                @"select u.*, ppu.UserName, ppu.StoredPassword, d.Pano, d.Name DeskName, null npoco_BranchObj, b.* 
                  from KappUser u inner join PermissionPolicyUser ppu on u.Oid = ppu.Oid 
                  left join Desk d on d.Oid = u.Desk and d.GCRecord is null 
                  left join Branch b on u.Branch = b.Oid 
                  where ppu.Oid = @0 and ppu.GCRecord is null", OId);
            KappUser model = SingleByCmd<KappUser>(cmd);

            return model;
        }

        public void ClearSelectedDeskFromUsers(string deskId)
        {
            Sql sql = new("update KappUser set Desk = null where Desk = @0", deskId);
            Execute(sql);
        }

        public void ClearUserDesk(string userId)
        {
            KappUser user = GetUser(userId);
            user.Desk = null;
            Db.Save(user);
        }

        public string SetUserToDesk(string userId, string deskId)
        {
            using var transaction = Cn.BeginTransactionScope();

            KappUser user = GetUser(userId);
            Desk desk = SingleById<Desk>(deskId);
            string oldDeskId = null;
            if (user.Desk != null && deskId != user.Desk)
            {
                oldDeskId = user.Desk;
                SetDeskStatus(oldDeskId, DeskActivityStatus.Closed, userId);
            }

            ClearSelectedDeskFromUsers(deskId);

            user.Desk = deskId;
            user.Branch = desk.Branch;
            desk.ActiveUser = user.UserName;
            desk.ActiveUserOid = user.Oid;
            Db.Save(user);
            Save(desk);
            SetDeskStatus(deskId, DeskActivityStatus.Open, userId);

            transaction.Complete();
            return oldDeskId;
        }

        public void SetDeskStatus(string deskId, DeskActivityStatus status, string userId = null)
        {
            string lockKey = $"BRANCH/{QorchUserContext.BranchId}";
            object lockObject = lockerObjects.GetOrAdd(lockKey, _ => new object());
            lock (lockObject)
            {
                using var transaction = Cn.BeginTransactionScope();
                var user = (string.IsNullOrEmpty(userId) ? QorchUserContext.UserId : userId);
                var t = DateTime.Now;

                if (status == DeskActivityStatus.Paused || status == DeskActivityStatus.Closed || status == DeskActivityStatus.Busy)
                {
                    EndCurrentService();
                }


                DeskStatus deskStatus = new()
                {
                    Oid = Guid.NewGuid().ToString(),
                    User = user,
                    Desk = deskId,
                    DeskActivityStatus = status,
                    StateEndTime = null,
                    StateStartTime = t
                };

                var deskLatestStatus = GetLastDeskStatus(deskId, user);
                if (deskLatestStatus != null)
                {
                    deskLatestStatus.StateEndTime = t;
                    Update(deskLatestStatus);
                }

                Desk desk = SingleById<Desk>(deskId);
                desk.SetActivityStatus(status);
                Insert(deskStatus);
                Update(desk);

                transaction.Complete();
            }
        }

    }
}
