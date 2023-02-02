namespace SplitAndMerge
{
    class UndefinedFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            return new Variable(Variable.VarType.UNDEFINED);
        }
    }
}
