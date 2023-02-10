namespace ScripterLang
{
    public class ReturnExpression : Expression
    {
        private readonly Expression _expression;
        private readonly LexicalContext _context;

        public ReturnExpression(Expression expression, LexicalContext context)
        {
            _expression = expression;
            _context = context;
        }

        public override void Bind()
        {
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            _context.GetFunctionContext().IsReturn = true;
            return _expression.Evaluate();
        }

        public override string ToString()
        {
            return $"return {_expression}";
        }
    }
}
