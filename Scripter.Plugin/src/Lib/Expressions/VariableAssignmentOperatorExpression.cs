namespace ScripterLang
{
    public class VariableAssignmentOperatorExpression : AssignmentOperatorExpression
    {
        private readonly string _name;

        public VariableAssignmentOperatorExpression(string name, string op, Expression expression)
            :base(op, expression)
        {
            _name = name;
        }

        protected override Value GetVariableValue(RuntimeDomain domain)
        {
            return domain.GetVariableValue(_name);
        }

        protected override Value SetVariableValue(RuntimeDomain domain, Value value)
        {
            return domain.SetVariableValue(_name, value);
        }

        protected override string LeftString()
        {
            return _name;
        }
    }
}
