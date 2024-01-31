namespace PortalPOC.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEditableAttribute : Attribute
    {
    }

}
