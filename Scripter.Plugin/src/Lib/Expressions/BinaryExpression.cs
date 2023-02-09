namespace ScripterLang
{
    public class BinaryExpression : Expression
    {
        private readonly Expression _left;
        private readonly string _operator;
        private readonly Expression _right;

        public BinaryExpression(Expression left, string @operator, Expression right)
        {
            _left = left;
            _operator = @operator;
            _right = right;
        }

        public override Value Evaluate()
        {
            var left = _left.Evaluate();
            var right = _right.Evaluate();

            switch (_operator)
            {
                case "+":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateInteger(left.RawInt + right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.AsNumber + right.AsNumber);
                    if (left.IsString || right.IsString)
                        return Value.CreateString(left.Stringify + right.Stringify);
                    throw MakeUnsupportedOperandsException(left, right);
                case "-":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateInteger(left.RawInt - right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.AsNumber - right.AsNumber);
                    throw MakeUnsupportedOperandsException(left, right);
                case "*":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateInteger(left.RawInt * right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.AsNumber * right.AsNumber);
                    throw MakeUnsupportedOperandsException(left, right);
                case "/":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateInteger(left.RawInt / right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.AsNumber / right.AsNumber);
                    throw MakeUnsupportedOperandsException(left, right);
                case "<":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateBoolean(left.RawInt < right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.AsNumber < right.AsNumber);
                    throw MakeUnsupportedOperandsException(left, right);
                case "<=":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateBoolean(left.RawInt <= right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.AsNumber <= right.AsNumber + Value.Epsilon);
                    throw MakeUnsupportedOperandsException(left, right);
                case ">":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateBoolean(left.RawInt > right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.AsNumber > right.AsNumber);
                    throw MakeUnsupportedOperandsException(left, right);
                case ">=":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateBoolean(left.RawInt >= right.RawInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.AsNumber >= right.AsNumber - Value.Epsilon);
                    throw MakeUnsupportedOperandsException(left, right);
                case "&&":
                    if (left.IsBool && right.IsBool)
                        return Value.CreateBoolean(left.RawBool && right.RawBool);
                    throw MakeUnsupportedOperandsException(left, right);
                case "||":
                    if (left.IsBool && right.IsBool)
                        return Value.CreateBoolean(left.RawBool || right.RawBool);
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
            return new ScripterRuntimeException($"Operator {_operator} is not supported on operands of type {ValueTypes.Name(left.Type)} and {ValueTypes.Name(right.Type)}");
        }

        public override string ToString()
        {
            return $"{_left} {_operator} {_right}";
        }
    }
}
