namespace Bilingual.Runtime.Godot.Net.Results
{
    /// <summary>
    /// The script has been paused by a Wait() function.
    /// </summary>
    public partial class ScriptPausedResult(double seconds) : BilingualResult
    {
        /// <summary>The amount of seconds this script will be paused for.</summary>
        public double Seconds { get; set; } = seconds;
    }
}
