using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;

namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// Dialogue from the <see cref="VirtualMachine"/>.
    /// </summary>
    public partial class DialogueResult(string dialogue, DialogueStatement dialogueStatement, 
        bool wasPaused) : BilingualResult
    {
        /// <summary>The dialogue.</summary>
        public string Dialogue { get; } = dialogue;

        /// <summary>The character speaki
        public string Character { get; } = dialogueStatement.Name;

        /// <summary>The emotion of the character.</summary>
        public string? Emotion { get; } = dialogueStatement.Emotion;

        /// <summary>The line id of this dialogue statement.</summary>
        public uint? LineId { get; } = dialogueStatement.LineId;

        /// <summary>A special comment for translators.</summary>
        public string? TranslationComment { get; } = dialogueStatement.TranslationComment;

        /// <summary>If the dialogue line was inline paused. If this is true,
        /// the <see cref="Dialogue"/> is the remaining text. Tack it onto the previous.</summary>
        public bool WasPaused { get; set; } = wasPaused;

        public override ResultType Type => ResultType.DialogueResult;
    }
}
