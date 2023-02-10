using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public class VariableReference
    {
        public readonly string Name;
        public readonly Location Location;
        public bool Bound;
        public bool Initialized;
        public bool Constant;
        public Value Value;
        public bool Local;

        public VariableReference(string name, Location location)
        {
            Name = name;
            Location = location;
        }

        public VariableReference(string name, Value value, Location location)
        {
            Name = name;
            Value = value;
            Initialized = true;
            Constant = true;
            Location = location;
        }

        [MethodImpl(0x0100)]
        public void Initialize(Value value)
        {
            Value = value;
            Initialized = true;
        }

        [MethodImpl(0x0100)]
        public void SetValue(Value value)
        {
            if (Constant)
                throw new ScripterRuntimeException($"Cannot assign to constant variable {Name}", Location);
            Value = value;
            Initialized = true;
        }

        [MethodImpl(0x0100)]
        public Value GetValue()
        {
            if (!Initialized)
                throw new ScripterRuntimeException($"Variable {Name} was not initialized", Location);
            return Value;
        }

        public void EnsureBound()
        {
            if(Bound) return;
            throw new ScripterParsingException($"Variable {Name} was used before it was declared", Location);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
