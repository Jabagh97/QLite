using Microsoft.Extensions.DependencyInjection;
using NPoco;
using QLite.Common;
using QLite.Data.Adapter.Common;
using QLite.Data.Adapter.Kapp;
using QLite.Data.Dto;
using QLite.Data.Model;
using QLite.Data.Model.Common;
using QLite.Data.Model.Kapp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static QLite.Data.Dto.Enums;

namespace QLite.Data.Adapter
{
    public class KioskDataAdapter : BaseDataAdapter
    {
        private readonly DeskDataAdapter _deskDa;
        private readonly static ConcurrentDictionary<string, object> lockerObjects = new();

        public KioskDataAdapter(IDbConnectionManager cn, DeskDataAdapter deskDa)
            : base(cn)
        {

            _deskDa = deskDa;
            //deskDataAdapterLazy = new Lazy<DeskDataAdapter>(() => sp.GetRequiredService<DeskDataAdapter>());
            //deskDataAdapterLazy = new Lazy<DeskDataAdapter>(() => CommonContext.AppLevelDIContainer.GetService<DeskDataAdapter>());
        }

        public List<ServiceType> ListServiceTypesForKiosk(string segmentId, bool callInKiosk)
        {
            var appId = QorchUserContext.AppId;
            var branchId = QorchUserContext.BranchId;

            Sql cmd = new(@"select st.*, tp.NotAvailable, tp.ServiceStartTime, tp.ServiceEndTime, tp.BreakStartTime, tp.BreakEndTime, tp.Oid TicketPool
                         from ServiceType st 
                         inner join TicketPool tp on st.Oid = tp.ServiceType and tp.Segment = @1 
                         and tp.Branch =@0 
                         left join Kiosk k on k.Oid = tp.Kiosk
                         where (tp.Kiosk is null or k.Oid = @2) 
                         and st.GCRecord is null and st.Parent is null and tp.NotAvailable != 1 
                         and (st.CallInKiosk = @3 or @3 is null) and tp.GCRecord is null order by st.SeqNo", branchId.ToString(), segmentId.ToString(), appId.ToString(), callInKiosk ? 1 : null);
            var lst = ListByCmd<ServiceType>(cmd);

            //foreach (var x in lst)
            //{0
            //    var b = x.ServiceStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
            //                         x.ServiceEndTime.TimeOfDay > DateTime.Now.TimeOfDay &&
            //                         !(x.BreakStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
            //                         x.BreakEndTime.TimeOfDay > DateTime.Now.TimeOfDay);
            //    Console.WriteLine(b);
            //}

            lst = lst.Where(x => x.ServiceStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
                                     x.ServiceEndTime.TimeOfDay > DateTime.Now.TimeOfDay &&
                                     !(x.BreakStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
                                     x.BreakEndTime.TimeOfDay > DateTime.Now.TimeOfDay)).ToList();
            return lst;
        }

        public List<ServiceType> ListChildrenServiceTypesByParent(string parentId, string segmentId)
        {
            var appId = QorchUserContext.AppId;
            var branchId = QorchUserContext.BranchId;
            Sql cmd = new(@"select st.*, tp.NotAvailable, tp.ServiceStartTime, tp.ServiceEndTime, tp.BreakStartTime, tp.BreakEndTime, tp.Oid TicketPool
                                from ServiceType st
                                inner join TicketPool tp on st.Oid = tp.ServiceType and tp.Segment = @0 and tp.Branch = @2 
                                left join Kiosk k on k.Oid = tp.Kiosk
                                where (tp.Kiosk is null or k.Oid = @3)
                                and tp.NotAvailable != 1 and tp.GCRecord is null
                                and st.GCRecord is null and st.Parent = @1 order by st.SeqNo", segmentId, parentId, branchId, appId);
            List<ServiceType> lst = ListByCmd<ServiceType>(cmd);
            lst = lst.Where(x => x.ServiceStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
                                     x.ServiceEndTime.TimeOfDay > DateTime.Now.TimeOfDay &&
                                     !(x.BreakStartTime.TimeOfDay < DateTime.Now.TimeOfDay &&
                                     x.BreakEndTime.TimeOfDay > DateTime.Now.TimeOfDay)).ToList();
            return lst;
        }

        public SegmentDto GetSegmentByPrefix(string no, bool throwIfNotFound = true)
        {
            Segment segment = null;
            var segments = Query<Segment>().Where(x => x.GCRecord == null && x.Prefix != null).ToList();
            foreach (var item in segments)
            {
                var prefixList = item.Prefix.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (prefixList.Any(x => x.StartsWith(no)))
                {
                    segment = item;
                    break;
                }
            }
            segment ??= GetDefaultSegment(throwIfNotFound);
            SegmentDto segmentDto = segment?.GetDto<SegmentDto>();
            LoggerAdapter.Debug("Segment Oid:" + segmentDto?.Oid);
            return segmentDto;
        }

        public Segment GetDefaultSegment(bool throwIfNotFound = true)
        {
            Segment sgm = Query<Segment>().FirstOrDefault(x => x.Default && x.GCRecord == null);
            if (sgm == null && throwIfNotFound)
                throw new QorchException("Default Segment Not Found", QorchErrorCodes.DefaultSegmentNotFound);
            return sgm;
        }

        public ServiceType GetDefaultServiceType(bool throwIfNotFound = true)
        {
            ServiceType svct = Query<ServiceType>().FirstOrDefault(x => x.Default && x.GCRecord == null);
            if (svct == null && throwIfNotFound)
                throw new QorchException("Default ServiceType Not Found", QorchErrorCodes.DefaultServiceTypeNotFound);
            return svct;
        }

        private ServiceType GetServiceType(string serviceTypeId)
        {
            Sql cmd = new(@"select st.* from ServiceType st where st.Oid = @0", serviceTypeId);
            ServiceType st = SingleByCmd<ServiceType>(cmd);
            return st;
        }

        public Ticket GetLastTicket(string stId, string sgmId, string tpId)
        {
            Ticket last = List<Ticket>(x => x.ServiceType == stId && x.Segment == sgmId && x.Branch == QorchUserContext.BranchId && x.Year == DateTime.Today.Year && x.DayOfYear == DateTime.Today.DayOfYear)
                            .OrderByDescending(x => x.CreatedDate)
                            .FirstOrDefault(x => x.TicketPool == tpId);
            return last;
        }

        private int GetNumberOfWaitingTickets(string stId, string sgmId)
        {
            return List<Ticket>(x => x.ServiceType == stId && x.Segment == sgmId && x.CurrentState == TicketStateEnum.Waiting && x.DayOfYear == DateTime.Today.DayOfYear && x.Year == DateTime.Today.Year && x.Branch == QorchUserContext.BranchId).Count;
        }

        private readonly static object ticketNumLock = new object();
        public (TicketDto ticket, TicketStateDto ticketState) GetNewTicket(TicketRequestDto req)
        {
            lock (ticketNumLock)
            {
                using var transaction = Cn.BeginTransactionScope();

                //if (IsSessionExpired(req.SessionOId))
                //    throw new QorchException("Ticket session is expired.", QorchErrorCodes.KioskSessionExpired);

                ServiceType svcType = GetServiceType(req.ServiceTypeId);
                string sgmName = "";

                TicketPool tp = FirstOrDefault<TicketPool>(x => x.ServiceType == req.ServiceTypeId && x.Segment == req.SegmentId && x.Oid == req.TicketPoolId && !x.NotAvailable);

                //if (tp == null)
                //    tp = FirstOrDefault<TicketPool>(x => x.ServiceType == req.ServiceTypeId && x.Segment == req.SegmentId
                //                && x.Branch == null && x.Account == QorchUserContext.AccountId && !x.NotAvailable);

                if (tp == null)
                    throw new QorchException("Ticket pool not defined for this service type", QorchErrorCodes.TicketPoolNotAvailable);

                if (!(tp.ServiceStartTime.TimeOfDay < DateTime.Now.TimeOfDay && tp.ServiceEndTime.AddSeconds(-1).TimeOfDay > DateTime.Now.TimeOfDay
                    && !(tp.BreakStartTime.TimeOfDay < DateTime.Now.TimeOfDay && tp.BreakEndTime.TimeOfDay > DateTime.Now.TimeOfDay)))
                    throw new QorchException("Ticket pool is currently unavailable", QorchErrorCodes.TicketPoolNotAvailable);

                int bekleyenSay = GetNumberOfWaitingTickets(req.ServiceTypeId, req.SegmentId);
                if (tp.MaxWaitingTicketCountControlTime.TimeOfDay > DateTime.Now.TimeOfDay)
                {

                    int waitingTicketCount = bekleyenSay;
                    if (tp.MaxWaitingTicketCount != null && tp.MaxWaitingTicketCount < waitingTicketCount)
                        throw new QorchException("MaximumWaitingTicket number is reached for this service type", QorchErrorCodes.TicketPoolMaxWaitingTicket);
                }

                Ticket lastTicket = GetLastTicket(req.ServiceTypeId, req.SegmentId, tp.Oid);
                int retNumber = tp.RangeStart;
                if (lastTicket != null)
                {
                    retNumber = lastTicket.Number + 1;
                    sgmName = lastTicket.SegmentName;
                }
                else
                {
                    Segment sgmnt = SingleById<Segment>(req.SegmentId);
                    sgmName = sgmnt.Name;
                }

                if (retNumber > tp.RangeEnd)
                {
                    if (tp.ResetOnRange)
                        retNumber = tp.RangeStart;
                    else
                        throw new QorchException("Ticket pool range exceeded", QorchErrorCodes.TicketPoolRangeExceeded);
                }
                else if (retNumber < tp.RangeStart)
                {
                    retNumber = tp.RangeStart;
                }

                //var session = GetSession(req.SessionOId);

                Ticket t = new()
                {
                    //Oid = req.SessionOId ?? Guid.NewGuid(),
                    Oid = Guid.NewGuid().ToString(),
                    ServiceTypeObj = svcType,//new ServiceType() { Oid = req.ServiceTypeId, Name = svcType.Name, Default = svcType.Default, NameLoc = svcType.NameLoc },
                    ServiceType = req.ServiceTypeId,
                    SegmentObj = new Segment() { Oid = req.SegmentId },
                    Segment = req.SegmentId,
                    Year = DateTime.Today.Year,
                    Branch = QorchUserContext.BranchId,
                    BranchName = QorchUserContext.BranchName,
                    CustomerId = req.CustomerId,
                    DayOfYear = DateTime.Today.DayOfYear,
                    Number = retNumber,
                    //t.Prefix = tp.Prefix;
                    ServiceTypeName = svcType?.Name,
                    SegmentName = sgmName,
                    CurrentState = TicketStateEnum.Waiting,
                    NumberOfWaitingTickets = bekleyenSay,
                    ToServiceType = req.ServiceTypeId,
                    LastOprTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    CreatedDateUtc = DateTime.UtcNow,
                    CreatedBy = QorchUserContext.Name,
                    TicketPool = tp.Oid,
                    CreatedByDesk = QorchUserContext.DeskId
                };


                Save(t);

                TicketState ts = new()
                {
                    Oid = Guid.NewGuid().ToString(),
                    Ticket = t.Oid,
                    TicketStateValue = TicketStateEnum.Waiting,
                    TicketOprValue = null,
                    Branch = QorchUserContext.BranchId,
                    StartTime = DateTime.Now,
                    TicketNumber = t.Number,
                    ServiceType = req.ServiceTypeId,
                    Segment = req.SegmentId,
                    SegmentName = sgmName,
                    ServiceTypeName = svcType.Name
                };
                ts.TicketObj = t;
                Save(ts);

                TicketDto ticketDto = t.GetDto<TicketDto>();

                transaction.Complete();

                //ticketDto.ImageBase64 = 
                return (ticketDto, ts.GetDto<TicketStateDto>());
            }
        }


        public List<Segment> ListSegments()
        {
            Sql cmd = new(@"select * from Segment where GCRecord is null");
            List<Segment> lst = ListByCmd<Segment>(cmd);
            return lst;
        }

        static Company c;
        public static void ClearCache()
        {
            c = null;
        }
        public Company GetCompany()
        {
            if (c == null)
            {
                Sql cmd = new(@"select * from Company where GCRecord is null limit 1");
                c = SingleByCmd<Company>(cmd);
            }
            return c;
        }

        public byte[] GetKioskBg(string webroot)
        {
            var bgPath = Path.Combine(webroot, "img", QorchClaimTypes.KioskBgImageFile);
            Sql cmd = new(@"select * from Company where GCRecord is null limit 1");
            c = SingleByCmd<Company>(cmd);
            if (c?.BgImage != null)
            {
                System.IO.File.WriteAllBytesAsync(bgPath, c.BgImage);
                return c.BgImage;
            }
            return null;
        }

        public byte[] GetDisplayBg(string webroot)
        {
            var bgPath = Path.Combine(webroot, "img", QorchClaimTypes.DisplayBgImageFile);
            Sql cmd = new(@"select * from Company where GCRecord is null limit 1");
            c = SingleByCmd<Company>(cmd);
            if (c?.BgImageDisplay != null)
            {
                System.IO.File.WriteAllBytesAsync(bgPath, c.BgImageDisplay);
                return c.BgImageDisplay;
            }
            return null;
        }


        static List<Resource> resource;
        public List<Resource> GetResources()
        {
            if (resource == null)
            {
                Sql cmd = new(@"select r.* from Resource r
                                inner join Language l on l.Oid=r.Language and l.IsDefault=1 and l.GCRecord is null
                                where r.GCRecord is null");
                resource = ListByCmd<Resource>(cmd);
            }
            return resource;
        }

        public DisplayDto GetDisplay(string kappId, bool withChildrenIfMain = true)
        {
            Sql cmd = new(@"select k.Oid KioskId, d.Name DeskName, svc.TicketNumber CurrentServiceNumber,
                            d.Oid DeskId, d.DisplayNo DisplayNo, d.ActiveUser DeskUserName, 0 IsMainDisplay
                            from Kiosk k 
                            left join Desk d on k.Oid = d.Pano and d.GCRecord is null
                            left join TicketState svc on d.Oid = svc.Desk and svc.TicketStateValue = 2 and svc.EndTime is null and svc.GCRecord is null
                            where k.GCRecord is null and k.Oid = @0 limit 1", kappId);

            DisplayDto pano = SingleByCmd<DisplayDto>(cmd);

            cmd = new(@"Select k.Oid KioskId, d.Name DeskName, kr.Icon, svc.TicketNumber CurrentServiceNumber,
                        d.Oid DeskId, d.DisplayNo DisplayNo, d.ActiveUser DeskUserName, 0 IsMainDisplay
                        from KioskRelation kr inner join Kiosk k on kr.Child = k.Oid and k.GCRecord is null
                        left join Desk d on k.Oid = d.Pano and d.GCRecord is null
                        left join TicketState svc on d.Oid = svc.Desk and svc.TicketStateValue = 2 and svc.EndTime is null and svc.GCRecord is null
                        Where kr.GCRecord is null and kr.Parent = @0", kappId);

            List<DisplayDto> childs = ListByCmd<DisplayDto>(cmd);

            if (pano != null && childs.Count > 0)
            {
                pano.IsMainDisplay = true;
                if (withChildrenIfMain)
                    pano.Children = childs;
            }

            return pano;
        }

        //private List<Pano> ListChildrenDisplays(string mainPanoId)
        //{
        //    Sql cmd = new(@"select p.*, d.Name DeskName, svc.TicketNumber TicketNumber from Pano p 
        //                    left join Desk d on p.Oid = d.Pano and d.GCRecord is null
        //                    left join TicketState svc on d.Oid = svc.Desk and svc.TicketStateValue = 2 and svc.EndTime is null and svc.GCRecord is null
        //                    inner join PanoRelation pr on pr.Child = p.Oid and pr.Parent = @0", mainPanoId);
        //    return ListByCmd<Pano>(cmd);
        //}

        public Desk GetFirstAutocallDesk()
        {
            return _deskDa.GetFirstDeskToAutocall(null);
        }
    }
}
