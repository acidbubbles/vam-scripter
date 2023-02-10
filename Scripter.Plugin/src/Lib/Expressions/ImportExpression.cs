using System.Collections.Generic;

namespace ScripterLang
{
    public class ImportExpression : Expression
    {
        private readonly List<string> _imports;
        private readonly string _module;
        private readonly LexicalContext _context;
        private readonly GlobalLexicalContext _globalContext;

        public ImportExpression(List<string> imports, string path, LexicalContext context)
        {
            _imports = imports;
            _module = path;
            _context = context;
            _globalContext = _context.GetGlobalContext();
        }

        public override Value Evaluate()
        {
            var module = _globalContext.GetModule(_module);
            var ns = module.Import();
            foreach (var import in _imports)
            {
                Value value;
                if (!ns.Exports.TryGetValue(import, out value))
                    throw new ScripterRuntimeException($"Module '{module.ModuleName}' does not export '{import}'");
                _context.Variables[import] = value;
            }
            return Value.Void;
        }

        public override string ToString()
        {
            return $"import {{ {string.Join(", ", _imports.ToArray())} }} from \"{_module}\"";
        }
    }
}
