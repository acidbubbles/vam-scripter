namespace ScripterLang
{
    public class AssignmentOperatorExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly string _op;
        private readonly Expression _expression;

        public AssignmentOperatorExpression(VariableAccessor accessor, string op, Expression expression)
        {
            _accessor = accessor;
            _op = op;
            _expression = expression;
        }

        public override void Bind()
        {
            _accessor.Bind();
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            var right = _expression.Evaluate();
            var value = _accessor.GetAndHold();
            Value result;
            switch (_op)
            {
                case "+=":
                    if (value.IsInt && right.IsInt)
                        result = value.RawInt + right.RawInt;
                    else if (value.IsNumber && right.IsNumber)
                        result = value.AsNumber + right.AsNumber;
                    else if (value.IsString)
                        result = value.Stringify + right.Stringify;
                    else
                        throw MakeUnsupportedOperandsException(value, right);
                    break;
                case "-=":
                    if (value.IsInt && right.IsInt)
                        result = value.RawInt - right.RawInt;
                    else if (value.IsNumber && right.IsNumber)
                        result = value.AsNumber - right.AsNumber;
                    else
                        throw MakeUnsupportedOperandsException(value, right);
                    break;
                case "*=":
                    if (value.IsInt && right.IsInt)
                        result = value.RawInt * right.RawInt;
                    else if (value.IsNumber && right.IsNumber)
                        result = value.AsNumber * right.AsNumber;
                    else
                        throw MakeUnsupportedOperandsException(value, right);
                    break;
                case "/*":
                    if (value.IsInt && right.IsInt)
                        result = value.RawInt / right.RawInt;
                    else if (value.IsNumber && right.IsNumber)
                        result = value.AsNumber / right.AsNumber;
                    else
                        throw MakeUnsupportedOperandsException(value, right);
                    break;
                default:
                    throw MakeUnsupportedOperandsException(value, right);
            }
            _accessor.SetAndRelease(result);
            return result;
        }

        private ScripterRuntimeException MakeUnsupportedOperandsException(Value value, Value right)
        {
            _accessor.Release();
            return new ScripterRuntimeException($"Operator {_op} is not supported on operands of type {ValueTypes.Name(value.Type)} and {ValueTypes.Name(right.Type)}");
        }

        public override string ToString()
        {
            return $"{_accessor} {_op} {_expression}";
        }
    }
}
