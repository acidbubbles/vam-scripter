using System.Collections.Generic;

namespace ScripterLang
{
    public class CodeBlockExpression : Expression
    {
        private readonly ScopeLexicalContext _lexicalContext;
        private readonly List<Expression> _expressions;

        public CodeBlockExpression(List<Expression> expressions, ScopeLexicalContext lexicalContext)
        {
            _lexicalContext = lexicalContext;
            _expressions = expressions;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            try
            {
                foreach (var expression in _expressions)
                {
                    var result = expression.Evaluate(domain);
                    if (domain.IsReturn)
                        return result;
                }

                return Value.Undefined;
            }
            finally
            {
                for (var i = 0; i < _lexicalContext.Declarations.Count; i++)
                {
                    domain.ClearVariable(_lexicalContext.Declarations[i]);
                }
            }
        }
    }
}
