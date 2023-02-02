namespace SplitAndMerge
{
    class TryBlock : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessTry(script);
        }

        public override string Description()
        {
            return "Try and catch control flow: try { statements; } catch (exceptionString) { statements; }. Curly brackets are mandatory.";
        }
    }
}
