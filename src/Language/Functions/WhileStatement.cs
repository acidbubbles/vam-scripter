namespace SplitAndMerge
{
    class WhileStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessWhile(script);
        }

        public override string Description()
        {
            return "Execute a loop as long as the condition is true: while (condition) { statements; }";
        }
    }
}
