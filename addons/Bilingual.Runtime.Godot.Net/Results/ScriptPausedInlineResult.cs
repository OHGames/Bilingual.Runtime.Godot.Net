using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;

namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// When a script is paused using a Wait command inline.
    /// This result inherits <see cref="DialogueResult"/>, so if using a type-based switch statement,
    /// this case must be above the <see cref="DialogueResult"/> case. Or, the case that handles
    /// <see cref="DialogueResult"/> uses a case gaurd to check when the type <c>is not ScriptPausedInlineResult</c>.
    /// </summary>
    public partial class ScriptPausedInlineResult(string dialogue, DialogueStatement dialogueStatement, double seconds,
        string fullDialogue, bool wasPaused) : DialogueResult(dialogue, dialogueStatement, wasPaused)
    {
        public override ResultType Type => ResultType.ScriptPausedInline;

        /// <summary>
        /// The amount of seconds paused.
        /// </summary>
        public double Seconds { get; set; } = seconds;

        /// <summary>
        /// The full line of dialogue. This can be used to determine the size
        /// of the remaining text in the paused inline text. 
        /// Use <see cref="DialogueResult.Dialogue"/> instead of this when showing
        /// dialogue, showing FullDialogue instead will defeat the purpose of inline pauses.
        /// This is only populated on the first chunk of text returned. All other chucks
        /// will have this value be <see cref="string.Empty"/>.
        /// </summary>
        public string FullDialogue { get; set; } = fullDialogue;
    }
}
