using System;

public class SubstrFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        string substring;

        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
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
        // 4. Get the initial index of the substring.
        var init = Utils.GetItem(data, ref from);
        Utils.CheckNonNegativeInt(init);

        // 5. Get the length of the substring if available.
        var lengthAvailable = Utils.SeparatorExists(data, from);
        if (lengthAvailable)
        {
            var length = Utils.GetItem(data, ref from);
            Utils.CheckPosInt(length);
            if (init.Value + length.Value > arg.Length)
            {
                throw new ArgumentException($"The total substring length is larger than [{arg}]");
            }

            substring = arg.Substring((int)init.Value, (int)length.Value);
        }
        else
        {
            substring = arg.Substring((int)init.Value);
        }

        var newValue = Variable.CreateString(substring);

        return newValue;
    }
}