using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public abstract class LexicalContext
    {
        #warning Try to make private
        public readonly List<string> Declarations = new List<string>();
        public readonly List<string> HoistedDeclarations = new List<string>();
        public readonly Dictionary<string, Value> Variables = new Dictionary<string, Value>();

        public void DeclareHoisted(string name, Value value, Location location = default(Location))
        {
            if (HoistedDeclarations.Contains(name))
                throw new ScripterParsingException($"Constant {name} was already declared", location);
            HoistedDeclarations.Add(name);
            Variables.Add(name, value);
        }

        public virtual void Declare(string name, Location location)
        {
            if (Declarations.Contains(name))
                throw new ScripterParsingException($"Variable {name} was already declared", location);
            Declarations.Add(name);
        }

        public void CreateVariableValue(string name, Value value)
        {
            Variables[name] = value;
        }

        public virtual Value GetVariableValue(string name)
        {
            Value value;
            if (Variables.TryGetValue(name, out value))
                return value;
            throw new ScripterRuntimeException($"Variable '{name}' was not declared");
        }

        public virtual Value SetVariableValue(string name, Value value)
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

        public abstract GlobalLexicalContext GetGlobalContext();
        public abstract ModuleLexicalContext GetModuleContext();
        public abstract FunctionLexicalContext GetFunctionContext();
    }
}
