using System.Collections.Generic;

namespace SplitAndMerge
{
    class SizeFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(script, Constants.END_ARG_ARRAY);
            Utils.CheckNotEnd(script, m_name);

            List<Variable> arrayIndices = Utils.GetArrayIndices(script, varName, (newName) => { varName = newName; } );

            // 2. Get the current value of the variable.
            ParserFunction func = InterpreterInstance.GetVariable(varName, script);
            Utils.CheckNotNull(varName, func, script);
            Variable currentValue = func.GetValue(script);
            Variable element = currentValue;

            // 2b. Special case for an array.
            if (arrayIndices.Count > 0)
            {// array element
                element = Utils.ExtractArrayElement(currentValue, arrayIndices, script);
                script.MoveForwardIf(Constants.END_ARRAY);
            }

            // 3. Take either the length of the underlying tuple or
            // string part if it is defined,
            // or the numerical part converted to a string otherwise.
            int size = element.GetSize();

            script.MoveForwardIf(Constants.END_ARG, Constants.SPACE);

            Variable newValue = new Variable(size);
            return newValue;
        }
        public override string Description()
        {
            return "Returns either a number of elements in an array if variable is of type ARRAY or a number of characters in a string representation of this variable.";
        }
    }
}
