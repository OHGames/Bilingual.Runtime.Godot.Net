using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Nodes;
using Bilingual.Runtime.Godot.Net.Results;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net
{
    public partial class Test : Node2D
    {
        private bool dialoguePaused;
        private DialogueRunner? runner = null;
        private Label label;

        public override void _Ready()
        {
            CommandStore.AddCommand($"Test.{nameof(TestCommand)}", TestCommand);
            CommandStore.AddCommand($"Test.{nameof(AsyncTestCommand)}", AsyncTestCommand);

            runner = GetNode<DialogueRunner>("DialogueRunner");
            label = GetNode<Label>("%Label");
            if (runner is null) throw new InvalidOperationException("Node cannot be found.");

            runner.DialoguePaused += (result) => dialoguePaused = true;
            runner.DialogueResumed += () => dialoguePaused = false;

            runner.RunScript("Test.Wow.Wow");
        }

        public override void _PhysicsProcess(double delta)
        {
            GetAndDisplay(runner!);
        }

        public void GetAndDisplay(DialogueRunner runner)
        {
            if (dialoguePaused) return;

            var line = runner.GetNextLine();
            switch (line)
            {
                case DialogueResult dialogue:
                    if (dialogue.WasPaused)
                        label.Text += dialogue.Dialogue;
                    else
                        label.Text = $"{dialogue.Character}: {dialogue.Dialogue}";
                    break;

                case ErrorResult error:
                    GD.PushWarning(error.ErrorReason);
                    break;

                default:
                    break;
            }
        }

        //private void Wait(double seconds)
        //{
        //    var timer = GetTree().CreateTimer(seconds);
        //    timer.Timeout += () =>
        //    {
        //        dialoguePaused = false;
        //    };
        //}

        public void TestCommand(List<object> parameters)
        {
            var i = parameters[0];
        }

        public async Task AsyncTestCommand(List<object> parameters)
        {
            await Task.Delay(1000);
            var i = 0;
        }
    }
}
