namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A run statement will run the dialogue listed and not return to the caller.
    /// </summary>
    public class RunStatement(string script) : Statement
    {
        /// <summary>Name of the script to run</summary>
        public string Script { get; set; } = script;
    }
}
