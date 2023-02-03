namespace ScripterLang
{
    public class EmptyExpression : Expression
    {
        public static EmptyExpression Instance = new EmptyExpression();

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Value.Undefined;
        }
    }
}
