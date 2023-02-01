using System;

public class RoundFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Round(arg.Value);
        return arg;
    }
}