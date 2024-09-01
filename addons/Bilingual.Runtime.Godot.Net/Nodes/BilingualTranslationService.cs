using Bilingual.Runtime.Godot.Net.BilingualTypes;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements.ControlFlow;
using Bilingual.Runtime.Godot.Net.Deserialization;
using CsvHelper;
using Godot;
using ReswPlusLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Expression = Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions.Expression;
using Script = Bilingual.Runtime.Godot.Net.BilingualTypes.Containers.Script;

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

        /// <summary>The original language.</summary>
        public static string OriginalLanguage => TranslationSettings.OriginalLanguage;

        /// <summary>Language to translate to.</summary>
        public static string TranslateInto => TranslationSettings.TranslateInto;

        /// <summary>Get the language code to translate to without the dialect.</summary>
        public static string TranslateIntoBasic
        {
            get
            {
                var to = TranslationSettings.TranslateInto;
                if (to.Contains('-'))
                {
                    // get rid of dialect code.
                    return to[0..to.IndexOf('-')];
                }
                else
                {
                    return to;
                }
            }
        }

        /// <summary>Reads and holds the zip file in memory.</summary>
        private static ZipReader reader = new ZipReader();

        /// <summary>The list of open scripts and their translated dialogues.</summary>
        private static readonly Dictionary<string, List<TextAndId>> openScripts = [];

        /// <summary>The translated lines.</summary>
        private static readonly Dictionary<uint, Expression> translatedLines = [];

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
        public static void AddScript(string fullName, Script script)
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

            // Pre-translate to save time when fetching dialogue.
            var dialogueLines = SearchForStatements(script.Block);
            foreach (var dialogue in dialogueLines)
            {
                var id = dialogue.LineId ?? throw new Exception("No line id. This should not happen.");
                translatedLines.Add(id, PreTranslate(id, fullName, dialogue));
            }
        }

        /// <summary>Look for dialogue lines.</summary>
        /// <param name="block">The block to search.</param>
        /// <returns>The list of dialogue lines.</returns>
        private static List<DialogueStatement> SearchForStatements(Block block)
        {
            List<DialogueStatement> statements = [];
            foreach (var line in block.Statements)
            {
                if (line is DialogueStatement dialogue)
                {
                    statements.Add(dialogue);
                }
                else if (line is ChooseStatement choose)
                {
                    foreach (var chooseBlock in choose.Blocks)
                    {
                        statements.AddRange(SearchForStatements(chooseBlock.Block));
                    }
                }
                else if (line is IfStatement ifStatement)
                {
                    statements.AddRange(SearchForStatements(ifStatement.Block));
                    foreach (var ifElse in ifStatement.ElseIfStatements)
                    {
                        statements.AddRange(SearchForStatements(ifElse.Block));
                    }
                    if (ifStatement.ElseStatement is not null)
                    {
                        statements.AddRange(SearchForStatements(ifStatement.ElseStatement.Block));
                    }
                }
                else if (line is BlockedStatement blocked)
                {
                    statements.AddRange(SearchForStatements(blocked.Block));
                }
            }
            return statements;
        }

        /// <summary>Remove the scripts from translation.</summary>
        public static void CloseScripts()
        {
            openScripts.Clear();
            translatedLines.Clear();
        }

        /// <summary>Get the translated content of a dialogue line.
        /// If there are inlines, the expressions come from <paramref name="originalLine"/>.</summary>
        /// <param name="lineId">The id.</param>
        /// <param name="fullNameScript">The full script name.</param>
        /// <param name="originalLine">The original line to get expressions from.</param>
        /// <returns>A <see cref="Literal"/> or <see cref="InterpolatedString"/>.</returns>
        public static Expression Translate(uint lineId)
        {
            return translatedLines[lineId];
        }

        /// <summary>Get the translated content of a dialogue line.
        /// If there are inlines, the expressions come from <paramref name="originalLine"/>.</summary>
        /// <param name="lineId">The id.</param>
        /// <param name="fullNameScript">The full script name.</param>
        /// <param name="originalLine">The original line to get expressions from.</param>
        /// <returns>A <see cref="Literal"/> or <see cref="InterpolatedString"/>.</returns>
        private static Expression PreTranslate(uint lineId, string fullNameScript, DialogueStatement originalLine)
        {
            // Get the localized text from the csv.
            var lines = openScripts[fullNameScript];
            var line = lines.Where(l => l.LineId == lineId).First();
            // This is the text from the localized csv.
            var dialogueText = line.Dialogue.ToString();
            var regex = MatchInlines();
            // Split the regex. See below foreach for more info.
            var split = regex.Split(dialogueText);

            if (split.Length == 1)
                // Nothing special, just get the text.
                return new Literal(dialogueText);
            else
            {
                // Interpolated dialogue, so keep the expressions from the original
                // but replace all strings with the localized versions.
                List<Expression> expressions = [];
                var originalInlines = ((InterpolatedString)originalLine.Dialogue).Expressions;
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
                        // Inline variable
                        if (match[1] == '[')
                        {
                            // get rid of the =[]=
                            var expressionIndex = int.Parse(match[2..^2]);
                            expressions.Add(originalInlines[expressionIndex]);
                        }
                        // Localized quanity
                        else
                        {
                            // Get rid of ={ }=
                            var newInline = match[2..^2];

                            // Get the index.
                            var indexString = newInline.Split(' ').First();
                            var index = int.Parse(indexString);
                            newInline = newInline[indexString.Length..];

                            Dictionary<PluralTypeEnum, string> plurals = [];
                            var splitPlurals = newInline.Split(',');
                            foreach (var plural in splitPlurals)
                            {
                                var trimmed = plural.Trim();
                                var parts = trimmed.Split('=');
                                var pluralType = parts[0];
                                var pluralStr = parts[1][1..^1]; // remove the single qoutes

                                var pluralTypeEnum = Enum.Parse<PluralTypeEnum>(pluralType, true);
                                plurals.Add(pluralTypeEnum, pluralStr);
                            }
                            // Get the expression that will determine the plurals from the original expression.
                            var valueExpression = ((LocalizedQuanity)originalInlines[index]).Value;
                            expressions.Add(new LocalizedQuanity(valueExpression, plurals));
                        }
                    }

                    i++;
                }
                return new InterpolatedString(expressions);
            }
        }

        // This Regex string matches either an inline variable or localized quanity.
        // Noncapturing groups '(?: )' are intentionally noncapturing due to Regex.Split() functionality.
        // For more a in depth explination, use regex101.com or the generated doc comment.
        // The left part of the giant boolean OR is for localized quanities.
        // The right part of the giant boolean OR is the inline variables/expressions.
        // backslash regex matching credit:
        // https://stackoverflow.com/a/11819111
        [GeneratedRegex(@"((?<!\\)(?:\\\\)*={\d+?\s+?(?:\w*?='(?:[^'])+?',?\s?)+}=|(?<!\\)(?:\\\\)*=\[\d+?\]=)")]
        public static partial Regex MatchInlines();

        // Match hash tag that is not escaped.
        [GeneratedRegex(@"(?<!\\)(?:\\\\)*#(?:#[mfnMFN]?)?")]
        public static partial Regex MatchHashTag();
    }
}
