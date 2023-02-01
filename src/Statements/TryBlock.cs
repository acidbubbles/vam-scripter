public class TryBlock : ParserFunction
{
    internal TryBlock(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        return _mInterpreter.ProcessTry(data, ref from);
    }

    private readonly Interpreter _mInterpreter;
}