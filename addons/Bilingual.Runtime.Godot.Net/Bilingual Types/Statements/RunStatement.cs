using System;
using System.Diagnostics.CodeAnalysis;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A run statement will run the dialogue listed and not return to the caller.
    /// </summary>
    public class RunStatement : Statement
    {
        /// <summary>Name of the script to run</summary>
        public string Script { get; set; }

        [Obsolete("Used by JSON only.")]
        private RunStatement()
        {
            // used by JSON.
        }

        public RunStatement(string script)
        {
            Script = script;
        }
    }
}
