namespace SplitAndMerge
{
    class NameExistsFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            string varName = Utils.GetToken(script, Constants.TOKEN_SEPARATION);
            varName = Constants.ConvertName(varName);

            bool result = InterpreterInstance.GetVariable(varName, script) != null;
            return new Variable(result);
        }
    }
}
