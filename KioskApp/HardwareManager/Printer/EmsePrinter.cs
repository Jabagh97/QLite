namespace Quavis.QorchLite.Hwlib.Printer
{
    using CoreHtmlToImage;
    using Microsoft.Extensions.Configuration;
    using QLite.Data.CommonContext;
    using QLite.Data.Dtos;
    using QLite.Kio;
    using Quavis.QorchLite.Hwlib.Hardware;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static QLite.Data.Models.Enums;

    public class EmsePrinter : IPrinter
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
        }

        public CuttingType CuttingType { get; set; } = CuttingType.FullCut;
        public bool SaveImage { get; set; }
        public bool TestMode { get; set; }
        public PrinterMaxPixel PrinterMaxPixelValue { get; set; } = PrinterMaxPixel.Pixel448;


        public void Initialize(object settings)
        {
            var prnSettings = settings as VCPrinterEmseSettings;
            Log.Debug("initializing emsePrinter");

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



        #region read status

        public void CheckPrinterStatus()
        {
            if (Device.Connected != true)
                return;
            Device.ReadPrinterStatus();
        }



        #endregion



        public void Send(string html)
        {
            Log.Debug("emsequeprinter Send");


            byte[] imgData;
            var converter = new HtmlConverter();

            if (!html.StartsWith("<html>"))
            {
                html = "<html><head><meta charset=\"utf-8\"/></head><body> " + html + " </body></html>";
            }
            imgData = converter.FromHtmlString(html, width: 200, format: ImageFormat.Jpg);


            Log.Debug("emsequeprinter image created");
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

        public void InitializeIfNeeded()
        {
            VCPrinterEmseSettings settings = new VCPrinterEmseSettings()

            {
                VendorId = "0x10c4",
                ProductId = "0x82CD",
                SaveImage = false,
                TestMode = false,
                PrinterMaxPixel = PrinterMaxPixel.Pixel448,
                CuttingType = CuttingType.HalfCut

            };

            if (_epu == null)
            {
                Initialize(settings);
            }
        }

    }
}
