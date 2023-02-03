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

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            if (lexicalContext.Declarations.ContainsKey(Name))
                throw new ScripterRuntimeException($"Variable '{Name}' was already declared");
            var rightValue = Expression.Evaluate(lexicalContext);
            lexicalContext.Declarations[Name] = rightValue;
            return rightValue;
        }
    }
}
