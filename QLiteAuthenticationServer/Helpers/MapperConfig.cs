using AutoMapper;
using QLite.Data.Models.Auth;
using QLite.Data.ViewModels.Auth;

namespace QLiteAuthenticationServer.Helpers
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ApplicationUser, ApplicationUserViewModel>();
            CreateMap<ApplicationUser, ApplicationUserCreateEditModel>();
        }
    }

}
