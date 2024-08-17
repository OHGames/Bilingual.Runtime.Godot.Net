namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// The script has been paused by a Wait() function.
    /// </summary>
    public class ScriptPausedResult(double seconds) : BilingualResult
    {
        public override ResultType Type => ResultType.ScriptPaused;

        /// <summary>The amount of seconds this script will be paused for.</summary>
        public double Seconds { get; set; } = seconds;
    }
}
