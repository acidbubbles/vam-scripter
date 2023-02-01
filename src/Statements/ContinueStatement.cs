public class ContinueStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return new Variable(Variable.VarType.Continue);
    }
}
