namespace SplitAndMerge
{
    class ContinueStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return new Variable(Variable.VarType.CONTINUE);
        }
        public override string Description()
        {
            return "Forces the next iteration of the loop.";
        }
    }
}
