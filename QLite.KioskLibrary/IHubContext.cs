using QLite.Dto;

namespace Quavis.QorchLite.Hwlib
{
    public interface IHwHubContext
    {
        void HwEvent(KioskHwStatusDto hwStatus);
    }
}
