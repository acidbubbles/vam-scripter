namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        private readonly ScopeLexicalContext _parent;

        public ScopeLexicalContext(ScopeLexicalContext parent)
        {
            _parent = parent;
        }

        public override void Declare(string name, Location location)
        {
            var scope = this;
            while((scope = scope._parent) != null)
            {
                if (scope.Declarations.Contains(name))
                    throw new ScripterParsingException($"Variable {name} was already declared in an outer scope", location);
            }
            base.Declare(name, location);
        }

        public override Value GetVariableValue(string name)
        {
            Value value;
            if (Variables.TryGetValue(name, out value))
                return value;
            if (_parent == null)
                throw new ScripterRuntimeException($"{name} was not defined");
            return _parent.GetVariableValue(name);
        }

        public override Value SetVariableValue(string name, Value value)
        {
            if (Declarations.Contains(name))
                return base.SetVariableValue(name, value);
            if (_parent == null)
                throw new ScripterRuntimeException($"{name} was not defined");
            return _parent.SetVariableValue(name, value);
        }

        public override ModuleLexicalContext GetModuleContext() => _parent.GetModuleContext();
        public override FunctionLexicalContext GetFunctionContext() => _parent.GetFunctionContext();
        public override GlobalLexicalContext GetGlobalContext() => _parent.GetGlobalContext();
    }
}
