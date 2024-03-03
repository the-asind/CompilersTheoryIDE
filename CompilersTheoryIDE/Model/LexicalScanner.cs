using System.Collections.Generic;
using System.Linq;

namespace CompilersTheoryIDE.Model;

public class LexicalScanner
{
    private List<Lexeme> Lexemes { get; } = new();
    
    // Analyzes the input string to identify lexemes. Returns a list of Lexeme objects.
    public List<Lexeme> Analyze(string input)
    {
        Lexemes.Clear();
        var i = 0;
        while (i < input.Length)
        {
            var value = string.Empty + input[i];

            if (input[i] == '#')
                Lexemes.Add(new Lexeme(value, i, i, LexemeType.SingleLineComment));
            else if (value == "\n")
                Lexemes.Add(new Lexeme("\\n", i, i, LexemeType.NewLine));
            else if (input.Skip(i).Take(3).SequenceEqual("\"\"\""))
            {
                Lexemes.Add(new Lexeme("\"\"\"", i, i + 2, LexemeType.MultiLineDoubleQuotesComment));
                i += 2;
            }
            else if (input.Skip(i).Take(3).SequenceEqual("'''"))
            {
                Lexemes.Add(new Lexeme("'''", i, i + 2, LexemeType.MultiLineSingleQuotesComment));
                i += 2;
            }
            else
            {
                var j = i + 1;
                while (j < input.Length && input[j] != '#' && input[j] != '\n' && 
                       !input.Skip(j).Take(3).SequenceEqual("\"\"\"") && !input.Skip(j).Take(3).SequenceEqual("'''"))
                {
                    value += input[j];
                    j++;
                }
                Lexemes.Add(new Lexeme(value, i, j - 1, LexemeType.SymbolSequence));
                i = j - 1;
            }

            i++;
        }

        return Lexemes;
    }
}