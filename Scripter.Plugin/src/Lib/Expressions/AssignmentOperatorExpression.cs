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

        public override Value Evaluate()
        {
            var value = _accessor.Evaluate();
            var right = _expression.Evaluate();
            switch (_op)
            {
                case "+=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(value.RawInt + right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(value.AsNumber + right.AsNumber);
                    if (value.IsString)
                        return _accessor.SetVariableValue(value.Stringify + right.Stringify);
                    throw MakeUnsupportedOperandsException(value, right);
                case "-=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(value.RawInt - right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(value.AsNumber - right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "*=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(value.RawInt * right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(value.AsNumber * right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "/*":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(value.RawInt / right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(value.AsNumber / right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                default:
                    throw MakeUnsupportedOperandsException(value, right);
            }
        }

        private ScripterRuntimeException MakeUnsupportedOperandsException(Value value, Value right)
        {
            return new ScripterRuntimeException($"Operator {_op} is not supported on operands of type {ValueTypes.Name(value.Type)} and {ValueTypes.Name(right.Type)}");
        }

        public override string ToString()
        {
            return $"{_accessor} {_op} {_expression}";
        }
    }
}
