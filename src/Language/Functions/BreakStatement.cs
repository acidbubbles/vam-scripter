namespace SplitAndMerge
{
    class BreakStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return new Variable(Variable.VarType.BREAK);
        }
        public override string Description()
        {
            return "Breaks out of a loop.";
        }
    }

    // Get a value of a variable or of an array element
}
