using System.Collections.Generic;

namespace SplitAndMerge
{
    public class SetNativeFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            string name  = Utils.GetSafeString(args, 0);
            string value = Utils.GetSafeString(args, 1);
            bool isSet   = Statics.SetVariableValue(name, value, script);

            return new Variable(isSet);
        }
    }
}
