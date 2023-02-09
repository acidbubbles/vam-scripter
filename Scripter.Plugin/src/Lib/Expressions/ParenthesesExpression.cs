namespace ScripterLang
{
    public class ParenthesesExpression : Expression
    {
        private readonly Expression _expression;

        public ParenthesesExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Value Evaluate()
        {
            return _expression.Evaluate();
        }

        public override string ToString()
        {
            return $"({_expression})";
        }
    }
}
