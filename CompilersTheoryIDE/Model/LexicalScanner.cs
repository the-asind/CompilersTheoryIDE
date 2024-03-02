using System.Collections.Generic;
using System.Linq;

namespace CompilersTheoryIDE.Model;

public class LexicalScanner
{
    public List<Lexeme> Lexemes { get; private set; } = new List<Lexeme>();
    
    // Analyzes the input string to identify lexemes. Returns a list of Lexeme objects.
    public List<Lexeme> Analyze(string input)
    {
        Lexemes.Clear();
        int currentPosition = 0;

        while (currentPosition < input.Length)
        {
            if (IsSingleLineCommentStart(input, currentPosition))
            {
                AddLexeme(input, currentPosition, currentPosition, LexemeType.SingleLineComment);
            }
            else if (IsNewLine(input[currentPosition]))
            {
                AddLexeme(input, currentPosition, currentPosition, LexemeType.NewLine);
            }
            else if (IsMultiLineDoubleQuotesCommentStart(input, currentPosition))
            {
                AddLexeme(input, currentPosition, currentPosition + 2, LexemeType.MultiLineDoubleQuotesComment);
                currentPosition += 2;
            }
            else if (IsMultiLineSingleQuotesCommentStart(input, currentPosition))
            {
                AddLexeme(input, currentPosition, currentPosition + 2, LexemeType.MultiLineSingleQuotesComment);
                currentPosition += 2;
            }
            else
            {
                currentPosition = HandleSymbolSequence(input, currentPosition);
            }

            currentPosition++;
        }

        return Lexemes;
    }

    private void AddLexeme(string input, int start, int end, LexemeType type)
    {
        string value = input.Substring(start, end - start + 1);
        Lexemes.Add(new Lexeme(value, start, end, type));
    }

    private int HandleSymbolSequence(string input, int start)
    {
        int end = start;

        while (end < input.Length && !IsSingleLineCommentStart(input, end) && !IsNewLine(input[end]) && !IsMultiLineDoubleQuotesCommentStart(input, end) && !IsMultiLineSingleQuotesCommentStart(input, end))
        {
            end++;
        }

        AddLexeme(input, start, end - 1, LexemeType.SymbolSequence);
        return end - 1;
    }

    private bool IsSingleLineCommentStart(string input, int position)
    {
        return input[position] == '#';
    }

    private bool IsNewLine(char character)
    {
        return character == '\n';
    }

    private bool IsMultiLineDoubleQuotesCommentStart(string input, int position)
    {
        return position + 2 < input.Length && input.Substring(position, 3) == "\"\"\"";
    }

    private bool IsMultiLineSingleQuotesCommentStart(string input, int position)
    {
        return position + 2 < input.Length && input.Substring(position, 3) == "'''";
    }
}