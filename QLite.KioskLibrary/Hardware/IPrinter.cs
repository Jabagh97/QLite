using Quavis.QorchLite.Data.Dto;
using Quavis.QorchLite.Hwlib.Call;
using Quavis.QorchLite.Hwlib.Display;
using System;

namespace Quavis.QorchLite.Hwlib.Hardware
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
