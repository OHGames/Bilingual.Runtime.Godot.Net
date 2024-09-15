using Godot;

namespace Bilingual.Runtime.Godot.Net.Localization
{
    /// <summary>
    /// Points to a translation file.
    /// </summary>
    [GlobalClass]
    public partial class TranslationFileResource : Resource
    {
        /// <summary>The file path.</summary>
        [Export(PropertyHint.File, "*.zip")]
        public string FilePath { get; set; } = "";
    }
}
