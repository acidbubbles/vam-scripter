namespace SplitAndMerge
{
    public class InvokeNativeFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            string methodName = Utils.GetItem(script).AsString();
            Utils.CheckNotEmpty(script, methodName, m_name);
            script.MoveForwardIf(Constants.NEXT_ARG);

            string paramName = Utils.GetToken(script, Constants.NEXT_ARG_ARRAY);
            Utils.CheckNotEmpty(script, paramName, m_name);
            script.MoveForwardIf(Constants.NEXT_ARG);

            Variable paramValueVar = Utils.GetItem(script);
            string paramValue = paramValueVar.AsString();

            var result = Statics.InvokeCall(typeof(Statics),
                methodName, paramName, paramValue);
            return result;
        }
    }
}
