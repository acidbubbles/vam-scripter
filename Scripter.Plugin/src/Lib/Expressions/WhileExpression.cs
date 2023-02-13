namespace ScripterLang
{
    public class WhileExpression : Expression
    {
        private readonly Expression _condition;
        private readonly CodeBlockExpression _body;
        private readonly LoopLexicalContext _context;

        public WhileExpression(Expression condition, CodeBlockExpression body, LoopLexicalContext context)
        {
            _condition = condition;
            _body = body;
            _context = context;
        }

        public override void Bind()
        {
            _condition.Bind();
            _body.Bind();
        }

        public override Value Evaluate()
        {
            try
            {
                while (_condition.Evaluate().Boolify)
                {
                    _body.Evaluate();
                    if (_context.IsContinue)
                    {
                        _context.IsContinue = false;
                        continue;
                    }
                    if (_context.IsBreak)
                    {
                        _context.IsBreak = false;
                        break;
                    }
                }
            }
            finally
            {
                _context.IsBreak = false;
                _context.IsContinue = false;
            }

            return Value.Void;
        }
    }
}
