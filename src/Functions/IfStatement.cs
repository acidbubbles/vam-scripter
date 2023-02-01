public class IfStatement : ParserFunction
{
    internal IfStatement(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var result = _mInterpreter.ProcessIf(data, ref from);

        return result;
    }

    private readonly Interpreter _mInterpreter;
}