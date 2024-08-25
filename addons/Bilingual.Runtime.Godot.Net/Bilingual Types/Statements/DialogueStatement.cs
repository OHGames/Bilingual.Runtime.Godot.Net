using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A line of dialogue.
    /// </summary>
    public class DialogueStatement(string name, string? emotion, Expression dialogue, 
        uint? lineId, string? translationComment) : Statement
    {
        /// <summary>The name of the character.</summary>
        public string Name { get; set; } = name;

        /// <summary>The emotion of the character.</summary>
        public string? Emotion { get; set; } = emotion;

        /// <summary>The dialogue. Either a string <see cref="Literal"/> or an <see cref="InterpolatedString"/>.</summary>
        public Expression Dialogue { get; set; } = dialogue;

        /// <summary>The line id.</summary>
        public uint? LineId { get; set; } = lineId;

        /// <summary>The translation comment.</summary>
        public string? TranslationComment { get; set; } = translationComment;

        /// <summary>Copy the statement with a different string.</summary>
        /// <param name="str">The new dialogue.</param>
        /// <returns>A new statement.</returns>
        internal DialogueStatement CopyWithNewDialogue(InterpolatedString str)
        {
            return new DialogueStatement(Name, Emotion, str, LineId, TranslationComment);
        }

        /// <summary>Copy the statement.</summary>
        internal DialogueStatement Copy()
        {
            return new DialogueStatement(Name, Emotion, Dialogue, LineId, TranslationComment);
        }
    }
}
