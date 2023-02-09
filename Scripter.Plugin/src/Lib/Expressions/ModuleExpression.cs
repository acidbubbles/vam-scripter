using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleExpression : CodeBlockExpression
    {
        public readonly string ModuleName;
        public readonly ModuleLexicalContext Context;

        private bool _evaluated;
        private Value _value = Value.Undefined;

        public ModuleExpression(List<Expression> expression, string moduleName, ModuleLexicalContext context)
            : base(expression, context)
        {
            ModuleName = moduleName;
            Context = context;
        }

        public override Value Evaluate()
        {
            try
            {
                return base.Evaluate();
            }
            finally
            {
                Context.IsReturn = false;
            }
        }

        public Dictionary<string, Value> Import()
        {
            if (_evaluated) return Context.Exports;
            _value = Evaluate();
            _evaluated = true;
            return Context.Exports;
        }
    }
}
