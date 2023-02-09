using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class CodeBlockExpression : Expression
    {
        private readonly List<Expression> _expressions;
        private readonly LexicalContext _context;
        private readonly FunctionLexicalContext _functionContext;

        public CodeBlockExpression(List<Expression> expressions, LexicalContext context)
        {
            _expressions = expressions;
            _context = context;
            _functionContext = _context.GetFunctionContext();
        }

        public override Value Evaluate()
        {
            try
            {
                foreach (var expression in _expressions)
                {
                    var result = expression.Evaluate();
                    if (_functionContext.IsReturn)
                        return result;
                }

                return Value.Void;
            }
            finally
            {
                for (var i = 0; i < _context.Declarations.Count; i++)
                {
                    _context.ClearVariable(_context.Declarations[i]);
                }
            }
        }

        public override string ToString()
        {
            return string.Join(";\n", _expressions.Select(e => e.ToString()).ToArray()) + ";";
        }
    }
}
