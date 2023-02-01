using System;

public class OperatorAssignFunction : ActionFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // Value to be added to the variable:
        var valueB = Utils.GetItem(data, ref from);

        // Check if the variable to be set has the form of x(0),
        // meaning that this is an array element.
        var arrayIndex = Utils.ExtractArrayElement(ref MName);
        var exists = FunctionExists(MName);
        if (!exists)
        {
            throw new ArgumentException($"Variable [{MName}] doesn't exist");
        }

        var currentValue = GetFunction(MName).GetValue(data, ref from);
        var valueA = currentValue;
        if (arrayIndex >= 0) // A variable with an index.
        {
            if (currentValue.Tuple == null)
            {
                throw new ArgumentException($"Tuple [{MName}] doesn't exist");
            }

            if (currentValue.Tuple.Count <= arrayIndex)
            {
                throw new ArgumentException($"Tuple [{MName}] has only {currentValue.Tuple.Count} elements");
            }

            valueA = currentValue.Tuple[arrayIndex]; //.Value;
        }

        if (valueA.Type == Variable.VarType.Number)
        {
            NumberOperator(valueA, valueB, MAction);
        }
        else
        {
            StringOperator(valueA, valueB, MAction);
        }

        var varValue = Variable.Copy(valueA);
        AddGlobalOrLocalVariable(MName, new GetVarFunction(varValue));
        return valueA;
    }

    private static void NumberOperator(Variable valueA,
        Variable valueB, string action)
    {
        switch (action)
        {
            case "+=":
                valueA.Value += valueB.Value;
                break;
            case "-=":
                valueA.Value -= valueB.Value;
                break;
            case "*=":
                valueA.Value *= valueB.Value;
                break;
            case "/=":
                valueA.Value /= valueB.Value;
                break;
            case "%=":
                valueA.Value %= valueB.Value;
                break;
            case "&=":
                valueA.Value = (int)valueA.Value & (int)valueB.Value;
                break;
            case "|=":
                valueA.Value = (int)valueA.Value | (int)valueB.Value;
                break;
            case "^=":
                valueA.Value = (int)valueA.Value ^ (int)valueB.Value;
                break;
        }
    }

    private void StringOperator(Variable valueA,
        Variable valueB, string action)
    {
        switch (action)
        {
            case "+=":
                if (valueB.Type == Variable.VarType.String)
                {
                    valueA.String += valueB.AsString();
                }
                else
                {
                    valueA.String += valueB.Value;
                }

                break;
        }
    }

    override public ParserFunction NewInstance()
    {
        return new OperatorAssignFunction();
    }
}
