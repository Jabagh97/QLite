using Microsoft.Extensions.Configuration;

namespace QLite.Data.CommonContext
{
    public class CommonCtx
    {

        public static IConfiguration Config { get; set; }
        public static string Env { get; set; } = "Production";
        public static global::Autofac.ILifetimeScope Container { get; set; }
        public static string KioskHwId { get; set; }
    }
}
