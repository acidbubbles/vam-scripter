namespace SplitAndMerge
{
    class IfStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            Variable result = InterpreterInstance.ProcessIf(script);
            return result;
        }

        public override string Description()
        {
            return "If-else control flow statements. if (condition) { statements; } elif(condition) { statements; } else { statements; }";
        }
    }
}
