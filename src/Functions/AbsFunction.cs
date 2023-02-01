using System;

public class AbsFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Abs(arg.Value);
        return arg;
    }
}