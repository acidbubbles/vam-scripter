namespace ScripterLang
{
    public class ParenthesesExpression : Expression
    {
        private readonly Expression _expression;

        public ParenthesesExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return _expression.Evaluate(domain);
        }

        public override string ToString()
        {
            return $"({_expression})";
        }
    }
}
