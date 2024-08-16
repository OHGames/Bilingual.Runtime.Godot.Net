#if TOOLS
using Bilingual.Runtime.Godot.Net.Nodes;
using Godot;

[Tool]
public partial class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        var runnerScript = GD.Load<Script>("res://addons/Bilingual.Runtime.Godot.Net/Nodes/DialogueRunner.cs");
        var runnerIcon = GD.Load<Texture2D>("res://addons/Bilingual.Runtime.Godot.Net/Assets/bubble.svg");
        AddCustomType(nameof(DialogueRunner), nameof(Node), runnerScript, runnerIcon);
    }

    public override void _ExitTree()
    {
        RemoveCustomType(nameof(DialogueRunner));
    }
}
#endif
