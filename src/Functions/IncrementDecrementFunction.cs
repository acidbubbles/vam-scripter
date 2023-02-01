using System;

public class IncrementDecrementFunction : ActionFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var prefix = string.IsNullOrEmpty(MName);
        if (prefix) // If it is a prefix we do not have variable name yet.
        {
            MName = Utils.GetToken(data, ref from, Constants.TokenSeparation);
        }

        // Value to be added to the variable:
        var valueDelta = MAction == Constants.Increment ? 1 : -1;
        var returnDelta = prefix ? valueDelta : 0;

        // Check if the variable to be set has the form of x(0),
        // meaning that this is an array element.
        double newValue;
        var arrayIndex = Utils.ExtractArrayElement(ref MName);
        var exists = FunctionExists(MName);
        if (!exists)
        {
            throw new ArgumentException($"Variable [{MName}] doesn't exist");
        }

        var currentValue = GetFunction(MName).GetValue(data, ref from);
        if (arrayIndex >= 0) // A variable with an index (array element).
        {
            if (currentValue.Tuple == null)
            {
                throw new ArgumentException($"Tuple [{MName}] doesn't exist");
            }

            if (currentValue.Tuple.Count <= arrayIndex)
            {
                throw new ArgumentException($"Tuple [{MName}] has only {currentValue.Tuple.Count} elements");
            }

            newValue = currentValue.Tuple[arrayIndex].Value + returnDelta;
            currentValue.Tuple[arrayIndex].Value += valueDelta;
        }
        else // A normal variable.
        {
            newValue = currentValue.Value + returnDelta;
            currentValue.Value += valueDelta;
        }

        var varValue = new Variable(newValue);
        AddGlobalOrLocalVariable(MName, new GetVarFunction(currentValue));

        return varValue;
    }

    override public ParserFunction NewInstance()
    {
        return new IncrementDecrementFunction();
    }
}