namespace ScripterLang
{
    public class AssignmentOperatorExpression : Expression
    {
        private readonly string _name;
        private readonly string _op;
        private readonly Expression _expression;

        public AssignmentOperatorExpression(string name, string op, Expression expression)
        {
            _name = name;
            _op = op;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = domain.GetVariableValue(_name);
            var right = _expression.Evaluate(domain);
            switch (_op)
            {
                case "+=":
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.FloatValue + right.FloatValue);
                    if (value.IsString)
                        return domain.SetVariableValue(_name, value.StringValue + right);
                    throw MakeUnsupportedOperandsException(value, right);
                case "-=":
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.FloatValue - right.FloatValue);
                    throw MakeUnsupportedOperandsException(value, right);
                case "*=":
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.FloatValue * right.FloatValue);
                    throw MakeUnsupportedOperandsException(value, right);
                case "/*":
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.FloatValue / right.FloatValue);
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
            return $"{_name} {_op} {_expression}";
        }
    }
}
