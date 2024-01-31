using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Common
{
    public static class Extensions
    {

        public static bool IsNullOrEmpty(this byte[] value)
        {
            return value == null || value.Length == 0;
        }

    }
}
