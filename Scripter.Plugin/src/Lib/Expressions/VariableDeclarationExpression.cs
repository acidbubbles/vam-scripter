namespace ScripterLang
{
    public class VariableDeclarationExpression : DeclarationExpression
    {
        private readonly Expression _expression;
        private readonly LexicalContext _context;

        public VariableDeclarationExpression(string name, Expression expression, LexicalContext context)
            : base(name)
        {
            _expression = expression;
            _context = context;
        }

        public override Value Evaluate()
        {
            var rightValue = _expression.Evaluate();
            _context.CreateVariableValue(Name, rightValue);
            return rightValue;
        }

        public override string ToString()
        {
            return $"var {Name} = {_expression}";
        }
    }
}
