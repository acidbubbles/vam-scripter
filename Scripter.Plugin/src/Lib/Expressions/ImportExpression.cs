using System.Collections.Generic;

namespace ScripterLang
{
    public class ImportExpression : Expression
    {
        private readonly List<string> _imports;
        private readonly string _module;
        private readonly LexicalContext _context;
        private readonly GlobalLexicalContext _globalContext;

        private List<VariableReference> _importVariables;

        public ImportExpression(List<string> imports, string path, LexicalContext context)
        {
            _imports = imports;
            _module = path;
            _context = context;
            _globalContext = _context.GetGlobalContext();
        }

        public override void Bind()
        {
            _importVariables = new List<VariableReference>(_imports.Count);
            foreach (var import in _imports)
            {
                var variable = _context.GetVariable(import);
                variable.Bound = true;
                _importVariables.Add(variable);
            }
        }

        public override Value Evaluate()
        {
            var module = _globalContext.GetModule(_module);
            var ns = module.Import();
            for (var i = 0; i < _importVariables.Count; i++)
            {
                var name = _imports[i];
                var variable = _importVariables[i];
                Value value;
                if (!ns.Exports.TryGetValue(name, out value))
                    throw new ScripterRuntimeException($"Module '{module.ModuleName}' does not export '{variable}'");
                variable.Initialize(value);
            }

            return Value.Void;
        }

        public override string ToString()
        {
            return $"import {{ {string.Join(", ", _imports.ToArray())} }} from \"{_module}\"";
        }
    }
}
