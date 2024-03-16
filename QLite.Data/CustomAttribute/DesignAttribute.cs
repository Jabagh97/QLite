using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property)]

    public class DesignAttribute : Attribute
    {
    }
}
