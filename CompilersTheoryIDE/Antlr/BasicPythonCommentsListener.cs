using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CompilersTheoryIDE.Antlr;

public class BasicPythonCommentsListener : PythonCommentsBaseListener
{
    public override void EnterEveryRule([NotNull] ParserRuleContext context)
    {
        base.EnterEveryRule(context);
        // Add your custom logic here
    }

    public override void ExitEveryRule([NotNull] ParserRuleContext context)
    {
        base.ExitEveryRule(context);
        // Add your custom logic here
    }
}