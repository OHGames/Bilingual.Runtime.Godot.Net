using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// An inject statement will add the script into the current scope.
    /// All variables in the calling script will be accessable from the injected script.
    /// The calling script will continue to run after the injected script is finished.
    /// </summary>
    public class InjectStatement : Statement
    {
        /// <summary>The name of the script to inject.</summary>
        public string Script { get; set; }

        [Obsolete("Used by JSON only.")]
        private InjectStatement()
        {
            // used by JSON.
        }

        public InjectStatement(string script)
        {
            Script = script;
        }
    }
}
