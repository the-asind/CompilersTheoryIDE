using System.Collections.Generic;
using System.Linq;

namespace CompilersTheoryIDE.Model;

public class Parser
{
    private enum ParserState
    {
        Default,
        SingleLineComment,
        MultiLineDoubleQuotesComment,
        MultiLineSingleQuotesComment
    }

    private int _currentLine = 1;
    private int _currentPosition;

    public IEnumerable<ParserError> Parse(IEnumerable<Lexeme> tokens)
    {
        var currentState = ParserState.Default;
        Lexeme lastToken = null;

        foreach (var token in tokens)
        {
            lastToken = token;
            _currentPosition = token.Value.Length;
            switch (currentState)
            {
                case ParserState.Default:
                    switch (token)
                    {
                        case { Type: LexemeType.SingleLineComment }:
                            currentState = ParserState.SingleLineComment;
                            break;
                        case { Type: LexemeType.MultiLineDoubleQuotesComment }:
                            currentState = ParserState.MultiLineDoubleQuotesComment;
                            break;
                        case { Type: LexemeType.MultiLineSingleQuotesComment }:
                            currentState = ParserState.MultiLineSingleQuotesComment;
                            break;
                        case { Type: LexemeType.NewLine }:
                            break; // Ignore NewLine tokens in the default state
                        case { Type: LexemeType.SymbolSequence }:
                            yield return new ParserError
                            {
                                ErrorLocation = 
                                    $"{token.IndexStart}-{token.IndexStart+_currentPosition}",
                                ErrorName = "Unexpected Symbol Sequence",
                                ErrorFragment = token.Value
                            };
                            break;
                    }

                    break;

                case ParserState.SingleLineComment:
                    switch (token)
                    {
                        case { Type: LexemeType.NewLine }:
                            currentState = ParserState.Default;
                            break;
                        case { Type: LexemeType.SingleLineComment }:
                        case { Type: LexemeType.MultiLineDoubleQuotesComment }:
                        case { Type: LexemeType.MultiLineSingleQuotesComment }:
                        case { Type: LexemeType.SymbolSequence }:
                            break; // Ignore other token types in the SingleLineComment state
                    }

                    break;

                case ParserState.MultiLineDoubleQuotesComment:
                    switch (token)
                    {
                        case { Type: LexemeType.MultiLineDoubleQuotesComment }:
                            currentState = ParserState.Default;
                            break;
                        case { Type: LexemeType.SingleLineComment }:
                        case { Type: LexemeType.MultiLineSingleQuotesComment }:
                        case { Type: LexemeType.NewLine }:
                        case { Type: LexemeType.SymbolSequence }:
                            break; // Ignore other token types in the MultiLineDoubleQuotesComment state
                    }

                    break;

                case ParserState.MultiLineSingleQuotesComment:
                    switch (token)
                    {
                        case { Type: LexemeType.MultiLineSingleQuotesComment }:
                            currentState = ParserState.Default;
                            break;
                        case { Type: LexemeType.SingleLineComment }:
                        case { Type: LexemeType.MultiLineDoubleQuotesComment }:
                        case { Type: LexemeType.NewLine }:
                        case { Type: LexemeType.SymbolSequence }:
                            break; // Ignore other token types in the MultiLineSingleQuotesComment state
                    }

                    break;
            }
        }
        
        // Check for unclosed comments at the end
        if (currentState != ParserState.Default && currentState != ParserState.SingleLineComment)
            yield return new ParserError
            {
                ErrorLocation = 
                    lastToken != null ? $"{lastToken.IndexStart}-{lastToken.IndexStart+_currentPosition}" : "Unknown",
                ErrorName = $"Unclosed {currentState}",
                ErrorFragment = null
            };
    }
}

public class ParserError
{
    public string? ErrorLocation { get; set; }
    public string? ErrorName { get; set; }
    public string? ErrorFragment { get; set; }
}