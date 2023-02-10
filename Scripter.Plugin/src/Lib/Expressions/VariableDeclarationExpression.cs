namespace ScripterLang
{
    public class VariableDeclarationExpression : DeclarationExpression
    {
        private readonly Expression _expression;
        private readonly LexicalContext _context;
        private VariableReference _variable;

        public VariableDeclarationExpression(string name, Expression expression, LexicalContext context)
            : base(name)
        {
            _expression = expression;
            _context = context;
        }

        public override void Bind()
        {
            _expression.Bind();
            _variable = _context.GetVariable(Name);
            _variable.Bound = true;
        }

        public override Value Evaluate()
        {
            var rightValue = _expression.Evaluate();
            _variable.Initialize(rightValue);
            return rightValue;
        }

        public override string ToString()
        {
            return $"var {Name} = {_expression}";
        }
    }
}
