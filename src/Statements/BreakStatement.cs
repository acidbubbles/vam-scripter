public class BreakStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return Variable.OfType(Variable.VarType.Break);
    }
}
