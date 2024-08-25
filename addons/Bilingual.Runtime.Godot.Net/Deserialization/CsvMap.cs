using Bilingual.Runtime.Godot.Net.BilingualTypes.Expressions;
using Bilingual.Runtime.Godot.Net.BilingualTypes.Statements;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace Bilingual.Runtime.Godot.Net.Deserialization
{
    internal class CsvMap : ClassMap<TextAndId>
    {
        public CsvMap()
        {
            Map(m => m.LineId).Index(0).Name("LineId").TypeConverter(new UintToStringConverter());
            //Map(m => m.Name).Index(1).Name("Name");
            Map(m => m.Dialogue).Index(2).Name("Dialogue");
            //Map(m => m.Emotion).Index(3).Name("Emotion");
           //Map(m => m.TranslationComment).Index(4).Name("TranslationComment");
        }
    }

    public class UintToStringConverter : ITypeConverter
    {
        public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text is null) throw new ArgumentException("Text should not be null", nameof(text));
            return uint.Parse(text);
        }

        public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
