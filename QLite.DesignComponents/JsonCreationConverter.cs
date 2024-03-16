using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.DesignComponents
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;

        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jObject = JObject.Load(reader);
            T val = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), val);
            return val;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
