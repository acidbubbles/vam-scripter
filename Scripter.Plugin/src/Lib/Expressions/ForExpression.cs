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
                    if (_context.isContinue)
                    {
                        _context.isContinue = false;
                        continue;
                    }
                    if (_context.isBreak)
                    {
                        _context.isBreak = false;
                        break;
                    }
                }
            }
            finally
            {
                _context.isBreak = false;
                _context.isContinue = false;
            }

            return Value.Void;
        }
    }
}
