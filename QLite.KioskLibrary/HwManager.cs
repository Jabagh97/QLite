using Autofac;
using HidSharp;
using HidSharp.Utility;
using Microsoft.Extensions.Configuration;
using Quavis.QorchLite.Common;
using Quavis.QorchLite.Data.Dto;
using Quavis.QorchLite.Hwlib.Display;
using Quavis.QorchLite.Hwlib.Hardware;
using Quavis.QorchLite.Hwlib.Printer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace Quavis.QorchLite.Hwlib
{
    public class HwManager
    {
        IPrinter _printer;
        IDisplay _display;
        ICallDevice _call;
        IHwHubContext _hc;

        public HwManager(IHwHubContext hc)
        {
            _hc = hc;
        }


        public void Print(string html)
        {
            _printer.Send(html);
        }

        public void Display(QueNumData data)
        {
            if (_display == null)
                return;

            _display.Send(data);
        }

        void CreateDevices()
        {

            var printerSetting = CommonCtx.Config.GetSection("devices:printer");
            if (printerSetting.Exists())
            {
                LoggerAdapter.Debug("creating printer device");

                _printer = CommonCtx.Container.Resolve<IPrinter>();

                var settings = printerSetting.Get(_printer.SettingsType);
                _printer.Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;

                _printer.Initialize(settings);
                RealDevices.Add(_printer.Device);

                if (_printer.Device is EmseUsbPrinterDevice prnRD)
                    prnRD.PrinterErrorEvent += Device_PrinterErrorEvent;
            }
            else 
            {
                LoggerAdapter.Warning("No printer device settings found");
            }

            var callSettings = CommonCtx.Config.GetSection("devices:call");
            if (callSettings.Exists())
            {
                LoggerAdapter.Debug("creating call device");

                _call = CommonCtx.Container.Resolve<ICallDevice>();

                var settings = callSettings.Get(_call.SettingsType);
                _call.Device.DeviceConnectionEvent += Device_DeviceConnectionEvent;
                _call.Initialize(settings);
                RealDevices.Add(_call.Device);
            }
            else
                LoggerAdapter.Warning("No call device settings found");


            var dt = CommonCtx.Config.GetValue<string>("devices:display:DType", null);
            if (dt != null)
            {
                LoggerAdapter.Debug("creating display device");

                _display = CommonCtx.Container.ResolveNamed<IDisplay>(dt);
                var settings = CommonCtx.Config.GetSection("devices:display").Get(_display.SettingsType);
                _display.Device.DeviceConnectionEvent += Device_DeviceConnectionEvent; ;
                _display.Initialize(settings);
                RealDevices.Add(_display.Device);
            }
            else
            {
                LoggerAdapter.Warning("No display device settings found");
            }
        }

        private void Device_DeviceConnectionEvent(object sender, bool? e)
        {
            _hc.HwEvent(GetKioskHwStatus());
        }

        public void InitHarware()
        {
            LoggerAdapter.Debug($"InitHardware");

            CreateDevices();
            HidSharpDiagnostics.EnableTracing = true;
            HidSharpDiagnostics.PerformStrictChecks = true;

            list.Changed += this.List_Changed;

            var allDeviceList = list.GetAllDevices().ToList();
            foreach (var item in allDeviceList)
            {
                LoggerAdapter.Info($"Devices fsName:{item.GetFileSystemName()} name:{item.DevicePath}");
            }

            GetDeviceComStates();
        }




        public KioskHwStatusDto GetKioskHwStatus()
        {
            var hws = new KioskHwStatusDto();
            hws.Ok = true;

            var printerHwStatus = _printer.GetHwStatus();
            if (!printerHwStatus.Ok)
            {
                hws.Ok = false;
                hws.AddHwStatus(printerHwStatus);
            }

            if (_display != null)
            {
                var dispHwStatus = _display.GetHwStatus();
                if (!dispHwStatus.Ok)
                {
                    hws.Ok = false;
                    hws.AddHwStatus(dispHwStatus);
                }
            }

            if (_call != null)
            {
                var callHwStatus = _call.GetHwStatus();
                if (!callHwStatus.Ok)
                {
                    hws.Ok = false;
                    hws.AddHwStatus(callHwStatus);
                }
            }

            //başka cihaz olursa burdan devam            

            return hws;


        }

        private void Printer_HardwareConnStatusChanged(object sender, bool? e)
        {

        }

        private void Device_PrinterErrorEvent(object sender, List<ErrorCodes> e)
        {
            _hc.HwEvent(GetKioskHwStatus());

        }


        public static List<RealDeviceBase> RealDevices = new List<RealDeviceBase>(); // global bir collection tüm cihazlar var

        DeviceList list = DeviceList.Local;


        private void List_Changed(object sender, DeviceListChangedEventArgs e)
        {
            GetDeviceComStates();
        }

        private void GetDeviceComStates()
        {
            LoggerAdapter.Debug("RealDeviceComManager, Device list changed.");

            var allSerials = list.GetSerialDevices().ToList();
            try
            {
                LoggerAdapter.Debug("SerialDevices:" + string.Join(",", allSerials.Select(x => x.GetFriendlyName())));
                LoggerAdapter.Debug("HIDDevices:" + string.Join(",", list.GetHidDevices().Select(x => x.GetFriendlyName())));
            }
            catch (Exception ex)
            {
                LoggerAdapter.Debug(ex.Message);
            }

            var searialPorts = SerialPort.GetPortNames();
            var serials = RealDevices.Where(x => x.Port != string.Empty && searialPorts.Contains(x.Port)).ToList();
            var hids = list.GetHidDevices().ToList().Where(s => RealDevices.Where(x => x.VID != 0 && x.PID != 0).Any(x => x.PID == s.ProductID && x.VID == s.VendorID)).ToList();

            foreach (var device in RealDevices)
            {
                if (!string.IsNullOrEmpty(device.Port))
                {
                    var exist = serials.Any(x => x.Port == device.Port);
                    device.OnDeviceConnectionEvent(exist ? null : exist);
                }
                else if (device.VID != 0 && device.PID != 0)
                {
                    device.OnDeviceConnectionEvent(hids.Any(x => device.PID == x.ProductID && device.VID == x.VendorID));
                }
            }
        }
    }
}
