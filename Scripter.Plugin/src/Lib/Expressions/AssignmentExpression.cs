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

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            var right = Expression.Evaluate(lexicalContext);
            lexicalContext.Declarations[Name] = right;
            return right;
        }
    }
}
