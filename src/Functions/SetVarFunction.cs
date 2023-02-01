using System;
using System.Collections.Generic;

public class SetVarFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't set variable before end of line");
        }

        var varValue = Utils.GetItem(data, ref from);

        // Check if the variable to be set has the form of x(0),
        // meaning that this is an array element.
        var arrayIndex = Utils.ExtractArrayElement(ref varName);
        if (arrayIndex >= 0)
        {
            var exists = FunctionExists(varName);
            var currentValue = exists ? GetFunction(varName).GetValue(data, ref from) : Variable.EmptyInstance;

            var tuple = currentValue.Tuple ?? new List<Variable>();
            if (tuple.Count > arrayIndex)
            {
                tuple[arrayIndex] = varValue;
            }
            else
            {
                for (var i = tuple.Count; i < arrayIndex; i++)
                {
                    tuple.Add(Variable.EmptyInstance);
                }

                tuple.Add(varValue);
            }

            varValue = new Variable(tuple);
        }

        AddGlobalOrLocalVariable(varName, new GetVarFunction(varValue));

        return varValue;
    }
}