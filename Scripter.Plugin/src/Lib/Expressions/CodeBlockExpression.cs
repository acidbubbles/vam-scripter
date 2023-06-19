using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class CodeBlockExpression : Expression
    {
        private readonly List<Expression> _expressions;
        private readonly LexicalContext _context;
        private readonly FunctionLexicalContext _functionContext;
        private readonly LoopLexicalContext _loopContext;

        public CodeBlockExpression(List<Expression> expressions, LexicalContext context)
        {
            _expressions = expressions;
            _context = context;
            _functionContext = _context.GetFunctionContext();
            _loopContext = _context.GetLoopContext();
        }

        public override void Bind()
        {
            for (var i = 0; i < _expressions.Count; i++)
                _expressions[i].Bind();
        }

        public override Value Evaluate()
        {
            try
            {
                var result = Value.Undefined;
                for (var i = 0; i < _expressions.Count; i++)
                {
                    var expression = _expressions[i];
                    result = expression.Evaluate();
                    if (_functionContext.isReturn)
                        return result;
                    if (_loopContext != null)
                    {
                        if (_loopContext.isBreak || _loopContext.isContinue)
                            break;
                    }
                }

                return result;
            }
            finally
            {
                _context.Exit();
            }
        }

        public override string ToString()
        {
            return string.Join(";\n", _expressions.Select(e => e.ToString()).ToArray()) + ";";
        }
    }
}
