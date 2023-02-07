namespace ScripterLang
{
    public class PropertyAssignmentExpression : Expression
    {
        private readonly Expression _left;
        private readonly string _property;
        private readonly Expression _expression;

        public PropertyAssignmentExpression(Expression left, string property, Expression expression)
        {
            _left = left;
            _property = property;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _expression.Evaluate(domain);
            var obj = _left.Evaluate(domain);
            obj.AsObject.Set(_property, value);
            return value;
        }

        public override string ToString()
        {
            return $"{_left}.{_property} = {_expression}";
        }
    }
}
