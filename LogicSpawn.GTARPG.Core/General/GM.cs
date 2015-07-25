using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LogicSpawn.GTARPG.Core.General
{
    static class GM
    {
        public static int GetHashKey(string hashName)
        {
            return Function.Call<int>(Hash.GET_HASH_KEY, hashName);
        }

        public static T Copy<T>(T other)
        {
            var objectStr = JsonConvert.SerializeObject(other, GetSerialisationSettings());
            var item = JsonConvert.DeserializeObject<T>(objectStr, GetSerialisationSettings());
            return item;
        }

        public static JsonSerializerSettings GetSerialisationSettings()
        {
            return new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.Objects,
                //TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ContractResolver = new DefaultContractResolver
                {
#pragma warning disable 612,618
                    DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
#pragma warning restore 612,618
                },
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };
        }

    }
}
