using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public class VariableReference
    {
        public readonly string name;
        public readonly Location location;
        public bool bound;
        public bool initialized;
        public bool constant;
        public Value value;
        public bool local;

        public VariableReference(string name, Location location)
        {
            this.name = name;
            this.location = location;
        }

        public VariableReference(string name, Value value, Location location)
        {
            this.name = name;
            this.value = value;
            initialized = true;
            constant = true;
            this.location = location;
        }

        [MethodImpl(0x0100)]
        public void Initialize(Value value)
        {
            this.value = value;
            initialized = true;
        }

        [MethodImpl(0x0100)]
        public void SetValue(Value value)
        {
            if (constant)
                throw new ScripterRuntimeException($"Cannot assign to constant variable {name}", location);
            this.value = value;
            initialized = true;
        }

        [MethodImpl(0x0100)]
        public Value GetValue()
        {
            if (!initialized)
                throw new ScripterRuntimeException($"Variable {name} was not initialized", location);
            return value;
        }

        [MethodImpl(0x0100)]
        public void EnsureBound()
        {
            if(bound) return;
            throw new ScripterParsingException($"Variable {name} was used before it was declared", location);
        }

        public override string ToString()
        {
            return $"{name} = {value.ToCodeString()}";
        }

        [MethodImpl(0x0100)]
        public void Clear()
        {
            value = Value.Undefined;
            initialized = false;
        }
    }
}
