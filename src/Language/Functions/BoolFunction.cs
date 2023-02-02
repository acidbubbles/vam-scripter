namespace SplitAndMerge
{
    class BoolFunction : ParserFunction, INumericFunction
    {
        bool m_value;
        public BoolFunction(bool init)
        {
            m_value = init;
        }
        protected override Variable Evaluate(ParsingScript script)
        {
            return new Variable(m_value);
        }
    }
}
