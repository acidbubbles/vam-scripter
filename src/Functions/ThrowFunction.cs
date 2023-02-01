using System;

public class ThrowFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Extract what to throw.
        var arg = Utils.GetItem(data, ref from);

        // 2. Convert it to a string.
        var result = arg.AsString();

        // 3. Throw it!
        throw new ArgumentException(result);
    }
}