namespace SplitAndMerge
{
    class NullFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return Variable.EmptyInstance;
        }
        public override string Description()
        {
            return "Returns a null value.";
        }
    }
}
