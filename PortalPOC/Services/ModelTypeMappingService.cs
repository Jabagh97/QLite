using System.Reflection;

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


    #region Reflection
    //Get Models and ViewModels using Reflection 


    //public class ModelTypeMappingService : IModelTypeMappingService
    //{
    //    private readonly Dictionary<string, (Type, Type)> modelTypeMapping;

    //    public ModelTypeMappingService()
    //    {
    //        modelTypeMapping = GetModelTypeMappingInternal();
    //    }

    //    private Dictionary<string, (Type, Type)> GetModelTypeMappingInternal()
    //    {
    //        Assembly assembly = Assembly.GetExecutingAssembly(); // You might need to adjust this depending on where your models and view models are located

    //        var modelTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "PortalPOC.Models");
    //        var viewModelTypes = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "PortalPOC.ViewModels");

    //        var mapping = new Dictionary<string, (Type, Type)>();

    //        foreach (var modelType in modelTypes)
    //        {
    //            var viewModelType = viewModelTypes.FirstOrDefault(vm => vm.Name == $"{modelType.Name}ViewModel");

    //            if (viewModelType != null)
    //            {
    //                mapping[modelType.Name] = (modelType, viewModelType);
    //            }
    //        }

    //        return mapping;
    //    }

    //    // Explicitly implement the interface method
    //    Dictionary<string, (Type, Type)> IModelTypeMappingService.GetModelTypeMapping()
    //    {
    //        return GetModelTypeMappingInternal();
    //    }
    //}
    #endregion



}
