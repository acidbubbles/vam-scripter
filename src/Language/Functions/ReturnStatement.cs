namespace SplitAndMerge
{
    class ReturnStatement : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            script.MoveForwardIf(Constants.SPACE);
            if (!script.FromPrev(Constants.RETURN.Length).Contains(Constants.RETURN))
            {
                script.Backward();
            }
            Variable result = Utils.GetItem(script);

            // If we are in Return, we are done:
            script.SetDone();
            result.IsReturn = true;

            return result;
        }

        public override string Description()
        {
            return "Finishes execution of a function and optionally can return a value.";
        }
    }
}
