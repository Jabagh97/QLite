using Newtonsoft.Json;

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
        public string padding;
        public string shape;
        public string margin;
        public string border;
        public string fontColor;
        public DesCompDataKeypadInput() : base()
        {
        }
    }

    public class DesCompDataServiceButton : DesCompData
    {
        public string ButtonText;
        public Guid ServiceTypeOid;
        public string padding;
        public string shape;
        public string margin;
        public string border;
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
        public string padding;
        public string shape;
        public string margin;
        public string border;
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
        public string padding;
        public string shape;
        public string margin;
        public string border;
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

        public string GenCompType = "";



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

        public string padding;
        public string shape;
        public string margin;
        public string border;



        public string Bold;

        public string FontFamily;

        public string FontStyle;

        public string FontWeight;
        public string FontSize;


        public string LineHeight;
        public string LetterSpacing;
        public string FontColor;
        public string TextAlignHo;
        public string TextAlignVE;
        public DesCompDataIDInput() : base()
        {
        }
    }

    public class DesCompDataText : DesCompData
    {
        public string Text;

        public int CtxIndex;
        public bool Localized;

        public string TextColor;

        public string Bold;

        public string FontFamily;

        public string FontStyle;

        public string FontWeight;
        public string FontSize;


        public string LineHeight;
        public string LetterSpacing;
        public string FontColor;
        public string TextAlignHo;
        public string TextAlignVE;
        public bool DataAware;
        public bool SlideAnimation;

        public DesCompDataText() : base()
        {
        }
    }

    public class DesCompDataLang : DesCompData
    {

        public string LangCode;
        public string padding;
        public string shape;
        public string margin;
        public string border;
        public string LogoURL;
        public string LanguageName;

        public DesCompDataLang() : base()
        {
        }
    }


    public enum WfButtonType
    {
        NEXT,
        BACK,
        CANCEL
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
    }
}
