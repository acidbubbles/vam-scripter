using System;

public class PiFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return new Variable(Math.PI);
    }
}

public class ExpFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var result = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        result.Value = Math.Exp(result.Value);
        return result;
    }
}

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

public class SinFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Sin(arg.Value);
        return arg;
    }
}

public class CosFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Cos(arg.Value);
        return arg;
    }
}

public class AsinFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Asin(arg.Value);
        return arg;
    }
}

public class AcosFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Acos(arg.Value);
        return arg;
    }
}

public class SqrtFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Sqrt(arg.Value);
        return arg;
    }
}

public class AbsFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Abs(arg.Value);
        return arg;
    }
}

public class CeilFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Ceiling(arg.Value);
        return arg;
    }
}

public class FloorFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Floor(arg.Value);
        return arg;
    }
}

public class RoundFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Round(arg.Value);
        return arg;
    }
}

public class LogFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        arg.Value = Math.Log(arg.Value);
        return arg;
    }
}
