namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// This is a statement used by the VM only and should not appear in the JSON.
    /// It tells the VM that the injected script has ended so the current script name can
    /// reset to the previous one.
    /// </summary>
    public class EndInjectStatement(string name) : Statement
    {
        public string PreviousScriptName { get; set; } = name;
    }
}
