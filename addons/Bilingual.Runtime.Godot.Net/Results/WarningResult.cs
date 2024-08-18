namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// When a simple warning occurs that can be easily fixed.
    /// </summary>
    public partial class WarningResult(WarningReason reason) : BilingualResult
    {
        /// <summary>
        /// The reason.
        /// </summary>
        public WarningReason WarningReason { get; } = reason;
    }
}
