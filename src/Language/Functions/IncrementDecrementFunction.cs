using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
    class IncrementDecrementFunction : ActionFunction, INumericFunction
    {
        protected bool m_prefix;

        protected override Variable Evaluate(ParsingScript script)
        {
            bool prefix = string.IsNullOrWhiteSpace(m_name);
            if (prefix)
            {// If it is a prefix we do not have the variable name yet.
                Name = Utils.GetToken(script, Constants.TOKEN_SEPARATION);
            }

            Utils.CheckForValidName(Name, script);

            return ProcessAction(m_name, m_action, prefix, script);
        }

        public static Variable ProcessAction(string name, string action, bool isPrefix, ParsingScript script)
        {
            var interpreter = script.InterpreterInstance;

            // Value to be added to the variable:
            int valueDelta = action == Constants.INCREMENT ? 1 : -1;
            int returnDelta = isPrefix ? valueDelta : 0;

            // Check if the variable to be set has the form of x[a][b],
            // meaning that this is an array element.
            double newValue = 0;
            List<Variable> arrayIndices = Utils.GetArrayIndices(script, name, (string _name) => { name = _name; });

            ParserFunction func = interpreter.GetVariable(name, script);
            Utils.CheckNotNull(name, func, script);

            Variable currentValue = func.GetValue(script);
            currentValue = currentValue.DeepClone();

            if (arrayIndices.Count > 0 || script.TryCurrent() == Constants.START_ARRAY)
            {
                if (isPrefix)
                {
                    string tmpName = name + script.Rest;
                    int delta = 0;
                    arrayIndices = Utils.GetArrayIndices(script, tmpName, delta, (string t, int d) => { tmpName = t; delta = d; });
                    script.Forward(Math.Max(0, delta - tmpName.Length));
                }

                Variable element = Utils.ExtractArrayElement(currentValue, arrayIndices, script);
                script.MoveForwardIf(Constants.END_ARRAY);

                newValue = element.Value + returnDelta;
                element.Value += valueDelta;
            }
            else
            { // A normal variable.
                newValue = currentValue.Value + returnDelta;
                currentValue.Value += valueDelta;
            }

            interpreter.AddGlobalOrLocalVariable(name,
                new GetVarFunction(currentValue), script);
            return new Variable(newValue);
        }

        override public ParserFunction NewInstance()
        {
            var newFunc = new IncrementDecrementFunction();
            newFunc.InterpreterInstance = InterpreterInstance;
            return newFunc;
        }
    }
}
