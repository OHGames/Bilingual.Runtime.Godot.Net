using Godot;
using Godot.Collections;

namespace Bilingual.Runtime.Godot.Net.Nodes
{
    /// <summary>
    /// Holds settings for translations.
    /// Share this resource between dialogue runners in different scenes.
    /// </summary>
    [GlobalClass]
    public partial class BilingualTranslationSettingsResource : Resource
    {
        /// <summary>The original language the dialogue was written in.
        /// This should be a language code.</summary>
        [Export]
        public string OriginalLanguage { get; set; } = "en";

        /// <summary>The language code to translate dialogue into.
        /// Set to empty string or <see cref="OriginalLanguage"/>
        /// to not perform translation.</summary>
        [Export]
        public string TranslateInto { get; set; } = "";

        /// <summary>A list of all the translations.</summary>
        [Export]
        public Array<TranslationFileResource> TranslationFiles { get; set; } = [];

        /// <summary>If we should translate.</summary>
        public bool ShouldTranslate => (TranslateInto != "") && (OriginalLanguage != TranslateInto);
    }
}
