using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleExpression : CodeBlockExpression, IModule
    {
        public string ModuleName { get; }

        private readonly ModuleLexicalContext _context;
        private bool _evaluated;

        public ModuleExpression(List<Expression> expression, string moduleName, ModuleLexicalContext context)
            : base(expression, context)
        {
            ModuleName = moduleName;
            _context = context;
        }

        public override Value Evaluate()
        {
            try
            {
                return base.Evaluate();
            }
            finally
            {
                _context.IsReturn = false;
            }
        }

        public ModuleReference Import()
        {
            if (_evaluated) return _context.Module;
            var value = Evaluate();
            _context.Module.Returned = value;
            _evaluated = true;
            return _context.Module;
        }

        public void Invalidate()
        {
            _context.Module.Returned = Value.Undefined;
            _context.Module.Exports.Clear();
            _evaluated = false;
        }
    }
}
