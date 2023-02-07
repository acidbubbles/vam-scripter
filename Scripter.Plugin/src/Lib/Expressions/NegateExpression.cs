namespace ScripterLang
{
    public class NegateExpression : Expression
    {
        private readonly Expression _expression;

        public NegateExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _expression.Evaluate(domain);
            if (value.Type != ValueTypes.BooleanType)
                throw new ScripterRuntimeException("Operator ! must be followed by a boolean value");
            return !value.AsBool;
        }

        public override string ToString()
        {
            return $"!{_expression}";
        }
    }
}
