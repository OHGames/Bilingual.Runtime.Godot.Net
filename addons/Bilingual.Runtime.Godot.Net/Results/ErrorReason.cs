namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// Reasons for errors.
    /// </summary>
    public enum ErrorReason
    {
        /// <summary>
        /// There is a choose statement that is being run and it needs an option to be selected.
        /// This error is returned when you try to get the next line of dialogue while the choose
        /// statement is waiting.
        /// </summary>
        MustSelectChooseOption,

        /// <summary>
        /// The script has been paused and is awaiting a timer to complete.
        /// </summary>
        ScriptPaused
    }
}
