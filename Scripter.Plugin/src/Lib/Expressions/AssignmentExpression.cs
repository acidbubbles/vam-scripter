namespace ScripterLang
{
    public class AssignmentExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _expression;

        public AssignmentExpression(string name, Expression expression)
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
    }
}
