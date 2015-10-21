﻿//
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaliber3D.Render;

namespace Kaliber3D.Render
{
    /// <summary>
    /// 
    /// </summary>
    internal class ProjectContractResolver : DefaultContractResolver
    {
        // NOTE: The UWP is missing Type.IsGenericType property.
        /*
        /// <summary>
        /// Use ImmutableArray for IList contract.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override JsonContract ResolveContract(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return base
                    .ResolveContract(typeof(ImmutableArray<>)
                    .MakeGenericType(type.GenericTypeArguments[0]));
            }
            return base.ResolveContract(type);
        }
        */

        /// <summary>
        /// Serialize only writable properties. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewtonsoftSerializer : ISerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Serialize<T>(T value)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver()
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var json = JsonConvert.SerializeObject(value, settings);
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver()
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var value = JsonConvert.DeserializeObject<T>(json, settings);
            return value;
        }
    }
}
