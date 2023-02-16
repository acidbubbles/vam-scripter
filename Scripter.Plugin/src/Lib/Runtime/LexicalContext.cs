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
            if (_variables.ContainsKey(variable.Name))
                throw new ScripterParsingException($"Variable {variable.Name} was already declared", variable.Location);
            _variables.Add(variable.Name, variable);
            if (variable.Local)
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
                variable.Value = Value.Undefined;
                variable.Initialized = false;
            }
        }
    }
}
