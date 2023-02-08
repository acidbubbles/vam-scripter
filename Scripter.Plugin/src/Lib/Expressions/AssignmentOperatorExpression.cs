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

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _accessor.Evaluate(domain);
            var right = _expression.Evaluate(domain);
            switch (_op)
            {
                case "+=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(domain, value.RawInt + right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(domain, value.AsNumber + right.AsNumber);
                    if (value.IsString)
                        return _accessor.SetVariableValue(domain, value.Stringify + right.Stringify);
                    throw MakeUnsupportedOperandsException(value, right);
                case "-=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(domain, value.RawInt - right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(domain, value.AsNumber - right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "*=":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(domain, value.RawInt * right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(domain, value.AsNumber * right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "/*":
                    if (value.IsInt && right.IsInt)
                        return _accessor.SetVariableValue(domain, value.RawInt / right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return _accessor.SetVariableValue(domain, value.AsNumber / right.AsNumber);
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
