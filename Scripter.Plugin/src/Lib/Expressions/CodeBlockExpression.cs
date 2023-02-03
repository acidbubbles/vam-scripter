using System.Collections.Generic;

namespace ScripterLang
{
    public class CodeBlockExpression : Expression
    {
        private readonly LexicalContext _lexicalContext;

        public CodeBlockExpression(List<Expression> expressions, LexicalContext lexicalContext)
        {
            _lexicalContext = lexicalContext;
            Expressions = expressions;
        }

        public List<Expression> Expressions { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            #warning Handle return;
            #warning Handle child lexical context
            foreach (var expression in Expressions)
            {
                var result = expression.Evaluate(domain);
                if (expression is ReturnExpression)
                    return result;
            }

            return Value.Undefined;
        }
    }
}
