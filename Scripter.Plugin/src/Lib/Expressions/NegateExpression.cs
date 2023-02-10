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
            if (value.Type != ValueTypes.BooleanType)
                throw new ScripterRuntimeException("Operator ! must be followed by a boolean value");
            return !value.RawBool;
        }

        public override string ToString()
        {
            return $"!{_expression}";
        }
    }
}
