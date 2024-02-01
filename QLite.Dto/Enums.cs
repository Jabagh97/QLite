using QLite.Dto.Kapp;
using System;
using static QLite.Dto.Enums;

namespace QLite.Dto
{

    public static class QorchClaimTypes
    {
        public const string KioskBgImageFile = "kioskbg.jpg";
        public const string DisplayBgImageFile = "displaybg.jpg";

        public const string SessionIdType = "SessionId";

        public const string DeskIdClaimType = "DeskId";
        public const string DeskNameClaimType = "DeskName";
        public const string BranchIdClaimType = "BranchId";
        public const string BranchNameClaimType = "BranchName";

        public const string DeskPanoIdClaimType = "DeskPanoId";
        public const string DeskDisplayNoClaimType = "DeskDisplayNo";        

        public const string ClientTypeClaimType = "ClientType";
        //public const string KioskKmsIdClaimType = "KioskKmsId";  // KMS tarafında Id bilgisi
        public const string KioskIdClaimType = "KioskId";  // display veya ticket kiosku olabilir. Dispay de kiosk olmuş oluyor neticede.. Db ID si 
        public const string ClientName = "ClientName";  // ws den bağlanan her kimse loglardan kolay okunabilen bir bilgi
        public const string KioskHwIdClaimType = "KioskHwId";
        public const string UserMacroIdClaimType = "UserMacroId";
        public const string UserMacroNameClaimType = "UserMacroName";

        public const string UserIdClaimType = "UserId";  //İnsan kullanıcıs. Desk veya kiosk tan girmiş de olabilir. UserName pws girip nerden girerse girsin.
        public const string UserNameClaimType = "UserName";  //İnsan kullanıcıs. Desk veya kiosk tan girmiş de olabilir. UserName pws girip nerden girerse girsin.
        public const string UserRoleIdClaimType = "UserRoleId";
        public const string UserMacroChangeClaimType = "CanChangeMacro";
        //public const string UserRoleNameClaimType = "UserRoleName";



        public const string CultureClaimtype = "Culture";
        public const string AdminClaimType = "Admin";

        public const string QorchAppTypeClaimType = "QorchAppType";
        public const string AppCodeClaimType = "AppCode";
        public const string AppIdClaimType = "AppId";
        public const string DeskAppTypeClaimType = "DeskAppType";


    }


    public enum QLDevice
    {
        Printer = 0,
        Display = 1,
        Terminal = 2,
    }


    //public enum MobilLoginState
    //{
    //    Success = 0,
    //    Unsuccess = 1,
    //    NoDesk = 2
    //}

    public class Enums
    {
        public enum QorchErrorCodes
        {
            TicketPoolNotAvailable,
            TicketPoolMaxWaitingTicket,
            TicketPoolRangeExceeded,
            TransferNotAllowedToSelf,
            TransferNotAllowedForTicketState,
            KioskSessionExpired,
            DefaultSegmentNotFound,
            DefaultServiceTypeNotFound,
            TicketNotFound,
            DeskIsTooActiveToLogin,
            TransferNotAllowedToUnknownTarget
        }



        public const string DEFAULT_BRANCH_NAME = "Merkez";
        public const string DEFAULT_BRANCH_CODE = "MRK";


        public enum QorchBusinessRole
        {
            Admin = 100,
            BranchAdmin = 20,
            Desk = 10
        }

        public enum UIProcessType
        {
            TcNoPrintTicket,
            ServiceTypePrintTicket,
            WelcomeTouchPrintTicket,
            WelcomeTouchServiceTypePrintTicket,
            DisplayContent
        }

        public enum ProcessState
        {
            Initializing,
            WaitingIdFromUI,
            WaitingServiceType,
            WaitingTouch,
            Printing
        }


        public enum WSNotificationEvents
        {
            ALL,
            WSCONNECTED,
            WSRECONNECTED,
            WSDISCONNECTED,
            WSTICKETCREATED,
            WSSERVICECREATED,
            WSDESKONLINE,
            WSDESKPAUSE,
            WSDESKOFFLINE,
            WSDESKBUSY,
            KIOSKSTATEINFO
        }

        public enum DesignableViewType
        {
            KioskGetServiceType,
            KioskGetTouch,
            KioskGetId,
            KioskPrinting,
            PanoIndex
        }

        public enum BusinessRole
        {
            RootDirector = 1,
            BranchDirector = 2,
            Desk = 3
        }

        public enum CustomerIdType
        {
            None,
            CustomerNo,
            NationalId,
            CardNo,
        }


        public enum DeskActivityStatus
        {
            Closed = 0,
            Open = 1,
            Paused = 2,
            Busy = 3
        }

        public enum TicketStateEnum
        {
            Waiting = 0,
            Waiting_T = 1,
            Service = 2,
            Park = 3,
            Final = 4
        }

        public enum TicketOprEnum
        {
            Call = 0,
            ServiceEnd = 1,
            Park = 2,
            Transfer = 3,
            Cancel = 4,
            SendToWaiting = 5
        }

        public enum MacroType
        {
            //WaitingTime = 0, //waitingTime iptal sequence la yapılabiliyormuş. enum dursun oransal falan yaparız sonra.
            Sequential = 1
        }

        public enum MeNotothersAll
        {
            Notothers = 0,
            Me = 1,
            All = 2
        }

        public static string GetControllerName(string ct)
        {
            var ctt = ParseClientType(ct);
            if (ctt != null)
                return GetControllerName(ctt.Value);
            else
                return null;
        }

        public static string GetControllerName(WebSocketClientType ct)
        {
            switch (ct)
            {
                case WebSocketClientType.Display:
                case WebSocketClientType.MainDisplay:
                    return "Pano"; // "Pano";
                case WebSocketClientType.Kiosk:
                    return "Kiosk"; //"Kiosk";
                default:
                    return null;
            }
        }

        public static WebSocketClientType? ParseClientType(string ct)
        {
            if (Enum.TryParse(ct, out WebSocketClientType res))
                return res;
            else
                return null;
        }

        public static WebSocketClientType? GetWsClientType(string ctStr, bool throwIfNotParsed = true)
        {
            if (Enum.TryParse(ctStr, out WebSocketClientType ct))
            {
                return ct;
            }
            if (throwIfNotParsed)
            {
                throw new ArgumentException("clientType can not be parsed");
            }

            return null;
        }

        public enum KioskType
        {
            Kiosk = 0,
            Display = 1,
        }

        
        public enum HesResultEnum
        {
            RISKLESS = 0,
            RISKY = 1,
            ERROR = 2
        }

        public enum TicketCallType
        {
            Normal = 0,
            Autocall = 1,
            Definitive = 2
        }

        public enum DeskAppType
        {
            Web = 0,
            Desktop = 1,
            Mobile = 2,
            Physical = 3
        }

    }
}
