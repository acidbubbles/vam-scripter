namespace SplitAndMerge
{
    class CaseStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessCase(script, Name);
        }
        public override string Description()
        {
            return "A case inside of a switch statement.";
        }
    }
}
