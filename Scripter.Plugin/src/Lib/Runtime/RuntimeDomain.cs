using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class RuntimeDomain
    {
        public bool IsReturn;

        public readonly Dictionary<string, Value> StaticVariables = new Dictionary<string, Value>();

        private readonly Dictionary<string, Value> _variables = new Dictionary<string, Value>();

        public void CreateVariable(string name, Value value)
        {
            _variables.Add(name, value);
        }

        public Value GetVariableValue(string name)
        {
            Value value;
            if (_variables.TryGetValue(name, out value))
                return value;
            if (StaticVariables.TryGetValue(name, out value))
                return value;
            throw new ScripterRuntimeException($"Variable '{name}' was not declared");
        }

        public Value SetVariableValue(string name, Value value)
        {
            if (_variables.ContainsKey(name))
                _variables[name] = value;
            else if (StaticVariables.ContainsKey(name))
                StaticVariables[name] = value;
            else
                throw new ScripterRuntimeException($"Variable '{name}' was not declared");
            return value;
        }

        public Func<Value[], Value> GetFunction(LexicalContext lexicalContext, string name)
        {
            return lexicalContext.GetFunction(name);
        }

        public void ClearVariable(string name)
        {
            _variables.Remove(name);
        }
    }
}
