using System;

public class PowFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg1 = Parser.LoadAndCalculate(data, ref from, Constants.NextArgArray);
        from++; // eat separation
        var arg2 = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);

        arg1.Value = Math.Pow(arg1.Value, arg2.Value);
        return arg1;
    }
}