using System;

namespace QLite.Data.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property)]

    public class CustomPropertyAttribute : Attribute
    {
        public string CustomStyle { get; }

        public CustomPropertyAttribute(string customStyle)
        {
            CustomStyle = customStyle;
        }
    }
}
