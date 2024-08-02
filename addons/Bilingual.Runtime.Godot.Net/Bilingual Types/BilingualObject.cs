using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes
{
    /// <summary>
    /// The base class for all Bilingual types.
    /// </summary>
    public class BilingualObject
    {
        // Will be set by JSON.
        public string ObjectType { get; set; } = "";
    }
}
