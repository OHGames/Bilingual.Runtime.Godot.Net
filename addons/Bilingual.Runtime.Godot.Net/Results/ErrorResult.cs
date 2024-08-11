namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// When a simple error occurs that can be easily fixed.
    /// </summary>
    public class ErrorResult(ErrorReason reason) : BilingualResult
    {
        public override ResultType Type => ResultType.Error;

        /// <summary>
        /// The reason.
        /// </summary>
        public ErrorReason ErrorReason { get; } = reason;
    }
}
