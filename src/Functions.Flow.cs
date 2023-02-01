using System;
using System.Collections.Generic;

public class ContinueStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return new Variable(Variable.VarType.Continue);
    }
}

public class BreakStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return new Variable(Variable.VarType.Break);
    }
}

public class ReturnStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        Utils.MoveForwardIf(data, ref from, Constants.Space);

        var result = Utils.GetItem(data, ref from);

        // If we are in Return, we are done:
        from = data.Length;

        return result;
    }
}

public class TryBlock : ParserFunction
{
    internal TryBlock(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        return _mInterpreter.ProcessTry(data, ref from);
    }

    private readonly Interpreter _mInterpreter;
}

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

public class FunctionCreator : ParserFunction
{
    internal FunctionCreator(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var funcName = Utils.GetToken(data, ref from, Constants.TokenSeparation);
        _mInterpreter.AppendOutput("Registering function [" + funcName + "] ...");

        var args = Utils.GetFunctionSignature(data, ref from);
        if (args.Length == 1 && string.IsNullOrEmpty(args[0]))
        {
            args = new string[0];
        }

        Utils.MoveForwardIf(data, ref from, Constants.StartGroup, Constants.Space);

        var body = Utils.GetBodyBetween(data, ref from, Constants.StartGroup, Constants.EndGroup);

        var customFunc = new CustomFunction(funcName, body, args);
        AddGlobal(funcName, customFunc);

        return new Variable(funcName);
    }

    private readonly Interpreter _mInterpreter;
}

public class CustomFunction : ParserFunction
{
    internal CustomFunction(string funcName,
        string body, string[] args)
    {
        MName = funcName;
        _mBody = body;
        _mArgs = args;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        bool isList;
        var functionArgs = Utils.GetArgs(data, ref from,
            Constants.StartArg, Constants.EndArg, out isList);

        Utils.MoveBackIf(data, ref from, Constants.StartGroup);
        if (functionArgs.Count != _mArgs.Length)
        {
            throw new ArgumentException("Function [" + MName + "] arguments mismatch: " +
                                        _mArgs.Length + " declared, " + functionArgs.Count + " supplied");
        }

        // 1. Add passed arguments as local variables to the Parser.
        var stackLevel = new StackLevel(MName);
        for (var i = 0; i < _mArgs.Length; i++)
        {
            stackLevel.Variables[_mArgs[i]] = new GetVarFunction(functionArgs[i]);
        }

        AddLocalVariables(stackLevel);

        // 2. Execute the body of the function.
        var temp = 0;
        Variable result = null;

        while (temp < _mBody.Length - 1)
        {
            result = Parser.LoadAndCalculate(_mBody, ref temp, Constants.EndParseArray);
            Utils.GoToNextStatement(_mBody, ref temp);
        }

        PopLocalVariables();
        return result;
    }

    private readonly string _mBody;
    private readonly string[] _mArgs;
}

public class StringOrNumberFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // First check if the passed expression is a string between quotes.
        if (Item.Length > 1 &&
            Item[0] == Constants.Quote &&
            Item[Item.Length - 1] == Constants.Quote)
        {
            return new Variable(Item.Substring(1, Item.Length - 2));
        }


        // Otherwise this should be a number.
        double num;
        if (!Double.TryParse(Item, out num))
        {
            throw new ArgumentException("Couldn't parse token [" + Item + "]");
        }

        return new Variable(num);
    }

    public string Item { private get; set; }
}

public class IdentityFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
    }
}

public class IfStatement : ParserFunction
{
    internal IfStatement(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var result = _mInterpreter.ProcessIf(data, ref from);

        return result;
    }

    private readonly Interpreter _mInterpreter;
}

public class WhileStatement : ParserFunction
{
    internal WhileStatement(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        return _mInterpreter.ProcessWhile(data, ref from);

        //return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
}

// Get a value of a variable or of an array element
public class GetVarFunction : ParserFunction
{
    internal GetVarFunction(Variable value)
    {
        _mValue = value;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        // First check if this element is part of an array:
        if (from - 1 < data.Length && data[from - 1] == Constants.StartArray)
        {
            // There is an index given - it may be for an element of the tuple.
            if (_mValue.Tuple == null || _mValue.Tuple.Count == 0)
            {
                throw new ArgumentException("No tuple exists for the index");
            }

            var index = Parser.LoadAndCalculate(data, ref from,
                Constants.EndArrayArray);

            //Variable index = Utils.GetItem(data, ref from);
            Utils.CheckInteger(index);

            if (index.Value < 0 || index.Value >= _mValue.Tuple.Count)
            {
                throw new ArgumentException("Incorrect index [" + index.Value +
                                            "] for tuple of size " + _mValue.Tuple.Count);
            }

            Utils.MoveForwardIf(data, ref from, Constants.EndArray);
            return _mValue.Tuple[(int)index.Value];
        }

        return _mValue;
    }

    private readonly Variable _mValue;
}

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
            throw new ArgumentException("Variable [" + MName + "] doesn't exist");
        }

        var currentValue = GetFunction(MName).GetValue(data, ref from);
        if (arrayIndex >= 0) // A variable with an index (array element).
        {
            if (currentValue.Tuple == null)
            {
                throw new ArgumentException("Tuple [" + MName + "] doesn't exist");
            }

            if (currentValue.Tuple.Count <= arrayIndex)
            {
                throw new ArgumentException("Tuple [" + MName + "] has only " +
                                            currentValue.Tuple.Count + " elements");
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
            throw new ArgumentException("Variable [" + MName + "] doesn't exist");
        }

        var currentValue = GetFunction(MName).GetValue(data, ref from);
        var valueA = currentValue;
        if (arrayIndex >= 0) // A variable with an index.
        {
            if (currentValue.Tuple == null)
            {
                throw new ArgumentException("Tuple [" + MName + "] doesn't exist");
            }

            if (currentValue.Tuple.Count <= arrayIndex)
            {
                throw new ArgumentException("Tuple [" + MName + "] has only " +
                                            currentValue.Tuple.Count + " elements");
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

        var varValue = new Variable(valueA);
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
            currentValue = new Variable();
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

public class SizeFunction : ParserFunction
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

        // 3. Take either the length of the underlying tuple or
        // string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var size = currentValue.Tuple?.Count ?? currentValue.AsString().Length;


        Utils.MoveForwardIf(data, ref from, Constants.EndArg, Constants.Space);

        var newValue = new Variable(size);
        return newValue;
    }
}
