using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleExpression : CodeBlockExpression
    {
        public readonly ModuleLexicalContext Context;

        public ModuleExpression(List<Expression> expression, ModuleLexicalContext context)
            : base(expression, context)
        {
            Context = context;
        }
    }
}
