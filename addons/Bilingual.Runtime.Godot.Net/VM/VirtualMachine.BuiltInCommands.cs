using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.Results;
using System;

namespace Bilingual.Runtime.Godot.Net.VM
{
    public partial class VirtualMachine
    {
        /// <summary>Check if a function name is a built-in command.</summary>
        /// <param name="name">Name of the function.</param>
        /// <returns>True if built in.</returns>
        private static bool CheckBuiltInCommands(string name)
        {
            return name == "Wait";
        }

        /// <summary>Run a built in command.</summary>
        /// <param name="name">The function name.</param>
        /// <param name="expression">The function call.</param>
        /// <returns>A bilingual result.</returns>
        private BilingualResult RunBuiltInCommand(string name, FunctionCallExpression expression)
        {
            return name switch
            {
                "Wait" => RunWaitCommand(expression),
                _ => throw new InvalidOperationException("Built-in command does not exist")
            };
        }

        /// <summary>Run a wait command.</summary>
        /// <param name="expression">The call.</param>
        /// <returns>A <see cref="ScriptPausedResult"/> result.</returns>
        private ScriptPausedResult RunWaitCommand(FunctionCallExpression expression)
        {
            var result = new ScriptPausedResult(GetWaitTime(expression));

            if (UseVmToWait)
            {
                paused = true;
                StartWait(result.Seconds);
                PausedCallback(result);
            }

            return result;
        }

        /// <summary>Get the wait time of the <c>Wait(double)</c> command.</summary>
        /// <param name="expression">The function.</param>
        /// <returns>How long.</returns>
        private double GetWaitTime(FunctionCallExpression expression)
        {
            var secondsExpr = expression.Params.Expressions[0];
            return EvaluateExpression<double>(secondsExpr);
        }

        /// <summary>Wait for dialogue.</summary>
        /// <param name="seconds"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void StartWait(double seconds)
        {
            if (tree is null) throw new InvalidOperationException("Scene tree is null");

            var timer = tree.CreateTimer(seconds);
            timer.Timeout += () =>
            {
                paused = false;
                ResumedCallback();
            };
        }
    }
}
