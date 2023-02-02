namespace SplitAndMerge
{
    class DoWhileStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessDoWhile(script);
        }
        public override string Description()
        {
            return "Execute a loop at least once and as long as the condition is true: do { statements; } while (condition);";
        }
    }
}
