namespace SplitAndMerge
{
    class ForStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessFor(script);
        }

        public override string Description()
        {
            return "A canonic for loop, e.g. for (i = 0; i < 10; ++i) or for (item : listOfValues)";
        }
    }
}
