namespace SplitAndMerge
{
    class FunctionCreator : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            string funcName = Utils.GetToken(script, Constants.TOKEN_SEPARATION);
            funcName = Constants.ConvertName(funcName);

            string[] args = Utils.GetFunctionSignature(script);
            if (args.Length == 1 && string.IsNullOrWhiteSpace(args[0]))
            {
                args = new string[0];
            }

            script.MoveForwardIf(Constants.START_GROUP, Constants.SPACE);
            int _;
            /*string line = */script.GetOriginalLine(out _);

            int parentOffset = script.Pointer;

            string body = Utils.GetBodyBetween(script, Constants.START_GROUP, Constants.END_GROUP);
            script.MoveForwardIf(Constants.END_GROUP);

            CustomFunction customFunc = new CustomFunction(funcName, body, args, script);
            customFunc.ParentScript = script;
            customFunc.ParentOffset = parentOffset;

            InterpreterInstance.RegisterFunction(funcName, customFunc, false /* not native */);

            return Variable.EmptyInstance;
        }

        public override string Description()
        {
            return "Creates a new CSCS function.";
        }
    }
}
