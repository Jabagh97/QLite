
using Quavis.QorchLite.Data.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Quavis.Kapp.Data.Dto.ErrorHandling
{
    [Serializable]
    public class KappException : Exception
    {
        public string ErrorCode { get; set; }
        public Func<Task<string>> UnavChecker { get; private set; }

        public bool FinalizingError { get; set; }

        public KappException()
        {
        }

        public KappException(string message) : base(message)
        {
        }

        public static KappException NewKappException(string msg, KappErrorCodes errCode)
        {
            var kapexp = new KappException(msg);
            kapexp.ErrorCode = errCode.ToString();
            return kapexp;
        }

        public KappException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public KappException(string message, string errorCode, Func<Task<string>> unavChecker) : base(message)
        {
            UnavChecker = unavChecker;
            ErrorCode = errorCode;
        }

        public KappException(string message, KappErrorCodes errorCode, Func<Task<string>> unavChecker) : base(message)
        {
            UnavChecker = unavChecker;
            ErrorCode = errorCode.ToString();
        }

        public KappException(string message, Exception innerException, KappErrorCodes errorCode, Func<Task<string>> unavChecker) : base(message, innerException)
        {
            UnavChecker = unavChecker;
            ErrorCode = errorCode.ToString();
        }

        protected KappException(SerializationInfo info, StreamingContext context, string errorCode) : base(info, context)
        {
        }
    }
}
