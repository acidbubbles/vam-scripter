namespace SplitAndMerge
{
    class VarFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            var args = Utils.GetTokens(script);
            Variable result = Variable.EmptyInstance;
            foreach (var arg in args)
            {
                var ind = arg.IndexOf('=');
                if (ind <= 0)
                {
                    if (!InterpreterInstance.FunctionExists(arg))
                    {
                        InterpreterInstance.AddGlobalOrLocalVariable(arg, new GetVarFunction(new Variable(Variable.VarType.NONE)), script);
                    }
                    continue;
                }
                var varName = arg.Substring(0, ind);
                ParsingScript tempScript = NewParsingScript(arg.Substring(ind + 1));
                AssignFunction assign = new AssignFunction(InterpreterInstance);
                result = assign.Assign(tempScript, varName, true);
            }
            return result;
        }


    }
}
