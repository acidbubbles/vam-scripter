namespace ScripterLang
{
    public class LoopControlFlowExpression : Expression
    {
        private readonly LoopLexicalContext _context;

        private readonly bool _isBreak;

        public LoopControlFlowExpression(string keyword, LexicalContext context)
        {
            _context = context.GetLoopContext();
            switch (keyword)
            {
                case "continue":
                    _isBreak = false;
                    break;
                case "break":
                    _isBreak = true;
                    break;
                default:
                    throw new ScripterParsingException($"Unknown loop control flow keyword: {keyword}", Location.Empty);
            }
        }

        public override Value Evaluate()
        {
            if(_isBreak)
                _context.IsBreak = true;
            else
                _context.IsContinue = true;
            return Value.Void;
        }

        public override string ToString()
        {
            return _isBreak ? "break" : "continue";
        }
    }
}
