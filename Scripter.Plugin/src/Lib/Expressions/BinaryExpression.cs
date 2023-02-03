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

        public override Value Evaluate(RuntimeDomain domain)
        {
            var left = Left.Evaluate(domain);
            var right = Right.Evaluate(domain);

            switch (Operator)
            {
                case "+":
                    if (left.IsInt && right.IsInt)
                    {
                        return Value.CreateInteger(left.IntValue + right.IntValue);
                    }
                    if (left.IsNumber && right.IsNumber)
                    {
                        return Value.CreateFloat(left.AsFloat + right.AsFloat);
                    }
                    if (left.IsString || right.IsString)
                    {
                        return Value.CreateString(left.ToString() + right);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "-":
                    if (left.IsInt && right.IsInt)
                    {
                        return Value.CreateInteger(left.IntValue - right.IntValue);
                    }
                    if (left.IsNumber && right.IsNumber)
                    {
                        return Value.CreateFloat(left.AsFloat - right.AsFloat);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "*":
                    if (left.IsInt && right.IsInt)
                    {
                        return Value.CreateInteger(left.IntValue * right.IntValue);
                    }
                    if (left.IsNumber && right.IsNumber)
                    {
                        return Value.CreateFloat(left.AsFloat * right.AsFloat);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "/":
                    if (left.IsInt && right.IsInt)
                    {
                        return Value.CreateInteger(left.IntValue / right.IntValue);
                    }
                    if (left.IsNumber && right.IsNumber)
                    {
                        return Value.CreateFloat(left.AsFloat / right.AsFloat);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "&&":
                    if (left.IsBool && right.IsBool)
                    {
                        return Value.CreateBoolean(left.AsBool && right.AsBool);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "||":
                    if (left.IsBool && right.IsBool)
                    {
                        return Value.CreateBoolean(left.AsBool || right.AsBool);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "==":
                    return Value.CreateBoolean(left.Equals(right));
                case "!=":
                    return Value.CreateBoolean(!left.Equals(right));
            }

            throw MakeUnsupportedOperandsException(left, right);
        }

        private ScripterRuntimeException MakeUnsupportedOperandsException(Value left, Value right)
        {
            return new ScripterRuntimeException($"Operator {Operator} is not supported on operands of type {ValueTypes.Name(left.Type)} and {ValueTypes.Name(right.Type)}");
        }
    }
}
