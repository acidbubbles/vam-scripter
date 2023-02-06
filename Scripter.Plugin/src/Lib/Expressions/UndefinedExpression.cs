namespace ScripterLang
{
    public class UndefinedExpression : Expression
    {
        public static readonly UndefinedExpression Instance = new UndefinedExpression();

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Value.Undefined;
        }
    }
}
