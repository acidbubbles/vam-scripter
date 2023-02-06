namespace ScripterLang
{
    public class StaticVariableDeclarationExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _expression;

        public StaticVariableDeclarationExpression(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            Value value;
            if (domain.StaticVariables.TryGetValue(_name, out value))
                return value;
            value = _expression.Evaluate(domain);
            domain.StaticVariables.Add(_name, value);
            return value;
        }
    }
}
