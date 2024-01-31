
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quavis.QorchLite.Common
{

    public class LoggerAdapter
    {
        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
        static ILogger _logger;

        public static void Debug(string log)
        {
            if (_logger == null)
                Console.WriteLine(log);
            _logger?.Debug(log);

            
        }

        public static void Info(string log)
        {
            if (_logger == null)
                Console.WriteLine(log);
            _logger?.Info(log);
        }

        public static void Warning(string log)
        {
            if (_logger == null)
                Console.WriteLine(log);

            _logger?.Warn(log);
        }

        public static void Error(Exception ex, string msg)
        {
            if (_logger == null)
                Console.WriteLine(ex);

            _logger?.Error(ex, msg);
        }

        public static void Error(Exception ex)
        {
            if (_logger == null)
                Console.WriteLine(ex);
            _logger?.Error(ex);
        }


        public static  void Error(string log)
        {
            if (_logger == null)
                Console.WriteLine(log);
            _logger?.Error().Message(log);

        }

    }
}
