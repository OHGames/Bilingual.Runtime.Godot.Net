namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// The type of result a <see cref="DialogueResult"/> is. A quick way to check the type of the result.
    /// </summary>
    public enum ResultType
    {
        Error,
        DialogueResult,
        ScriptOver,
        ChooseOptions,
        ScriptPaused,
        ScriptPausedInline
    }
}
