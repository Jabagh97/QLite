using QLite.Dto;
using Quavis.QorchLite.Hwlib.Call;
using Quavis.QorchLite.Hwlib.Display;
using Quavis.QorchLite.Hwlib.Hardware;
using System;

namespace QLite.Kio
{

    public interface ICallDevice
    {
        RealDeviceBase Device { get; }

        void Initialize(object settings);
        void Send(InterfacePackage data);
        HwStatusDto GetHwStatus();

        Type SettingsType { get; }

    }

    public interface IPrinter
    {
        RealDeviceBase Device { get; }

        void Initialize(object settings);
        void Send(string html);
        HwStatusDto GetHwStatus();

        Type SettingsType { get; }

    }

    public interface IDisplay
    {
        RealDeviceBase Device { get; }
        void Initialize(object settings);

        void Send(QueNumData qnumDataObject);
        
        HwStatusDto GetHwStatus();

        Type SettingsType { get; }

    }

}
