using System;

public class AsinFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Asin(arg.Value);
        return arg;
    }
}