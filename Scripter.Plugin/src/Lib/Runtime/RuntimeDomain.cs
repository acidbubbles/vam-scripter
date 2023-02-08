using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public class RuntimeDomain
    {
        public bool IsReturn;

        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();

        public RuntimeDomain(GlobalLexicalContext globalLexicalContext)
        {
            foreach(var global in globalLexicalContext.Globals)
                CreateVariableValue(global.Key, global.Value);
        }

        public void CreateVariableValue(string name, Value value)
        {
            if (Variables.ContainsKey(name))
                throw new ScripterRuntimeException($"Variable '{name}' was already declared");
            Variables.Add(name, value);
        }

        [MethodImpl(0x0100)]
        public Value GetVariableValue(string name)
        {
            Value value;
            if (Variables.TryGetValue(name, out value))
                return value;
            throw new ScripterRuntimeException($"Variable '{name}' was not declared");
        }

        [MethodImpl(0x0100)]
        public Value SetVariableValue(string name, Value value)
        {
            if (Variables.ContainsKey(name))
                Variables[name] = value;
            else
                throw new ScripterRuntimeException($"Variable '{name}' was not declared");
            return value;
        }

        [MethodImpl(0x0100)]
        public void ClearVariable(string name)
        {
            Variables.Remove(name);
        }
    }
}
