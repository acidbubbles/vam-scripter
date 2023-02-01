using System;

public class FloorFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Floor(arg.Value);
        return arg;
    }
}