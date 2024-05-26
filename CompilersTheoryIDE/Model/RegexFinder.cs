using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace CompilersTheoryIDE.Model;

public class RegexFinder
{
    /* Method to find all matches of specified regex patterns in a given text */
    public static string FindAllMatches(string text)
    {
        var matchesInfo = new List<string>();
        
        // Define regex patterns
        var regexPatterns = new Dictionary<string, string>
        {
            { "КПП организации", @"\d{3}\s?\d{3}\s?\d{3}" },
            { "Идентификатор", @"[a-zA-Z$_][a-zA-Z0-9]*" },
            { "VIN-номер", @"[A-HJ-NPR-Z0-9]{17}" }
        };

        foreach (var pattern in regexPatterns)
        {
            var regex = new Regex(pattern.Value);
            var matches = regex.Matches(text);
            
            foreach (Match match in matches) 
                matchesInfo.Add($"Вид РВ: {pattern.Key}, на позиции: {match.Index}, подошедший текст: {match.Value}");
        }

        return string.Join("\n", matchesInfo);
    }
}
