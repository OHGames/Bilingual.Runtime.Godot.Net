using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Nodes;
using Bilingual.Runtime.Godot.Net.Results;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bilingual.Runtime.Godot.Net
{
    public partial class Test : Node2D
    {
        private bool dialoguePaused;
        private DialogueRunner runner = null!;
        private Label label = null!;
        private Label pausedLabel = null!;
        private bool getNextLine = true;

        public override async void _Ready()
        {
            CommandStore.AddCommand($"Test.{nameof(TestCommand)}", TestCommand);
            CommandStore.AddCommand($"Test.{nameof(AsyncTestCommand)}", AsyncTestCommand);

            runner = GetNode<DialogueRunner>("DialogueRunner");
            label = GetNode<Label>("%Label");
            pausedLabel = GetNode<Label>("%IsPaused");
            if (runner is null) throw new InvalidOperationException("Node cannot be found.");

            runner.DialoguePaused += DialoguePaused;
            runner.DialogueResumed += DialogueResumed;
            runner.ScriptStartedRunning += (dict) =>
            {
                if (dict.TryGetValue("Characters", out object? chars))
                {
                    var characters = ((List<object>)chars).Cast<string>();
                    GD.PushWarning(string.Join(", ", characters));
                }
            };

            runner.RunScript("Test.Wow.Wow");
            pausedLabel.Text = $"paused: {dialoguePaused}";
            await GetAndDisplay(runner);
        }

        private void DialoguePaused(BilingualResult result)
        {
            dialoguePaused = true; 
            pausedLabel.Text = $"paused: {dialoguePaused}";
        }

        private async void DialogueResumed()
        {
            dialoguePaused = false; 
            pausedLabel.Text = $"paused: {dialoguePaused}";
            getNextLine = true; 
            await GetAndDisplay(runner);
        }

        public override async void _PhysicsProcess(double delta)
        {
            if (dialoguePaused) return;
            if (Input.IsActionJustPressed("continue_dialogue") && !getNextLine)
            {
                getNextLine = true;
                await GetAndDisplay(runner);
            }
        }

        public async Task GetAndDisplay(DialogueRunner runner)
        {
            if (dialoguePaused) return;

            var line = await runner.GetNextLine();
            switch (line)
            {
                case DialogueResult dialogue:
                    if (dialogue.WasPaused)
                        label.Text += dialogue.Dialogue;
                    else
                        label.Text = $"{dialogue.Character}: {dialogue.Dialogue}";
                    break;

                case WarningResult error:
                    GD.PushWarning(error.WarningReason);
                    break;

                default:
                    break;
            }

            getNextLine = false; 
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
            await Task.Delay(2000);
            var i = 0;
            GD.PushWarning("AsyncTestCommand finished.");
        }
    }
}
