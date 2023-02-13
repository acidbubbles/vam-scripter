namespace ScripterLang
{
    public class ForExpression : Expression
    {
        private readonly Expression _start;
        private readonly Expression _end;
        private readonly Expression _increment;
        private readonly CodeBlockExpression _body;
        private readonly LoopLexicalContext _context;

        public ForExpression(Expression start, Expression end, Expression increment, CodeBlockExpression body, LoopLexicalContext context)
        {
            _start = start;
            _end = end;
            _increment = increment;
            _body = body;
            _context = context;
        }

        public override void Bind()
        {
            _start.Bind();
            _end.Bind();
            _increment.Bind();
            _body.Bind();
        }

        public override Value Evaluate()
        {
            try
            {
                for (_start.Evaluate(); _end.Evaluate().Boolify; _increment.Evaluate())
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
