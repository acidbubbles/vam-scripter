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
                    if (value.IsInt && right.IsInt)
                        return domain.SetVariableValue(_name, value.IntValue + right.IntValue);
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.AsFloat + right.AsFloat);
                    if (value.IsString)
                        return domain.SetVariableValue(_name, value.StringValue + right);
                    throw MakeUnsupportedOperandsException(value, right);
                case "-=":
                    if (value.IsInt && right.IsInt)
                        return domain.SetVariableValue(_name, value.IntValue - right.IntValue);
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.AsFloat - right.AsFloat);
                    throw MakeUnsupportedOperandsException(value, right);
                case "*=":
                    if (value.IsInt && right.IsInt)
                        return domain.SetVariableValue(_name, value.IntValue * right.IntValue);
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.AsFloat * right.AsFloat);
                    throw MakeUnsupportedOperandsException(value, right);
                case "/*":
                    if (value.IsInt && right.IsInt)
                        return domain.SetVariableValue(_name, value.IntValue / right.IntValue);
                    if (value.IsNumber && right.IsNumber)
                        return domain.SetVariableValue(_name, value.AsFloat / right.AsFloat);
                    throw MakeUnsupportedOperandsException(value, right);
                default:
                    throw MakeUnsupportedOperandsException(value, right);
            }
        }

        private ScripterRuntimeException MakeUnsupportedOperandsException(Value value, Value right)
        {
            return new ScripterRuntimeException($"Operator {_op} is not supported on operands of type {ValueTypes.Name(value.Type)} and {ValueTypes.Name(right.Type)}");
        }
    }
}
