using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;

namespace QLite.DesignComponents
{
    public class DesPageData
    {
        //other properties of the whole target page, size, bg image, etc.
        public string Name
        {
            get; set;
        }

        public string PosY
        {
            get; set;
        }

        public string Width
        {
            get; set;
        }
        public string Height
        {
            get; set;
        }
        public string BgImageUrl
        {
            get; set;
        }

        public string BackGroundColor
        {
            get; set;
        }
        public string CustomCss
        {
            get; set;
        }
        public int PageTimeOut
        {
            get; set;
        }
        public List<DesCompData> Comps
        {
            get; set;
        }
    }





    [JsonConverter(typeof(DesCompJsonConverter))]
    public class DesCompData
    {
        public DesCompData()
        {
            TypeInfo = this.GetType().FullName;
        }

        public string Id
        {
            get; set;
        }

        public string DesCompType
        {
            get; set;
        }
        public string PosX
        {
            get; set;
        }
        public string PosY
        {
            get; set;
        }

        public string Width
        {
            get; set;
        }

        public string Height
        {
            get; set;
        }

        public string TypeInfo
        {
            get; set;
        }
        public string BackGroundColor
        {
            get; set;
        }

        public string BgImageUrl
        {
            get; set;
        }

        public string fileName
        {
            get; set;
        }

        public string CustomCss
        {
            get; set;
        }
    }


    public class DesCompDataServiceSelection : DesCompData
    {
        public DesCompDataServiceSelection() : base()
        {
        }
        public int ButtonInterval
        {
            get; set;
        }
    }

    public class DesCompDataWfButton : DesCompData
    {
        public string ButtonText;
        public WfButtonType BtnType;

        public DesCompDataWfButton() : base()
        {
        }
    }

    public class DesCompDataKeypadInput : DesCompData
    {
       
        public DesCompDataKeypadInput() : base()
        {
        }
    }

    public class DesCompDataServiceButton : DesCompData
    {
        public string ButtonText;
        public Guid ServiceTypeOid;
        public bool Localized;
        public bool PopUp;
        public bool Bounce;


        public string ButtonValue;

        public DesCompDataServiceButton() : base()
        {
        }
    }

    public class DesCompDataSegment : DesCompData
    {
        public string ButtonText;
        public Guid SegmentID;
        public bool Localized;
        public bool Bounce;


        public string ButtonValue;

        public DesCompDataSegment() : base()
        {
        }
    }
    public class DesCompDataAppointment : DesCompData
    {
        public string ButtonText;
        public AppointmentOperation Operation;
        public bool Localized;
        public bool Bounce;


        public string ButtonValue;

        public DesCompDataAppointment() : base()
        {
        }
    }

    public class DesCompDataGenericHtml : DesCompData
    {

        public string ButtonText;
        public string fileURL;
        public long? fileSize;
        public string YoutubeUrl;
        public string LocalUrl;

        public HtmlCompType GenCompType;



        public DesCompDataGenericHtml() : base()
        {
        }

    }

    public class DesCompDataFrame : DesCompData
    {
        public string DesignId;
        public bool SlideAnimation;
    }

    public class DesCompDataIDInput : DesCompData
    {

      
        public DesCompDataIDInput() : base()
        {
        }
    }

    public class DesCompDataText : DesCompData
    {
        public string Text;

        public int CtxIndex;
        public bool Localized;
        public bool DataAware;
        public bool SlideAnimation;
        public TicketInfoType InfoType;

        public DesCompDataText() : base()
        {
        }
    }

    public class DesCompDataLang : DesCompData
    {

        public string LangCode;
        public string LanguageName;
        public Guid LangID;

        public DesCompDataLang() : base()
        {
        }
    }

    public enum TicketInfoType
    {
        ServiceCode,
        Number,
        WaitingTickets,
        ServiceTypeName,
        Segment
    }

    public enum WfButtonType
    {
        NEXT,
        BACK,
        CANCEL
    }

    public enum HtmlCompType
    {
        Image,
        LocalVideo,
        YoutubeVideo,
        Text,
        Date,
        Layout
    }
    public enum AppointmentOperation
    {
        Appointment,
        NoAppointment,
    }
    public enum SysFuncs
    {
        Time12,
        Date,
        Time24
    }

    public class DesPageDataViewModel
    {
        public string DesPageDataJson { get; set; }
        public string DesignImage { get; set; }
    }

    public class HomeAndDesPageDataViewModel
    {
        public string KioskHwId { get; set; }
        public DesPageData DesPageData { get; set; }

       

    }

    public class TicketAndDesPageDataViewModel
    {
        public Ticket Ticket { get; set; }
        public DesPageData DesPageData { get; set; }

      

    }
    public class SegmentsAndDesignModel
    {
        public DesPageData DesignData { get; set; }
        public List<SegmentDto> Segments { get; set; }

      
    }
    public class ServicesAndDesignModel
    {
        public DesPageData DesignData { get; set; }
        public List<ServiceTypeDto> Services { get; set; }

    
    }
}
