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

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            var left = Left.Evaluate(lexicalContext);
            var right = Right.Evaluate(lexicalContext);

            switch (Operator)
            {
                case "+":
                    if (left.Type == ValueTypes.FloatType && right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.FloatValue + right.FloatValue);
                    }
                    if (left.Type == ValueTypes.FloatType || right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.AsFloat + right.AsFloat);
                    }
                    if (left.Type == ValueTypes.IntegerType && right.Type == ValueTypes.IntegerType)
                    {
                        return Value.CreateInteger(left.IntValue + right.IntValue);
                    }
                    if (left.Type == ValueTypes.StringType && right.Type == ValueTypes.StringType)
                    {
                        return Value.CreateString(left.StringValue + right.StringValue);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "-":
                    if (left.Type == ValueTypes.FloatType && right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.FloatValue - right.FloatValue);
                    }
                    if (left.Type == ValueTypes.FloatType || right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.AsFloat - right.AsFloat);
                    }
                    if (left.Type == ValueTypes.IntegerType && right.Type == ValueTypes.IntegerType)
                    {
                        return Value.CreateInteger(left.IntValue - right.IntValue);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "*":
                    if (left.Type == ValueTypes.FloatType && right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.FloatValue * right.FloatValue);
                    }
                    if (left.Type == ValueTypes.FloatType || right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.AsFloat * right.AsFloat);
                    }
                    if (left.Type == ValueTypes.IntegerType && right.Type == ValueTypes.IntegerType)
                    {
                        return Value.CreateInteger(left.IntValue * right.IntValue);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "/":
                    if (left.Type == ValueTypes.FloatType && right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.FloatValue / right.FloatValue);
                    }
                    if (left.Type == ValueTypes.FloatType || right.Type == ValueTypes.FloatType)
                    {
                        return Value.CreateFloat(left.AsFloat / right.AsFloat);
                    }
                    if (left.Type == ValueTypes.IntegerType && right.Type == ValueTypes.IntegerType)
                    {
                        return Value.CreateInteger(left.IntValue / right.IntValue);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "&&":
                    if (left.Type == ValueTypes.BooleanType && right.Type == ValueTypes.BooleanType)
                    {
                        return Value.CreateBoolean(left.AsBool && right.AsBool);
                    }
                    throw MakeUnsupportedOperandsException(left, right);
                case "||":
                    if (left.Type == ValueTypes.BooleanType && right.Type == ValueTypes.BooleanType)
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
