using System.Collections.Generic;

namespace ScripterLang
{
    public class CodeBlockExpression : Expression
    {
        public CodeBlockExpression(List<Expression> expressions)
        {
            Expressions = expressions;
        }

        public List<Expression> Expressions { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            #warning Handle return;
            #warning Handle child lexical context
            foreach (var expression in Expressions)
            {
                var result = expression.Evaluate(lexicalContext);
                if (expression is ReturnExpression)
                    return result;
            }

            return Value.Undefined;
        }
    }
}
