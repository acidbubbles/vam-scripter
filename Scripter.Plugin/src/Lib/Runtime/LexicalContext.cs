using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        private readonly Dictionary<string, VariableReference> _variables = new Dictionary<string, VariableReference>();
        private readonly List<VariableReference> _localReferences = new List<VariableReference>();

        [MethodImpl(0x0100)]
        protected bool IsDeclared(string name)
        {
            return _variables.ContainsKey(name);
        }

        public virtual void Declare(VariableReference variable)
        {
            if (_variables.ContainsKey(variable.name))
                throw new ScripterParsingException($"Variable {variable.name} was already declared", variable.location);
            _variables.Add(variable.name, variable);
            if (variable.local)
                _localReferences.Add(variable);
        }

        public virtual VariableReference GetVariable(string name)
        {
            VariableReference variable;
            if (!_variables.TryGetValue(name, out variable))
                throw new ScripterRuntimeException($"Variable '{name}' was not declared");
            return variable;
        }

        public abstract LoopLexicalContext GetLoopContext();
        public abstract FunctionLexicalContext GetFunctionContext();
        public abstract ModuleLexicalContext GetModuleContext();

        public virtual void Exit()
        {
            for (var i = 0; i < _localReferences.Count; i++)
            {
                var variable = _localReferences[i];
                variable.value = Value.Undefined;
                variable.initialized = false;
            }
        }
    }
}
