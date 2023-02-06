using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class RuntimeDomain
    {
        public bool IsReturn;

        public readonly Dictionary<string, Value> StaticVariables = new Dictionary<string, Value>();
        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();

        public Value GetVariableValue(string name)
        {
            Value value;
            if (Variables.TryGetValue(name, out value))
                return value;
            if (StaticVariables.TryGetValue(name, out value))
                return value;
            throw new ScripterRuntimeException($"Variable '{name}' was not declared");
        }

        public Value SetVariableValue(string name, Value value)
        {
            if (Variables.ContainsKey(name))
                Variables[name] = value;
            else if (StaticVariables.ContainsKey(name))
                StaticVariables[name] = value;
            else
                throw new ScripterRuntimeException($"Variable '{name}' was not declared");
            return value;
        }

        public Func<RuntimeDomain, Value[], Value> GetFunction(LexicalContext lexicalContext, string name)
        {
            return lexicalContext.GetFunction(name);
        }

        public void ClearVariable(string name)
        {
            Variables.Remove(name);
        }
    }
}
