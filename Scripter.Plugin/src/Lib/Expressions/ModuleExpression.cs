using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleExpression : CodeBlockExpression
    {
        public readonly string ModuleName;
        public readonly ModuleLexicalContext Context;

        public ModuleExpression(List<Expression> expression, string moduleName, ModuleLexicalContext context)
            : base(expression, context)
        {
            ModuleName = moduleName;
            Context = context;
        }
    }
}
