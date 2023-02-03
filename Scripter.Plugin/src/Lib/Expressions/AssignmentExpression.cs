namespace ScripterLang
{
    public class AssignmentExpression : Expression
    {
        public AssignmentExpression(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }

        public string Name { get; }
        public Expression Expression { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var right = Expression.Evaluate(domain);
            domain.SetVariableValue(Name, right);
            return right;
        }
    }
}
