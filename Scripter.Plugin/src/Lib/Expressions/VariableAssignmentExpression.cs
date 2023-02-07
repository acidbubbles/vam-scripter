namespace ScripterLang
{
    public class VariableAssignmentExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _expression;

        public VariableAssignmentExpression(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var right = _expression.Evaluate(domain);
            domain.SetVariableValue(_name, right);
            return right;
        }

        public override string ToString()
        {
            return $"{_name} = {_expression}";
        }
    }
}
