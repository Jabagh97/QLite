using Quavis.QorchLite.Data.Dto;

namespace Quavis.QorchLite.Hwlib
{
    public interface IHwHubContext
    {
        void HwEvent(KioskHwStatusDto hwStatus);
    }
}
