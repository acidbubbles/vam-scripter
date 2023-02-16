namespace ScripterLang
{
    public class ScopeLexicalContext : LexicalContext
    {
        private readonly ScopeLexicalContext _parent;

        public ScopeLexicalContext(ScopeLexicalContext parent)
        {
            _parent = parent;
        }

        public override void Declare(VariableReference variable)
        {
            var scope = this;
            while((scope = scope._parent) != null)
            {
                if (scope.IsDeclared(variable.Name))
                    throw new ScripterParsingException($"Variable {variable.Name} was already declared in an outer scope", variable.Location);
            }
            base.Declare(variable);
        }

        public override VariableReference GetVariable(string name)
        {
            var scope = this;
            while((scope = scope._parent) != null)
            {
                if (scope.IsDeclared(name))
                    return scope.GetVariable(name);
            }
            return base.GetVariable(name);
        }

        public override LoopLexicalContext GetLoopContext() => _parent.GetLoopContext();

        public override FunctionLexicalContext GetFunctionContext() => _parent.GetFunctionContext();
        public override ModuleLexicalContext GetModuleContext() => _parent.GetModuleContext();
    }
}
