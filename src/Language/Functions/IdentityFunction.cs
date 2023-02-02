namespace SplitAndMerge
{
    class IdentityFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return script.Execute(Constants.END_ARG_ARRAY);
        }

    }
}
