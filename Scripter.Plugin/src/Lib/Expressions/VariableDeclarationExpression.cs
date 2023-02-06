namespace ScripterLang
{
    public class VariableDeclarationExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _expression;

        public VariableDeclarationExpression(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var rightValue = _expression.Evaluate(domain);
            domain.Variables.Add(_name, rightValue);
            return rightValue;
        }
    }
}
