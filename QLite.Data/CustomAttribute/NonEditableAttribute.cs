using System;

namespace QLite.Data.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEditableAttribute : Attribute
    {
    }

}
