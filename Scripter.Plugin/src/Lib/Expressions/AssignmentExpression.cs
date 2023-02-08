namespace ScripterLang
{
    public class AssignmentExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression _expression;

        public AssignmentExpression(VariableAccessor accessor, Expression expression)
        {
            _accessor = accessor;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var right = _expression.Evaluate(domain);
            _accessor.SetVariableValue(domain, right);
            return right;
        }

        public override string ToString()
        {
            return $"{_accessor} = {_expression}";
        }
    }
}
