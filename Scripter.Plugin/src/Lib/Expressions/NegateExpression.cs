namespace ScripterLang
{
    public class NegateExpression : Expression
    {
        private readonly Expression _expression;

        public NegateExpression(Expression expression)
        {
            _expression = expression;
        }

        public override void Bind()
        {
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            var value = _expression.Evaluate();
            return !value.Boolify;
        }

        public override string ToString()
        {
            return $"!{_expression}";
        }
    }
}
