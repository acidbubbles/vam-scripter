namespace ScripterLang
{
    public class UnaryOperatorExpression : Expression
    {
        private readonly string _op;
        private readonly Expression _expression;

        public UnaryOperatorExpression(string op, Expression expression)
        {
            _op = op;
            _expression = expression;
        }

        public override void Bind()
        {
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            var value = _expression.Evaluate();
            if (!value.IsNumber)
                throw new ScripterRuntimeException($"Unexpected type for unary operator: {ValueTypes.Name(value.Type)}");
            switch (_op)
            {
                case "-":
                    if (value.IsInt)
                        return -value.AsInt;
                    return -value.AsNumber;
                case "+":
                    return value;
                default:
                    throw new ScripterRuntimeException($"Unknown unary operator: {_op}");
            }
        }

        public override string ToString()
        {
            return $"({_op} {_expression})";
        }
    }
}
