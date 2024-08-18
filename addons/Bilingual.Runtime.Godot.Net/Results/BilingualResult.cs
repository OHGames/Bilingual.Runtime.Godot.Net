using Godot;

namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// A result from getting the next line of dialogue.
    /// </summary>
    public abstract partial class BilingualResult : RefCounted
    {
        /// <summary>The result type. A quick way to check the type of result.</summary>
        public abstract ResultType Type { get; }
    }
}
