using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.Deserialization;
using CsvHelper;
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Expression = Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions.Expression;

namespace Bilingual.Runtime.Godot.Net.Nodes
{
    /// <summary>
    /// Holds translation data.
    /// The translation must be set via <see cref="SetTranslation(BilingualTranslationSettingsResource)"/> before
    /// scripts can be added using <see cref="AddScript(string)"/>.
    /// </summary>
    public static partial class BilingualTranslationService
    {
        /// <summary>The translation settings. Should be set by the <see cref="DialogueRunner"/>.</summary>
        public static BilingualTranslationSettingsResource TranslationSettings { get; set; } = null!;

        /// <summary>Reads and holds the zip file in memory.</summary>
        private static ZipReader reader = new ZipReader();

        /// <summary>The list of open scripts and their dialogue.</summary>
        private static readonly Dictionary<string, List<TextAndId>> openScripts = [];

        /// <summary>Set the translation.
        /// Must be called before files are added to the <see cref="DialogueRunner"/>.</summary>
        /// <param name="settings">The new translation settings.</param>
        public static void SetTranslation(BilingualTranslationSettingsResource settings)
        {
            openScripts.Clear();
            TranslationSettings = settings;

            if (!settings.ShouldTranslate) return;

            var zipReader = new ZipReader();
            var result = zipReader
                .Open(settings.TranslationFiles
                .Where(f => 
                    f.FilePath.EndsWith($"{TranslationSettings.TranslateInto}.zip"))
                .First().FilePath);

            if (result != Error.Ok) 
                throw new InvalidOperationException($"Cannot find translation " +
                    $"file for {TranslationSettings.TranslateInto}");

            reader = zipReader;
        }

        /// <summary>Add a script to the translation service.</summary>
        /// <param name="fullName"></param>
        public static void AddScript(string fullName)
        {
            if (!TranslationSettings.ShouldTranslate) return;

            // Path is the full name with each period replaced with slashes.
            var path = fullName.Replace('.', '/');
            var fileBytes = reader.ReadFile(path + ".csv");
            string fileText = fileBytes.GetStringFromUtf8();

            // load the lines
            using var stream = new StringReader(fileText);
            using var csv = new CsvReader(stream, System.Globalization.CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<CsvMap>();
            var records = csv.GetRecords<TextAndId>();
            openScripts.Add(fullName, records.ToList());
        }

        /// <summary>Remove the scripts from translation.</summary>
        public static void CloseScripts()
        {
            openScripts.Clear();
        }

        /// <summary>Get the translated content of a dialogue line.
        /// If there are inlines, the expressions come from <paramref name="originalLine"/>.</summary>
        /// <param name="lineId">The id.</param>
        /// <param name="fullNameScript">The full script name.</param>
        /// <param name="originalLine">The original line to get expressions from.</param>
        /// <returns>A <see cref="Literal"/> or <see cref="InterpolatedString"/>.</returns>
        public static Expression Translate(uint lineId, string fullNameScript, DialogueStatement originalLine)
        {
            var lines = openScripts[fullNameScript];
            var line = lines.Where(l => l.LineId == lineId).First();
            var dialogueText = line.Dialogue.ToString();
            var regex = MatchInlineReplacement();
            var split = regex.Split(dialogueText);

            if (split.Length == 1)
                return new Literal(dialogueText);
            else
            {
                List<Expression> expressions = [];
                var inlineExpressions = ((InterpolatedString)originalLine.Dialogue).Expressions;
                var i = 0;
                foreach (var match in split)
                {
                    // there should be an alternating pattern of text and regex match
                    // If match is at beginning of string, regex.Split() inserts an empty string
                    // before the match.
                    var isText = i % 2 == 0;

                    if (isText) expressions.Add(new Literal(match));
                    else
                    {
                        // get rid of the =[]=
                        var expressionIndex = int.Parse(match[2..^2]);
                        expressions.Add(inlineExpressions[expressionIndex]);
                    }

                    i++;
                }
                return new InterpolatedString(expressions);
            }
        }

        [GeneratedRegex(@"(?<!\\)(=\[\d+?\]=)")]
        public static partial Regex MatchInlineReplacement();
    }
}
