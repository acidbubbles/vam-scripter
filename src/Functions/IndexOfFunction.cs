using System;

public class IndexOfFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't extract variable name");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Get the value to be looked for.
        var searchValue = Utils.GetItem(data, ref from);

        // 4. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var basePart = currentValue.AsString();
        var search = searchValue.AsString();

        var result = basePart.IndexOf(search);
        return new Variable(result);
    }
}

// Get a substring of a string


// Convert a string to the upper case

// Convert a string to the lower case