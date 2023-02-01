using System;

public class ToUpperFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.EndArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't get variable");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var arg = currentValue.AsString();

        var newValue = Variable.CreateString(arg.ToUpper());
        return newValue;
    }
}
