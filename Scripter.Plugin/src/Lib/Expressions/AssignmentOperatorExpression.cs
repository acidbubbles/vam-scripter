namespace ScripterLang
{
    public abstract class AssignmentOperatorExpression : Expression
    {
        private readonly string _op;
        private readonly Expression _expression;

        protected AssignmentOperatorExpression(string op, Expression expression)
        {
            _op = op;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = GetVariableValue(domain);
            var right = _expression.Evaluate(domain);
            switch (_op)
            {
                case "+=":
                    if (value.IsInt && right.IsInt)
                        return SetVariableValue(domain, value.RawInt + right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return SetVariableValue(domain, value.AsNumber + right.AsNumber);
                    if (value.IsString)
                        return SetVariableValue(domain, value.Stringify + right.Stringify);
                    throw MakeUnsupportedOperandsException(value, right);
                case "-=":
                    if (value.IsInt && right.IsInt)
                        return SetVariableValue(domain, value.RawInt - right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return SetVariableValue(domain, value.AsNumber - right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "*=":
                    if (value.IsInt && right.IsInt)
                        return SetVariableValue(domain, value.RawInt * right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return SetVariableValue(domain, value.AsNumber * right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                case "/*":
                    if (value.IsInt && right.IsInt)
                        return SetVariableValue(domain, value.RawInt / right.RawInt);
                    if (value.IsNumber && right.IsNumber)
                        return SetVariableValue(domain, value.AsNumber / right.AsNumber);
                    throw MakeUnsupportedOperandsException(value, right);
                default:
                    throw MakeUnsupportedOperandsException(value, right);
            }
        }

        protected abstract string LeftString();
        protected abstract Value GetVariableValue(RuntimeDomain domain);
        protected abstract Value SetVariableValue(RuntimeDomain domain, Value value);

        private ScripterRuntimeException MakeUnsupportedOperandsException(Value value, Value right)
        {
            return new ScripterRuntimeException($"Operator {_op} is not supported on operands of type {ValueTypes.Name(value.Type)} and {ValueTypes.Name(right.Type)}");
        }

        public override string ToString()
        {
            return $"{LeftString()} {_op} {_expression}";
        }
    }
}
