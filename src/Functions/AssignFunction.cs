using System.Collections.Generic;

public class AssignFunction : ActionFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var varValue = Utils.GetItem(data, ref from);

        // Special case for adding a string (or a number) to a string.

        while (varValue.Type != Variable.VarType.Number &&
               from > 0 && data[from - 1] == '+')
        {
            var addition = Utils.GetItem(data, ref from);
            varValue.String += addition.AsString();
        }

        // Check if the variable to be set has the form of x(0),
        // meaning that this is an array element.
        var arrayIndex = Utils.ExtractArrayElement(ref MName);

        if (arrayIndex < 0)
        {
            AddGlobalOrLocalVariable(MName, new GetVarFunction(varValue));
            return varValue;
        }

        Variable currentValue;

        var pf = GetFunction(MName);
        if (pf != null)
        {
            currentValue = pf.GetValue(data, ref from);
        }
        else
        {
            currentValue = Variable.Undefined();
        }

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

        currentValue.Tuple = tuple;

        AddGlobalOrLocalVariable(MName, new GetVarFunction(currentValue));
        return currentValue;
    }

    override public ParserFunction NewInstance()
    {
        return new AssignFunction();
    }
}
