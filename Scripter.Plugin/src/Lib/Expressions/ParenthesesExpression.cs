namespace ScripterLang
{
    public class ParenthesesExpression : Expression
    {
        public ParenthesesExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Expression.Evaluate(domain);
        }
    }
}
