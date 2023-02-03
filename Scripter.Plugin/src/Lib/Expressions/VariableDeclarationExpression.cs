namespace ScripterLang
{
    public class VariableDeclarationExpression : Expression
    {
        public VariableDeclarationExpression(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }

        public string Name { get; }
        public Expression Expression { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var rightValue = Expression.Evaluate(domain);
            domain.Variables[Name] = rightValue;
            return rightValue;
        }
    }
}
