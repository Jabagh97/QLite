using Microsoft.Extensions.Configuration;
using NLog;
using System;

namespace Quavis.QorchLite.Common
{
    public class CommonCtx
    {

        public static IConfiguration Config { get; set; }
        public static string Env { get; set; } = "Production";
        public static global::Autofac.ILifetimeScope Container { get; set; }
        public static string KioskHwId { get; set; }
    }
}
