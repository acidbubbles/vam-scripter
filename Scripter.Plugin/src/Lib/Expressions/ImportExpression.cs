using System.Collections.Generic;

namespace ScripterLang
{
    public class ImportExpression : Expression
    {
        private readonly List<string> _imports;
        private readonly string _module;
        private readonly ModuleLexicalContext _context;

        public ImportExpression(List<string> imports, string module, ModuleLexicalContext context)
        {
            _imports = imports;
            _module = module;
            _context = context;
        }

        public override Value Evaluate()
        {
            #warning Do not import twice
            var module = _context.GetGlobalContext().GetModule(_module);
            foreach (var import in _imports)
            {
                _context.DeclareHoisted(import, module.GetVariableValue(import));
            }
            return Value.Void;
        }

        public override string ToString()
        {
            return $"import {{ {string.Join(", ", _imports)} }} from \"{_module}\"";
        }
    }
}
