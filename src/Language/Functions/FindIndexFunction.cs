using System.Collections.Generic;

namespace SplitAndMerge
{
    class FindIndexFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            Variable var = Utils.GetSafeVariable(args, 0);
            string val = Utils.GetSafeString(args, 1);

            int index = var.FindIndex(val);

            return new Variable(index);
        }
    }
}
