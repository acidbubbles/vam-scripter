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
                _context.isReturn = false;
            }
        }

        public ModuleNamespace Import()
        {
            if (_evaluated) return _context.module;
            var value = Evaluate();
            _context.module.returned = value;
            _evaluated = true;
            return _context.module;
        }

        public void Invalidate()
        {
            _context.module.returned = Value.Undefined;
            _context.module.exports.Clear();
            _evaluated = false;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
