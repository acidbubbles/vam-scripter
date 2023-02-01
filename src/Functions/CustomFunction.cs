using System;

public class CustomFunction : ParserFunction
{
    internal CustomFunction(string funcName,
        string body, string[] args)
    {
        MName = funcName;
        _mBody = body;
        _mArgs = args;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        bool isList;
        var functionArgs = Utils.GetArgs(data, ref from,
            Constants.StartArg, Constants.EndArg, out isList);

        Utils.MoveBackIf(data, ref from, Constants.StartGroup);
        if (functionArgs.Count != _mArgs.Length)
        {
            throw new ArgumentException($"Function [{MName}] arguments mismatch: {_mArgs.Length} declared, {functionArgs.Count} supplied");
        }

        // 1. Add passed arguments as local variables to the Parser.
        var stackLevel = new StackLevel(MName);
        for (var i = 0; i < _mArgs.Length; i++)
        {
            stackLevel.Variables[_mArgs[i]] = new GetVarFunction(functionArgs[i]);
        }

        AddLocalVariables(stackLevel);

        // 2. Execute the body of the function.
        var temp = 0;
        Variable result = null;

        while (temp < _mBody.Length - 1)
        {
            result = Parser.LoadAndCalculate(_mBody, ref temp, Constants.EndParseArray);
            Utils.GoToNextStatement(_mBody, ref temp);
        }

        PopLocalVariables();
        return result;
    }

    private readonly string _mBody;
    private readonly string[] _mArgs;
}