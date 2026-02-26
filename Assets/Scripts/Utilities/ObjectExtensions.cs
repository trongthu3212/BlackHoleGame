using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Serialization.Json;

namespace BlackHole.Utilities
{
    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    ?.SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static Dictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static JsonObject AsJsonObject(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var dict = source.AsDictionary(bindingAttr);
            var jsonObject = new JsonObject();
            foreach (var kvp in dict)
            {
                jsonObject[kvp.Key] = kvp.Value;
            }

            return jsonObject;
        }
    }
}