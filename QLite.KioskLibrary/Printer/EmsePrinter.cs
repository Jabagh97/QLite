namespace Quavis.QorchLite.Hwlib.Printer
{
    using CoreHtmlToImage;
    using Microsoft.Extensions.Configuration;
    using QLite.Dto;
    using QLite.Kio;
    using Quavis.QorchLite.Hwlib.Hardware;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class EmsePrinter: IPrinter
    {

        public EmseUsbPrinterDevice Device;
        EmsePrintUtil _epu;
        RealDeviceBase IPrinter.Device => Device;

        public event EventHandler<bool?> HardwareConnStatusChanged;


        public Type SettingsType => typeof(VCPrinterEmseSettings);

        List<ErrorCodes> HardErrors = new List<ErrorCodes>() { ErrorCodes.RollPaperEmpty, ErrorCodes.PrinterHeadOpenMechanical };
        public HwStatusDto GetHwStatus()
        {
            if (TestMode)
            {
                return new HwStatusDto
                {
                    Device = QLDevice.Printer,
                    Connected = true,
                    Ok = true
                };
            }

            var connected = Device.Connected != false;
            var notok = !connected || Device.errorCodeList.Any(x => HardErrors.Contains(x));
            return new HwStatusDto
            {
                Device = QLDevice.Printer,
                Connected = connected,
                Status = Device.errorCodeList.Select(x => x.ToString()).ToList(),
                Ok = !notok
            };
            
        }

        IConfiguration _config;
        public EmsePrinter(IConfiguration config, EmseUsbPrinterDevice dev)
        {
            _config = config;
            Device = dev;
            //RealDeviceComManager.RealDevices.Add(Device);
        }

        public CuttingType CuttingType { get; set; } = CuttingType.FullCut;
        public bool SaveImage { get; set; }
        public bool TestMode { get; set; }
        public PrinterMaxPixel PrinterMaxPixelValue { get; set; } = PrinterMaxPixel.Pixel448;


        public void Initialize(object settings)
        {
            var prnSettings = settings as VCPrinterEmseSettings;
           // LoggerAdapter.Debug("emsePrinter initialize");

            var vid = "0x10C4";
            var pid = "0x82CD";


            TestMode = prnSettings.TestMode;
            CuttingType = prnSettings.CuttingType;
            PrinterMaxPixelValue = prnSettings.PrinterMaxPixel;
            SaveImage = prnSettings.SaveImage;
            vid = prnSettings.VendorId;
            pid = prnSettings.ProductId;


            _epu = new EmsePrintUtil(Device, CuttingType, PrinterMaxPixelValue);

            if (Device.Initialize(vid, pid))
            {
                //HardwareConnStatusChanged?.Invoke(this, true);
            }
            //else if (ListenData)
            //    SingleDevice.StartListeningDeviceData(RealDevice_DeviceDataEvent);

            StartCheckDeviceStatus(CancellationToken.None);

            Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;
        }

        private void StartCheckDeviceStatus(CancellationToken ct)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (ct.IsCancellationRequested)
                        break;
                    CheckPrinterStatus();
                    Thread.Sleep(4000);
                }
            }, ct);
        }

        private void Device_DeviceConnectionEvent(object sender, bool? e)
        {
            HardwareConnStatusChanged?.Invoke(this, e);
        }



       

        #region overrides

        //protected override Type SettingsType => typeof(VCPrinterEmseSettings);

        //public override Dictionary<string, string> GetSettings()
        //{
        //    var dic = base.GetSettings();

        //    dic.Add("Cutting Type", CuttingType.ToString());
        //    dic.Add("Max Pixel", PrinterMaxPixelValue.ToString());
        //    return dic;
        //}



        //protected override bool InitializeRealDevice(string realDeviceName, object settingsObj)
        //{
        //    var res = base.InitializeRealDevice(realDeviceName, settingsObj);
        //    if (res)
        //        PrinterRd.PrinterErrorEvent += this.Printer_PrinterErrorEvent;
        //    _epu = new EmsePrintUtil(SingleDevice, CuttingType, PrinterMaxPixelValue);
        //    return res;
        //}

        //protected override VCBaseSettings ConfigVCForAddionalSettings(VCBaseSettings settings)
        //{
        //    var settings2 = settings as VCPrinterEmseSettings;
        //    CuttingType = settings2?.CuttingType ?? CuttingType.FullCut;
        //    PrinterMaxPixelValue = settings2?.PrinterMaxPixel ?? PrinterMaxPixel.Pixel448;
        //    if (settings2 != null)
        //    {
        //        SaveImage = settings2.SaveImage;
        //    }
        //    return settings2;
        //}
        #endregion

        private void Printer_PrinterErrorEvent(object sender, List<ErrorCodes> e)
        {
            //SetCussDeviceStatusByCustomStatusList(StatusCodes.OK);
        }




        #region read status

        public void CheckPrinterStatus()
        {
            if (Device.Connected != true)
                return;
            Device.ReadPrinterStatus();
        }

        //protected override void SetCussDeviceStatusByCustomStatusList(StatusCodes defStatus)
        //{
        //    if (PrinterRd.errorCodeList.Contains(ErrorCodes.PrinterHeadOpenMechanical) || PrinterRd.errorCodeList.Contains(ErrorCodes.PaperJam))
        //        SetHardwareStatus(StatusCodes.HARDWAREERROR);
        //    else if (PrinterRd.errorCodeList.Contains(ErrorCodes.RollPaperEmpty))
        //        SetHardwareStatus(StatusCodes.MEDIAEMPTY);
        //    else if (PrinterRd.errorCodeList.Contains(ErrorCodes.RollPaperDecreased))
        //        SetHardwareStatus(StatusCodes.MEDIALOW);
        //    else
        //        SetHardwareStatus(defStatus);
        //}

        #endregion




        public void Send(string html)
        {
            //LoggerAdapter.Debug("emsequeprinter Send");


            byte[] imgData;
            var converter = new HtmlConverter();

            if (!html.StartsWith("<html>"))
            {
                html = "<html><head><meta charset=\"utf-8\"/></head><body> " + html + " </body></html>";
            }
            imgData = converter.FromHtmlString(html, width: 150, format: ImageFormat.Jpg);


            //LoggerAdapter.Debug("emsequeprinter image created");
            using (var ms = new MemoryStream(imgData))
            {
                var bitmap = new Bitmap(ms);
                if (SaveImage)
                {
                    bitmap.Save("lastImage.jpg");
                }
                if (!TestMode)
                {
                    _epu.TryToPrint(bitmap);
                }
            }
        }
    }
}
