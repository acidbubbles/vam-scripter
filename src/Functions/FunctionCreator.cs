public class FunctionCreator : ParserFunction
{
    internal FunctionCreator(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var funcName = Utils.GetToken(data, ref from, Constants.TokenSeparation);

        var args = Utils.GetFunctionSignature(data, ref from);
        if (args.Length == 1 && string.IsNullOrEmpty(args[0]))
        {
            args = new string[0];
        }

        Utils.MoveForwardIf(data, ref from, Constants.StartGroup, Constants.Space);

        var body = Utils.GetBodyBetween(data, ref from, Constants.StartGroup, Constants.EndGroup);

        var customFunc = new CustomFunction(funcName, body, args);
        AddGlobal(funcName, customFunc);

        return new Variable(funcName);
    }

    private readonly Interpreter _mInterpreter;
}