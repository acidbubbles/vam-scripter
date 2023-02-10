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

        public override void Bind()
        {
            _accessor.Bind();
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            var right = _expression.Evaluate();
            _accessor.SetVariableValue(right);
            return right;
        }

        public override string ToString()
        {
            return $"{_accessor} = {_expression}";
        }
    }
}
