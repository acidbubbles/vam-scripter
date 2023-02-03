namespace ScripterLang
{
    public class ReturnExpression : Expression
    {
        private readonly Expression _expression;

        public ReturnExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return _expression.Evaluate(domain);
        }
    }
}
