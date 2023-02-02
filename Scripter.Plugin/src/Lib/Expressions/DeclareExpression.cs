namespace ScripterLang
{
    public class DeclareExpression : Expression
    {
        public DeclareExpression(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }

        public string Name { get; }
        public Expression Expression { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            if (lexicalContext.Variables.ContainsKey(Name))
                throw new ScripterRuntimeException($"Variable '{Name}' was already declared");
            var right = Expression.Evaluate(lexicalContext);
            lexicalContext.Variables.Add(Name, right);
            return right;
        }
    }
}
