namespace ScripterLang
{
    public class StaticVariableDeclarationExpression : Expression
    {
        private readonly string _name;
        private readonly Expression _expression;
        private readonly LexicalContext _context;

        public StaticVariableDeclarationExpression(string name, Expression expression, LexicalContext context)
        {
            _name = name;
            _expression = expression;
            _context = context.GetModuleContext();
        }

        public override Value Evaluate()
        {
            Value value;
            if (_context.Variables.TryGetValue(_name, out value))
                return value;
            value = _expression.Evaluate();
            _context.CreateVariableValue(_name, value);
            return value;
        }
    }
}
