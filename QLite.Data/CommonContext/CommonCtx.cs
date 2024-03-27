using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace QLite.Data.CommonContext
{
    public class CommonCtx
    {

        public static IConfiguration Config { get; set; }
        public static string Env { get; set; } = "Production";
        public static global::Autofac.ILifetimeScope Container { get; set; }
        public static string KioskHwId { get; set; }

        public static Guid CurrentLanguage { get; set; }

        public static List<Resource> Resources { get; set; }

        public static List<Language> Languages { get; set; }


    }
}
