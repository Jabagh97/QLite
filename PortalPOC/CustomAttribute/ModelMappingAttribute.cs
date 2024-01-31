namespace  PortalPOC.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelMappingAttribute : Attribute
    {
        public Type ModelType { get; }
        public Type ViewModelType { get; }

        public ModelMappingAttribute(Type modelType, Type viewModelType)
        {
            ModelType = modelType;
            ViewModelType = viewModelType;
        }
    }
}
