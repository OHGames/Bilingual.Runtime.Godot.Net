# Bilingual.Runtime.Godot.Net
The Bilingual Runtime for the Godot game engine. Bilingual is a dialogue-oriented scripting language.

This project will run Bilingual scripts compiled using the [Bilingual Compiler](https://github.com/OHGames/Bilingual.Compiler).
To read how to script in Bilingual, visit the Compiler's GitHub repo.

## Features

- Character names
- For .NET Godot Projects
- Interpolated dialogue (inline content)
- Custom commands
- Async or sync support for commands
- Basic API allows for flexibility
- Control over wait command
- C# inspired syntax
- Compiled scripts can be run on engines with a supported runtime
- Localization support
    - Plural/Ordinal support across languages
    - Accounts for possibility of grammatical gender and abbreviation in pluralized/ordinalized words

## Basic Setup
Make sure to include the `Bilingual.Runtime.Godot.Net` folder in your addons and enable it.

To add dialogue, create a `DialogueRunner` node. This node will load the dialogue and run it.
In the editor, include the compiled Bilingual scripts in the Runner's `FilePaths` variable. These will be parsed and loaded on load.

Running dialogue is easy! Just find the DialogueRunner node and load the script. Load and run the script by calling `RunScript(script)` on the Runner.
After the script is loaded, call `GetNextLine` to get the next line of dialogue. And thats it!

The `GetNextLine` function returns a `BilingualResult`. Each Result has a special meaning and carries special data.
You can also choose dialogue options from the Runner and set its current language.

Refer to the `Test.tcs` and `Source/Test.cs` files for a quick example.