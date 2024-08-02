using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Godot;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bilingual.Runtime.Godot.Net.Deserialization
{
    /// <summary>
    /// Deserialize compiled <see cref="BilingualFile"/>s.
    /// </summary>
    public class Deserializer
    {
        /// <summary>The current assembly.</summary>
        private static readonly Assembly assembly = Assembly.GetAssembly(typeof(BilingualObject))
            ?? throw new Exception("Cant find assembly");

        /// <summary>All the types that are decendants of <see cref="BilingualObject"/>.</summary>
        private static IEnumerable<Type> bilingualTypes = [];

        /// <summary>The serializer settings.</summary>
        public static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings();

        public Deserializer()
        {
            // load the types in and cache them for later use.
            if (!bilingualTypes.Any())
            {
                bilingualTypes = assembly.GetTypes().ToList()
                    .Where(t => t.IsAssignableTo(typeof(BilingualObject)));

                // Let the ObjectType property determine the type for polymorphism.
                var builder = JsonSubtypesConverterBuilder
                   .Of(typeof(BilingualObject), nameof(BilingualObject.ObjectType));
                foreach (var type in bilingualTypes)
                {
                    // Add each type by their name.
                    builder.RegisterSubtype(type, type.Name);
                }
                serializerSettings.Converters.Add(builder.Build());
            }
        }

        /// <summary>Deserialize the json.</summary>
        /// <param name="path">The Godot path to the script.</param>
        /// <param name="bson">If the compiled script is in BSON.</param>
        /// <returns>A new <see cref="BilingualFile"/>.</returns>
        /// <exception cref="JsonException">If an error while deserializing occured.</exception>
        public BilingualFile DeserializeFile(string path, bool bson)
        {
            var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            if (!bson)
            {
                var json = file.GetAsText();
                var jsonObject = JsonConvert.DeserializeObject<BilingualFile>(json, serializerSettings);
                return jsonObject ?? throw new JsonException("The deseralized object is null");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
