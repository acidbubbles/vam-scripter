namespace SplitAndMerge
{
    class RemoveAtFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(script, Constants.NEXT_OR_END_ARRAY);
            Utils.CheckNotEnd(script, m_name);

            // 2. Get the current value of the variable.
            ParserFunction func = InterpreterInstance.GetVariable(varName, script);
            Utils.CheckNotNull(varName, func, script);
            Variable currentValue = func.GetValue(script);
            Utils.CheckArray(currentValue, varName);

            // 3. Get the variable to remove.
            Variable item = Utils.GetItem(script);
            Utils.CheckNonNegativeInt(item, script);

            currentValue.Tuple.RemoveAt(item.AsInt());

            InterpreterInstance.AddGlobalOrLocalVariable(varName,
                new GetVarFunction(currentValue), script);
            return Variable.EmptyInstance;
        }
    }
}
