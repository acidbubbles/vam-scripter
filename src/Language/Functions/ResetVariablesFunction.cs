namespace SplitAndMerge
{
    class ResetVariablesFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            InterpreterInstance.CleanUpVariables();
            return Variable.EmptyInstance;
        }
    }
}
