using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
	class ContinueStatement : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{

      return new Variable(Variable.VarType.CONTINUE);
		}
	}

	class BreakStatement : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
      return new Variable(Variable.VarType.BREAK);
		}
	}

  class ReturnStatement : ParserFunction
  {
    
    protected override Variable Evaluate(string data, ref int from)
    {
      Utils.MoveForwardIf(data, ref from, Constants.SPACE);

      Variable result = Utils.GetItem(data, ref from);

      // If we are in Return, we are done:
      from = data.Length;

      return result;
    }
  }

  class TryBlock : ParserFunction
  {
    internal TryBlock(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      return m_interpreter.ProcessTry(data, ref from);
    }

    private Interpreter m_interpreter;
  }

  class ThrowFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Extract what to throw.
      Variable arg = Utils.GetItem(data, ref from);

      // 2. Convert it to a string.
      string result = arg.AsString();

      // 3. Throw it!
      throw new ArgumentException(result);
    }
  }

  class FunctionCreator : ParserFunction
  {
    internal FunctionCreator(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string funcName = Utils.GetToken(data, ref from, Constants.TOKEN_SEPARATION);
      m_interpreter.AppendOutput("Registering function [" + funcName + "] ...");

      string[] args = Utils.GetFunctionSignature(data, ref from);
      if (args.Length == 1 && string.IsNullOrWhiteSpace(args[0])) {
        args = new string[0];
      }

      Utils.MoveForwardIf(data, ref from, Constants.START_GROUP, Constants.SPACE);

      string body = Utils.GetBodyBetween(data, ref from, Constants.START_GROUP, Constants.END_GROUP);

      CustomFunction customFunc = new CustomFunction(funcName, body, args);
      ParserFunction.AddGlobal(funcName, customFunc);

      return new Variable(funcName);
    }

    private Interpreter m_interpreter;
  }

  class CustomFunction : ParserFunction
  {
    internal CustomFunction(string funcName,
                            string body, string[] args)
    {
      m_name = funcName;
      m_body = body;
      m_args = args;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      bool isList;
      List<Variable> functionArgs = Utils.GetArgs(data, ref from,
        Constants.START_ARG, Constants.END_ARG, out isList);

      Utils.MoveBackIf(data, ref from, Constants.START_GROUP);
      if (functionArgs.Count != m_args.Length) {   
        throw new ArgumentException("Function [" + m_name + "] arguments mismatch: " +
          m_args.Length + " declared, " + functionArgs.Count + " supplied");
      }

      // 1. Add passed arguments as local variables to the Parser.
      StackLevel stackLevel = new StackLevel(m_name);
      for (int i = 0; i < m_args.Length; i++) {
        stackLevel.Variables[m_args[i]] = new GetVarFunction(functionArgs[i]);
      }

      ParserFunction.AddLocalVariables(stackLevel);

      // 2. Execute the body of the function.
      int temp = 0;
      Variable result = null;

      while (temp < m_body.Length - 1)
      { 
        result = Parser.LoadAndCalculate(m_body, ref temp, Constants.END_PARSE_ARRAY);
        Utils.GoToNextStatement(m_body, ref temp);
      }

      ParserFunction.PopLocalVariables();
      return result;
    }

    private string      m_body;
    private string[]    m_args;
  }

 	class StringOrNumberFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			// First check if the passed expression is a string between quotes.
			if (Item.Length > 1 &&
				Item[0] == Constants.QUOTE &&
				Item[Item.Length - 1]  == Constants.QUOTE) {
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

	class IdentityFunction : ParserFunction
  {
		protected override Variable Evaluate(string data, ref int from)
		{
			return Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
		}
	}

  class IfStatement : ParserFunction
  {
    internal IfStatement(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      Variable result = m_interpreter.ProcessIf(data, ref from);

      return result;
    }

    private Interpreter m_interpreter;
  }

  class WhileStatement : ParserFunction
  {
    internal WhileStatement(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      return m_interpreter.ProcessWhile(data, ref from);

      //return Variable.EmptyInstance;
    }

    private Interpreter m_interpreter;
  }

  class IncludeFile : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetItem(data, ref from).AsString();
      string[] lines = Utils.GetFileLines(filename);

      string includeFile = string.Join(Environment.NewLine, lines);
      string includeScript = Utils.ConvertToScript(includeFile);

      int filePtr = 0;
      while (filePtr < includeScript.Length)
      {
        Parser.LoadAndCalculate(includeScript, ref filePtr,
                                Constants.END_LINE_ARRAY);
        Utils.GoToNextStatement(includeScript, ref filePtr);
      }
      return Variable.EmptyInstance;
    }
  }

  // Get a value of a variable or of an array element
  class GetVarFunction : ParserFunction
  {
    internal GetVarFunction(Variable value)
    {
      m_value = value;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      // First check if this element is part of an array:
      if (from - 1 < data.Length && data[from - 1] == Constants.START_ARRAY)
      {
        // There is an index given - it may be for an element of the tuple.
        if (m_value.Tuple == null || m_value.Tuple.Count == 0)
        {
          throw new ArgumentException("No tuple exists for the index");
        }

        Variable index = Parser.LoadAndCalculate(data, ref from,
          Constants.END_ARRAY_ARRAY);

        //Variable index = Utils.GetItem(data, ref from);
        Utils.CheckInteger(index);

        if (index.Value < 0 || index.Value >= m_value.Tuple.Count)
        {
          throw new ArgumentException("Incorrect index [" + index.Value +
            "] for tuple of size " + m_value.Tuple.Count);
        }

        Utils.MoveForwardIf(data, ref from, Constants.END_ARRAY);
        return m_value.Tuple[(int)index.Value];
      }

      return m_value;
    }

    private Variable m_value;
  }

  class IncrementDecrementFunction : ActionFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      bool prefix = string.IsNullOrWhiteSpace(m_name);
      if (prefix) // If it is a prefix we do not have variable name yet.
      {
        m_name = Utils.GetToken(data, ref from, Constants.TOKEN_SEPARATION);

      }

      // Value to be added to the variable:
      int valueDelta = m_action == Constants.INCREMENT ? 1 : -1;
      int returnDelta = prefix ? valueDelta : 0;

      // Check if the variable to be set has the form of x(0),
      // meaning that this is an array element.
      double newValue = 0;
      int arrayIndex = Utils.ExtractArrayElement(ref m_name);
      bool exists = ParserFunction.FunctionExists(m_name);
      if (!exists)
      {
        throw new ArgumentException("Variable [" + m_name + "] doesn't exist");
      }

      Variable currentValue = ParserFunction.GetFunction(m_name).GetValue(data, ref from);
      if (arrayIndex >= 0) // A variable with an index (array element).
      {
        if (currentValue.Tuple == null)
        {
          throw new ArgumentException("Tuple [" + m_name + "] doesn't exist");
        }
        if (currentValue.Tuple.Count <= arrayIndex)
        {
          throw new ArgumentException("Tuple [" + m_name + "] has only " +
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

      Variable varValue = new Variable(newValue);
      ParserFunction.AddGlobalOrLocalVariable(m_name, new GetVarFunction(currentValue));

      return varValue;
    }

    override public ParserFunction NewInstance()
    {
      return new IncrementDecrementFunction();
    }
  }

  class OperatorAssignFunction : ActionFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // Value to be added to the variable:
      Variable valueB  = Utils.GetItem(data, ref from);

      // Check if the variable to be set has the form of x(0),
      // meaning that this is an array element.
      int arrayIndex = Utils.ExtractArrayElement(ref m_name);
      bool exists = ParserFunction.FunctionExists(m_name);
      if (!exists)
      {
        throw new ArgumentException("Variable [" + m_name + "] doesn't exist");
      }

      Variable currentValue = ParserFunction.GetFunction(m_name).GetValue(data, ref from);
      Variable valueA = currentValue;
      if (arrayIndex >= 0) // A variable with an index.
      {
        if (currentValue.Tuple == null)
        {
          throw new ArgumentException("Tuple [" + m_name + "] doesn't exist");
        }
        if (currentValue.Tuple.Count <= arrayIndex)
        {
          throw new ArgumentException("Tuple [" + m_name + "] has only " +
            currentValue.Tuple.Count + " elements");
        }
        valueA = currentValue.Tuple[arrayIndex];//.Value;
      }

      if (valueA.Type == Variable.VarType.NUMBER)
      {
        NumberOperator (valueA, valueB, m_action);
      }
      else
      {
        StringOperator (valueA, valueB, m_action);
      }

      Variable varValue = new Variable(valueA);
      ParserFunction.AddGlobalOrLocalVariable(m_name, new GetVarFunction(varValue));
      return valueA;
    }

    static void NumberOperator(Variable valueA,
                               Variable valueB, string action)
    {
      switch (action) {
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
        if (valueB.Type == Variable.VarType.STRING) {
          valueA.String += valueB.AsString();
        } else {
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

  class AssignFunction : ActionFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      Variable varValue = Utils.GetItem(data, ref from);

      // Special case for adding a string (or a number) to a string.

      while (varValue.Type != Variable.VarType.NUMBER &&
             from > 0 && data[from - 1] == '+') {
        Variable addition = Utils.GetItem(data, ref from);
        varValue.String += addition.AsString();
      }

      // Check if the variable to be set has the form of x(0),
      // meaning that this is an array element.
      int arrayIndex = Utils.ExtractArrayElement(ref m_name);

      if (arrayIndex < 0) {
        ParserFunction.AddGlobalOrLocalVariable(m_name, new GetVarFunction(varValue));
        return varValue;
      }

      Variable currentValue;
 
      ParserFunction pf = ParserFunction.GetFunction(m_name);
      if (pf != null) {
        currentValue = pf.GetValue(data, ref from);
      } else {
        currentValue = new Variable();
      }

      List<Variable> tuple = currentValue.Tuple == null ?
                            new List<Variable>() :
                            currentValue.Tuple;
      if (tuple.Count > arrayIndex) {
          tuple[arrayIndex] = varValue;
      } else {
          for (int i = tuple.Count; i < arrayIndex; i++) {
            tuple.Add(Variable.EmptyInstance);
          }
          tuple.Add(varValue);
      }
      currentValue.Tuple = tuple;

      ParserFunction.AddGlobalOrLocalVariable(m_name, new GetVarFunction(currentValue));
      return currentValue;
    }

    override public ParserFunction NewInstance()
    {
      return new AssignFunction();
    }
  }

  class SetVarFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't set variable before end of line");
      }

      Variable varValue = Utils.GetItem(data, ref from);

      // Check if the variable to be set has the form of x(0),
      // meaning that this is an array element.
      int arrayIndex = Utils.ExtractArrayElement(ref varName);
      if (arrayIndex >= 0)
      {
        bool exists = ParserFunction.FunctionExists(varName);
        Variable currentValue = exists ?
          ParserFunction.GetFunction(varName).GetValue(data, ref from) :
          Variable.EmptyInstance;

        List<Variable> tuple = currentValue.Tuple == null ?
          new List<Variable>() :
          currentValue.Tuple;
        if (tuple.Count > arrayIndex)
        {
          tuple[arrayIndex] = varValue;
        }
        else
        {
          for (int i = tuple.Count; i < arrayIndex; i++)
          {
            tuple.Add(Variable.EmptyInstance);
          }
          tuple.Add(varValue);
        }

        varValue = new Variable(tuple);
      }

      ParserFunction.AddGlobalOrLocalVariable(varName, new GetVarFunction(varValue));

      return varValue;
    }
  }

  class SizeFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't get variable");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Take either the length of the underlying tuple or
      // string part if it is defined,
      // or the numerical part converted to a string otherwise.
      int size = currentValue.Tuple != null ? currentValue.Tuple.Count : 
        currentValue.AsString().Length;


      Utils.MoveForwardIf(data, ref from, Constants.END_ARG, Constants.SPACE);

      Variable newValue = new Variable(size);
      return newValue;
    }
  }

}
