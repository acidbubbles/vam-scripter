namespace SplitAndMerge
{
    class SwitchStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return InterpreterInstance.ProcessSwitch(script);
        }
        public override string Description()
        {
            return "Execute a switch(value) statement.";
        }
    }
}
