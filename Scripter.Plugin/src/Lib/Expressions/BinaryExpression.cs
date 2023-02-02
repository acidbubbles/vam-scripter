namespace ScripterLang
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, string @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public Expression Left { get; }
        public string Operator { get; }
        public Expression Right { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            var left = Left.Evaluate(lexicalContext);
            var right = Right.Evaluate(lexicalContext);

            switch (Operator)
            {
                case "+":
                    if (left.Type == ValueTypes.NumberType && right.Type == ValueTypes.NumberType)
                    {
                        return Value.CreateNumber(left.Number + right.Number);
                    }
                    if (left.Type == ValueTypes.StringType && right.Type == ValueTypes.StringType)
                    {
                        return Value.CreateString(left.String + right.String);
                    }

                    break;
                case "-":
                    if (left.Type == ValueTypes.NumberType && right.Type == ValueTypes.NumberType)
                    {
                        return Value.CreateNumber(left.Number - right.Number);
                    }

                    break;
                case "*":
                    if (left.Type == ValueTypes.NumberType && right.Type == ValueTypes.NumberType)
                    {
                        return Value.CreateNumber(left.Number * right.Number);
                    }

                    break;
                case "/":
                    if (left.Type == ValueTypes.NumberType && right.Type == ValueTypes.NumberType)
                    {
                        return Value.CreateNumber(left.Number / right.Number);
                    }

                    break;
            }

            throw new ScripterRuntimeException($"Operator '{Operator}' is not supported for operands of type '{ValueTypes.Name(left.Type)}' and '{ValueTypes.Name(right.Type)}'.");
        }
    }
}
