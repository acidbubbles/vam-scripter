using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleExpression : CodeBlockExpression, IModule
    {
        public string ModuleName { get; }
        public readonly ModuleLexicalContext Context;

        private bool _evaluated;

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

        public ModuleReference Import()
        {
            if (_evaluated) return Context.Module;
            var value = Evaluate();
            Context.Module.Returned = value;
            _evaluated = true;
            return Context.Module;
        }

        public void Invalidate()
        {
            Context.Module.Returned = Value.Undefined;
            Context.Module.Exports.Clear();
            _evaluated = false;
        }
    }
}
