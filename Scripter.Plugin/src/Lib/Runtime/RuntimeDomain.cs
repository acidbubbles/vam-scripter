using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public class RuntimeDomain
    {
        private int _objectIdCounter;

        public bool IsReturn;

        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();
        public readonly Dictionary<int, ReferenceCounted> Objects = new Dictionary<int, ReferenceCounted>();

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
        public Func<RuntimeDomain, Value[], Value> GetFunction(LexicalContext lexicalContext, string name)
        {
            return lexicalContext.GetFunction(name);
        }

        [MethodImpl(0x0100)]
        public void ClearVariable(string name)
        {
            #warning If count is set to zero, delete the object from Objects (ObjectType)
            Variables.Remove(name);
        }

        [MethodImpl(0x0100)]
        public Value WrapReference(Reference reference)
        {
            var id = _objectIdCounter++;
            Objects.Add(id, new ReferenceCounted
            {
                Reference = reference,
                Count = 1
            });
            return Value.CreateObject(id);
        }
    }
}
