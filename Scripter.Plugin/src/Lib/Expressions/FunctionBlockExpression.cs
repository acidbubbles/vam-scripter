using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionBlockExpression : CodeBlockExpression
    {
        public FunctionBlockExpression(List<Expression> expressions, ScopeLexicalContext lexicalContext)
            : base(expressions, lexicalContext)
        {
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            try
            {
                return base.Evaluate(domain);
            }
            finally
            {
                domain.IsReturn = false;
            }
        }
    }
}
