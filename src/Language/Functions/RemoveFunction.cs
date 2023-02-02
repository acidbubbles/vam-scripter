using System.Collections.Generic;

namespace SplitAndMerge
{
    class RemoveFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // 1. Get the name of the variable.
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name, true);
            string varName = args[0].AsString();

            // 2. Get the current value of the variable.
            ParserFunction func = InterpreterInstance.GetVariable(varName, script);
            Utils.CheckNotNull(varName, func, script);
            Variable currentValue = func.GetValue(script);
            Utils.CheckArray(currentValue, varName);

            // 3. Get the variable to remove.
            Variable item = args[1];

            bool removed = currentValue.Tuple.Remove(item);

            InterpreterInstance.AddGlobalOrLocalVariable(varName,
                new GetVarFunction(currentValue), script);
            return new Variable(removed);
        }
    }
}
