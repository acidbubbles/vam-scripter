public class WhileStatement : ParserFunction
{
    internal WhileStatement(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        return _mInterpreter.ProcessWhile(data, ref from);

        //return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
}