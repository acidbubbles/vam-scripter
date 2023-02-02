using System.Collections.Generic;

namespace SplitAndMerge
{
    public class GetNativeFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            string name = Utils.GetSafeString(args, 0);
            var objValue = Statics.GetVariableValue(name, script);

            return new Variable(objValue.ToString());
        }
    }
}
