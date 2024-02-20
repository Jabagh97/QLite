using QLite.Data.Dtos;

namespace QLite.Data.CommonContext
{
    public interface IHwHubContext
    {
        void HwEvent(KioskHwStatusDto hwStatus);
    }
}
