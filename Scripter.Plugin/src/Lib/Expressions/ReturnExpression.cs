namespace ScripterLang
{
    public class ReturnExpression : Expression
    {
        public ReturnExpression(Expression expression)
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
