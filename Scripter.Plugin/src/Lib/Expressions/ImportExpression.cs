using System.Collections.Generic;

namespace ScripterLang
{
    public class ImportExpression : Expression
    {
        private readonly List<string> _imports;
        private readonly string _module;
        private readonly LexicalContext _context;

        public ImportExpression(List<string> imports, string module, LexicalContext context)
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
                Value value;
                if (!module.Context.Exports.TryGetValue(import, out value))
                    throw new ScripterRuntimeException($"Module '{module.ModuleName}' does not export '{import}'");
                _context.DeclareHoisted(import, value);
            }
            return Value.Void;
        }

        public override string ToString()
        {
            return $"import {{ {string.Join(", ", _imports)} }} from \"{_module}\"";
        }
    }
}
