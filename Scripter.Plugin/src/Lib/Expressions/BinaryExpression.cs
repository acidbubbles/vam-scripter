using System;

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

        public override Value Evaluate(RuntimeDomain domain)
        {
            var left = _left.Evaluate(domain);
            var right = _right.Evaluate(domain);

            switch (_operator)
            {
                case "+":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.FloatValue + right.FloatValue);
                    if (left.IsString || right.IsString)
                        return Value.CreateString(left.ToString() + right);
                    throw MakeUnsupportedOperandsException(left, right);
                case "-":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.FloatValue - right.FloatValue);
                    throw MakeUnsupportedOperandsException(left, right);
                case "*":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.FloatValue * right.FloatValue);
                    throw MakeUnsupportedOperandsException(left, right);
                case "/":
                    if (left.IsInt && right.IsInt)
                        return Value.CreateInteger(left.AsInt / right.AsInt);
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateFloat(left.FloatValue / right.FloatValue);
                    throw MakeUnsupportedOperandsException(left, right);
                case "<":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.FloatValue < right.FloatValue);
                    throw MakeUnsupportedOperandsException(left, right);
                case "<=":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.FloatValue <= right.FloatValue + Value.Epsilon);
                    throw MakeUnsupportedOperandsException(left, right);
                case ">":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.FloatValue > right.FloatValue);
                    throw MakeUnsupportedOperandsException(left, right);
                case ">=":
                    if (left.IsNumber && right.IsNumber)
                        return Value.CreateBoolean(left.FloatValue >= right.FloatValue - Value.Epsilon);
                    throw MakeUnsupportedOperandsException(left, right);
                case "&&":
                    if (left.IsBool && right.IsBool)
                        return Value.CreateBoolean(left.AsBool && right.AsBool);
                    throw MakeUnsupportedOperandsException(left, right);
                case "||":
                    if (left.IsBool && right.IsBool)
                        return Value.CreateBoolean(left.AsBool || right.AsBool);
                    throw MakeUnsupportedOperandsException(left, right);
                case "==":
                    if (left.IsFloat || right.IsFloat)
                        return Math.Abs(left.FloatValue - right.FloatValue) <= Value.Epsilon;
                    else
                        return Value.CreateBoolean(left.Equals(right));
                case "!=":
                    if (left.IsFloat || right.IsFloat)
                        return Math.Abs(left.FloatValue - right.FloatValue) > Value.Epsilon;
                    else
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
