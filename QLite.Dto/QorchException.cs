using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class QorchException: Exception
    {
        public QorchException(string message, QorchErrorCodes code): base(message)
        {
            Code = code;
        }

        public QorchException(string message, Exception innerException, QorchErrorCodes code) : base(message, innerException)
        {
            Code = code;
        }

        public QorchErrorCodes Code { get; set; }
    }
}
