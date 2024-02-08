using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Models
{
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
            Sequential = 0,
         
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
        public enum WfStep
        {
            PageServiceTypeSelection = 0,
            PageSelectLanguage = 1,
            PageSegmentSelection = 2,
            PagePrint = 3,
            PageNationalId = 4,
            PageInputType = 5,
            PageGeneric = 6,
            PageCustomerNo = 7,

            None = 8,

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
    }

}
