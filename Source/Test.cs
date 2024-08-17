using Bilingual.Runtime.Godot.Net.Commands;
using Bilingual.Runtime.Godot.Net.Deserialization;
using Bilingual.Runtime.Godot.Net.Nodes;
using Bilingual.Runtime.Godot.Net.Results;
using Bilingual.Runtime.Godot.Net.Scopes;
using Bilingual.Runtime.Godot.Net.VM;
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

            runner.RunScript("Test.Wow.Wow");
            if (runner is null) throw new InvalidOperationException("Node cannot be found.");
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
                case DialogueResult dialogue when line is not ScriptPausedInlineResult:
                    if (dialogue.WasPaused)
                        label.Text += dialogue.Dialogue;
                    else
                        label.Text = $"{dialogue.Character}: {dialogue.Dialogue}";
                    break;

                case ScriptPausedResult paused:
                    dialoguePaused = true;
                    Wait(paused.Seconds);
                    break;

                case ScriptPausedInlineResult inline:
                    if (inline.WasPaused)
                        label.Text += inline.Dialogue;
                    else
                        label.Text = $"{inline.Character}: {inline.Dialogue}";
                    dialoguePaused = true;
                    Wait(inline.Seconds);
                    break;

                default:
                    break;
            }
        }

        private void Wait(double seconds)
        {
            var timer = GetTree().CreateTimer(seconds);
            timer.Timeout += () =>
            {
                dialoguePaused = false;
            };
        }

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
