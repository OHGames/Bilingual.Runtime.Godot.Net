using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using System;

namespace Bilingual.Runtime.Godot.Net.BilingualTypes.Statements
{
    /// <summary>
    /// A line of dialogue.
    /// </summary>
    public class DialogueStatement : Statement
    {
        /// <summary>The name of the character.</summary>
        public string Name { get; set; }

        /// <summary>The emotion of the character.</summary>
        public string? Emotion { get; set; }

        /// <summary>The dialogue. Either a string <see cref="Literal"/> or an <see cref="InterpolatedString"/>.</summary>
        public Expression Dialogue { get; set; }

        /// <summary>The line id.</summary>
        public uint? LineId { get; set; }

        /// <summary>The translation comment.</summary>
        public string? TranslationComment { get; set; }

        [Obsolete("Used by JSON only.")]
        private DialogueStatement()
        {
            // used by JSON.
        }

        public DialogueStatement(string name, string? emotion, Expression dialogue, uint? lineId, 
            string? translationComment)
        {
            Name = name;
            Emotion = emotion;
            Dialogue = dialogue;
            LineId = lineId;
            TranslationComment = translationComment;
        }

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
