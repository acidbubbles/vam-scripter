namespace ScripterLang
{
    public class UndefinedExpression : Expression
    {
        public static readonly Expression Instance = new UndefinedExpression();

        public override Value Evaluate()
        {
            return Value.Undefined;
        }
    }
}
