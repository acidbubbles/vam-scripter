namespace SplitAndMerge
{
    class ConstantsFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return new Variable(m_name);
        }
    }
}
