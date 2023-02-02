using System.Collections.Generic;

namespace SplitAndMerge
{
    class OperatorAssignFunction : ActionFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return ProcessOperator(m_name, m_action, script);
        }

        public static Variable ProcessOperator(string name, string action, ParsingScript script)
        {
            var interpreter = script.InterpreterInstance;
            // Value to be added to the variable:
            Variable right = Utils.GetItem(script);

            List<Variable> arrayIndices = Utils.GetArrayIndices(script, name, (string _name) => { name = _name; });

            ParserFunction func = interpreter.GetVariable(name, script);
            Utils.CheckNotNull(func, name, script);

            Variable currentValue = func.GetValue(script);
            #warning
            //currentValue = currentValue.DeepClone();
            Variable left = currentValue;

            if (arrayIndices.Count > 0)
            {// array element
                left = Utils.ExtractArrayElement(currentValue, arrayIndices, script);
                script.MoveForwardIf(Constants.END_ARRAY);
            }

            ProcessOperator(left, right, action, script, name);

            if (arrayIndices.Count > 0)
            {// array element
                AssignFunction.ExtendArray(currentValue, arrayIndices, 0, left);
                interpreter.AddGlobalOrLocalVariable(name,
                    new GetVarFunction(currentValue), script);
            }
            else
            {
                interpreter.AddGlobalOrLocalVariable(name,
                    new GetVarFunction(left), script);
            }
            return left;
        }

        public static void ProcessOperator(Variable left, Variable right, string action,
            ParsingScript script = null, string name = "")
        {
            if (left.Type == Variable.VarType.NUMBER)
            {
                NumberOperator(left, right, action);
            }
            else
            {
                StringOperator(left, right, action);
            }
        }

        public static void NumberOperator(Variable valueA,
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
        public static void StringOperator(Variable valueA,
            Variable valueB, string action)
        {
            switch (action)
            {
                case "+=":
                    if (valueB.Type == Variable.VarType.STRING)
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
            var newFunc = new OperatorAssignFunction();
            newFunc.InterpreterInstance = InterpreterInstance;
            return newFunc;
        }
    }
}
