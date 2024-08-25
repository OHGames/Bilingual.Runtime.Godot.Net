namespace Bilingual.Runtime.Godot.Net.Deserialization
{
    /// <summary>
    /// A line and its id for translation.
    /// </summary>
    internal struct TextAndId
    {
        /// <summary>
        /// The translated dialogue. 
        /// </summary>
        public string Dialogue { get; set; }

        /// <summary>
        /// The line id.
        /// </summary>
        public uint LineId { get; set; }
    }
}
