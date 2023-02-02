namespace SplitAndMerge
{
    class IsUndefinedFunction : ParserFunction
    {
        string m_argument;
        string m_action;

        internal IsUndefinedFunction(string arg = "", string action = "")
        {
            m_argument = arg;
            m_action = action;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            var variable = InterpreterInstance.GetVariable(m_argument, script);
            var varValue = variable == null ? null : variable.GetValue(script);
            bool isUndefined = varValue == null || varValue.Type == Variable.VarType.UNDEFINED;

            bool result = m_action == "===" || m_action == "==" ? isUndefined :
                !isUndefined;
            return new Variable(result);
        }
        public override string Description()
        {
            return "Returns if the current expression is defined.";
        }
    }
}
