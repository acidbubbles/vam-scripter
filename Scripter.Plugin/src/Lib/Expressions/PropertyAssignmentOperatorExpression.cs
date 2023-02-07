namespace ScripterLang
{
    public class PropertyAssignmentOperatorExpression : AssignmentOperatorExpression
    {
        private readonly Expression _left;
        private readonly string _name;

        // TODO: Ugly, refactor!
        private Reference _temp;

        public PropertyAssignmentOperatorExpression(Expression left, string name, string op, Expression expression)
            :base(op, expression)
        {
            _left = left;
            _name = name;
        }

        protected override Value GetVariableValue(RuntimeDomain domain)
        {
            _temp = _left.Evaluate(domain).ForceObject;
            return _temp.Get(_name);
        }

        protected override Value SetVariableValue(RuntimeDomain domain, Value value)
        {
            _temp.Set(_name, value);
            _temp = null;
            return value;
        }

        protected override string LeftString()
        {
            return _name;
        }
    }
}
