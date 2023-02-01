using System;

public class PiFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return new Variable(Math.PI);
    }
}