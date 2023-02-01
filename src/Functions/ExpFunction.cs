using System;

public class ExpFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var result = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        result.Value = Math.Exp(result.Value);
        return result;
    }
}