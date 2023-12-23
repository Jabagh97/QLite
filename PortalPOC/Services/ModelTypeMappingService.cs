namespace PortalPOC.Services
{
    public interface IModelTypeMappingService
    {
        Dictionary<string, (Type, Type)> GetModelTypeMapping();
    }

    public class ModelTypeMappingService : IModelTypeMappingService
    {
        private readonly Dictionary<string, (Type, Type)> modelTypeMapping;

        public ModelTypeMappingService(IConfiguration configuration)
        {
            modelTypeMapping = configuration.GetSection("ModelTypeMapping")
                .Get<Dictionary<string, string[]>>()
                .ToDictionary(kv => kv.Key, kv => (GetTypeFromConfig(kv.Value[0]), GetTypeFromConfig(kv.Value[1])));
        }

        private Type GetTypeFromConfig(string typeConfig)
        {
            string[] typeInfo = typeConfig.Split(',');

            if (typeInfo.Length >= 2)
            {
                string typeName = typeInfo[0].Trim();
                string assemblyName = typeInfo[1].Trim();

                return Type.GetType($"{typeName}, {assemblyName}", throwOnError: true);
            }

            throw new ArgumentException($"Invalid type configuration: {typeConfig}");
        }

        public Dictionary<string, (Type, Type)> GetModelTypeMapping()
        {
            return modelTypeMapping;
        }
    }

}
