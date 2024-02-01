using QLite.Data.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QLite.Data.Services
{
    public interface IModelTypeMappingService
    {
        Dictionary<string, (Type, Type)> GetModelTypeMapping();
        bool TryGetModelTypes(string modelName, out Type? modelType, out Type? viewModelType);
    }

    public class ModelTypeMappingService : IModelTypeMappingService
    {
        private readonly Dictionary<string, (Type, Type)> modelTypeMapping;

        public ModelTypeMappingService()
        {
            var assemblyName = "QLite.Data";
            var targetAssembly = Assembly.Load(assemblyName);

            modelTypeMapping = targetAssembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(ModelMappingAttribute), true).Length > 0)
                .ToDictionary(type =>
                {
                    var attribute = (ModelMappingAttribute)type.GetCustomAttributes(typeof(ModelMappingAttribute), true)[0];
                    return type.Name;
                },
                type =>
                {
                    var attribute = (ModelMappingAttribute)type.GetCustomAttributes(typeof(ModelMappingAttribute), true)[0];
                    return (attribute.ModelType, attribute.ViewModelType);
                });

        }

        public Dictionary<string, (Type, Type)> GetModelTypeMapping()
        {
            return modelTypeMapping;
        }

        public bool TryGetModelTypes(string modelName, out Type? modelType, out Type? viewModelType)
        {
            modelType = null;
            viewModelType = null;

            var modelTypeMapping = GetModelTypeMapping();

            if (modelTypeMapping.TryGetValue(modelName, out var typeTuple))
            {
                modelType = typeTuple.Item1;
                viewModelType = typeTuple.Item2;
                return true;
            }

            return false;
        }

    }
}
