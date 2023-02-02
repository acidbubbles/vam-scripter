using System.Collections.Generic;

namespace SplitAndMerge
{
    class DefineLocalFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            string varName = Utils.GetSafeString(args, 0);
            Variable currentValue = Utils.GetSafeVariable(args, 1);

            if (currentValue == null)
            {
                currentValue = new Variable("");
            }

            if (script.StackLevel != null)
            {
                InterpreterInstance.AddLocalVariable(new GetVarFunction(currentValue), varName);
            }
            else
            {
                InterpreterInstance.AddLocalScopeVariable(varName, null,
                    new GetVarFunction(currentValue));
            }

            return currentValue;
        }
    }
}
