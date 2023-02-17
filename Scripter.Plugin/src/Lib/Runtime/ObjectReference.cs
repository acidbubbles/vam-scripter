using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public abstract class ObjectReference
    {
        public virtual Value GetProperty(string name)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist on the object");
        }

        [MethodImpl(0x0100)]
        public Value GetPropertyWithDefault(string name, Value defaultValue)
        {
            var value = GetProperty(name);
            if (value.IsUndefined)
                return defaultValue;
            return value;
        }

        public virtual void SetProperty(string name, Value value)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist or is not writable");
        }

        public virtual Value GetIndex(Value index)
        {
            throw new ScripterRuntimeException("Object has no indexer");
        }

        public virtual void SetIndex(Value index, Value value)
        {
            throw new ScripterRuntimeException("Object has no indexer");
        }

        [MethodImpl(0x0100)]
        protected static Value Func(FunctionReference f) => Value.CreateFunction(f);

        [MethodImpl(0x0100)]
        protected static void ValidateArgumentsLength(string name, Value[] args, int expectedLength)
        {
            if (args.Length < expectedLength) throw new ScripterRuntimeException($"Method {name} Expected {expectedLength} arguments, received {args.Length}");
        }
    }
}
