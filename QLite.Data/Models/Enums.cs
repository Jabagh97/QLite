using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Models
{
    public class Enums
    {

       
        public enum KioskType
        {
            Kiosk,
            Display,
        }


        public enum WorkFlowType
        {
            [Description("Welcome -> Segments -> Services")]
            WelcomeSegmentService,

            [Description("Segments -> Services")]
            SegmentService,

            [Description("Services")]
            OnlyServices
        }


        public enum QLDevice
        {
            Printer = 0,
            Display = 1,
            Terminal = 2,
        }

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
            ArgumentNullException,
        }

        public enum QorchAppTypes
        {
            TicketPrinterApp = 0,
            DisplayApp = 1,
            DeskApp = 3
        }





        public enum QorchBusinessRole
        {
            Admin = 100,
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

        public enum ToDesk
        {
            NotSpecified = 0,
            OnlyForSelectedDesk = 1,
            AllDesks = 2
        }

        public enum MacroType
        {
            [Description("Sequential")]

            Sequential = 0,
            [Description("Proportional")]

            Proportional = 1,
            //TestColor = 1,
            //WaitingTime = 1, //waitingTime iptal sequence la yapılabiliyormuş. enum dursun oransal falan yaparız sonra.
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


        public enum Parameter
        {
            PrimaryColor = 0,
            SecondaryColor = 1,
            autocall = 2,
            customerNoInput_timeout = 3,
            error_timeout = 4,
            nationalIdInput_timeout = 5,
            print_timeout = 6,
            serviceTypeSelection_timeout = 7,
            steptimeout = 8,
            TextColor = 9,


        }
        public enum Step
        {
            ServiceTypeSelection = 0,
            SegmentSelection = 2,
            PagePrint = 3,
            DisplayScreen = 4,
            WelcomePage = 9

        }

        public enum MeNotothersAll
        {
            NotSpecified = 0,
            OnlyForSelectedDesk = 1,
            AllTerminals = 2
        }



        public enum MobilLoginState
        {
            Success = 0,
            Unsuccess = 1,
            NoDesk = 2
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

        public enum KappErrorCodes
        {
            ClientError,
            ServerError,
            UiTimeout,
            PlatformDeviceNotFound,
            WorkflowError,
            CancelledByUser,
            UnexpectedError,
            DataPresentError,
            EmergencyStop,
            IntrusionSafety,
            DeviceTimeout,
            RetryCountExceed,
            PrintError,
            DataMissingError,
            PlatformDirValError,
            PlatformDirTimeoutError,
            DeviceDirError,
            NetworkError,
            ServiceSqlAccessError,
            DeserializingError,
            HttpError,
            DirValidationError,
            CussAppProcessError,
            UserNotAuthenticated,
            NotFound,
            GettingClientSecretError,
            DbConnManError,
            MissingConfig,
            KioskNotFound,
            WorkflowConfigError,
            WfEvalError,
            ServiceTypeNotFound,
        }

        public enum WebSocketClientType
        {
            Display,
            MainDisplay,
            Kiosk,
            User,
            Admin
        }

        public enum SubStates
        {
            IdleStep,
            Default,
            EnableFromMediaOutput,
            DisableFromMediaOutput,
            OfferFromFeeder,

        }

        public enum PectabSettingsType
        {
            Pectab,
            Template,
            Logo,
            DataStream
        }

        public enum KioskActivityStatus
        {
            Active,
            Available,
            UnAvailable
        }
    }

}
