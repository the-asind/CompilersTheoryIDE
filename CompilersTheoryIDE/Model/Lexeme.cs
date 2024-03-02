using System.Collections.Generic;

namespace CompilersTheoryIDE.Model;

public enum LexemeType
{
    SingleLineComment = 1,
    MultiLineDoubleQuotesComment = 2,
    MultiLineSingleQuotesComment = 3,
    NewLine = 4,
    SymbolSequence = 5
}

public class Lexeme
{
    public int LexemeId => (int)Type;
    public string LexemeName => lexemeNames[LexemeId - 1];
    public string Value { get; set; }
    public int IndexStart { get; set; }
    public int IndexEnd { get; set; }
    public LexemeType Type { get; set; }
    public string Location => $"{IndexStart};{IndexEnd}";
    
    private List<string> lexemeNames;
    
    public Lexeme(string value, int positionStart, int positionEnd, LexemeType type)
    {
        Value = value;
        IndexStart = positionStart;
        IndexEnd = positionEnd;
        Type = type;
        
        lexemeNames = new List<string>
        {
            "m_SingleLineComment",
            "m_MultiLineDoubleQuotesComment",
            "m_MultiLineSingleQuotesComment",
            "m_NewLine",
            "m_SymbolSequence"
        };
    }
}