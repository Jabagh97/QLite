namespace KioskApp.HardwareManager.Display.Settings
{
    public class VCBaseSettings
    {
        public string DummyDeviceData { get; set; }
        public string RealDeviceType { get; set; }
        public string DisplayName { get; set; }


        public bool MultipleRealDevice { get; set; }

        public virtual object RealDeviceSettingsO { get; }
    }

    public class VCBaseSettings<T> : VCBaseSettings
    {
        public T RealDeviceSettings { get; set; }

        public override object RealDeviceSettingsO => RealDeviceSettings;
    }
}
