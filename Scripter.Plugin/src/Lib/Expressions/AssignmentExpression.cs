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

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            var right = Expression.Evaluate(lexicalContext);
            lexicalContext.Variables[Name] = right;
            return right;
        }
    }
}
