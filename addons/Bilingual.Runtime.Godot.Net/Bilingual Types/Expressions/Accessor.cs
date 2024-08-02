using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions
{
    /// <summary>
    /// Access the value of a function call or an array.
    /// </summary>
    public class Accessor : BilingualObject
    {
        public string MemberName { get; set; }
        public Expression? Indexer { get; set; }
        public Params? Params { get; set; }

        [Obsolete("Used by JSON only.")]
        private Accessor()
        {
            // used by JSON.
        }

        /// <param name="memberName">The name of the property/array/function.</param>
        /// <param name="indexer">The index of the array. (not indexing if null)</param>
        /// <param name="params">The parameters of the function call. (not calling function if null).</param>
        public Accessor(string memberName, Expression? indexer, Params? @params)
        {
            MemberName = memberName;
            Indexer = indexer;
            Params = @params;
        }
    }
}
