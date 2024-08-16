using Godot;

namespace Bilingual.Runtime.Godot.Net.Nodes
{
    /// <summary>
    /// Points to a .bic file.
    /// </summary>
    [Icon("res://addons/Bilingual.Runtime.Godot.Net/Assets/bilingual_file_tiny_optimized.svg")]
    [GlobalClass]
    public partial class BilingualFileResource : Resource
    {
        /// <summary>
        /// The path to the .bic file this resource points to.
        /// </summary>
        [Export(PropertyHint.File, "*.bic,*.json")]
        public string FilePath { get; set; } = "";
    }
}
