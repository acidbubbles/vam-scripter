using System.Collections.Generic;

namespace ScripterLang
{
    public class ModuleReference : ObjectReference
    {
        public Value Returned;
        public readonly Dictionary<string, Value> Exports = new Dictionary<string, Value>();
    }

    public abstract class ObjectReference
    {
        public virtual Value Get(string name)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist on the object");
        }

        public virtual void Set(string name, Value value)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist or is not writable");
        }

        public virtual Value GetIndex(Value index)
        {
            throw new ScripterRuntimeException($"Object has no indexer");
        }

        public virtual void SetIndex(Value index, Value value)
        {
            throw new ScripterRuntimeException($"Object has no indexer");
        }

        protected static Value Func(FunctionReference f) => Value.CreateFunction(f);

        protected static void ValidateArgumentsLength(string name, Value[] args, int expectedLength)
        {
            if (args.Length < expectedLength) throw new ScripterRuntimeException($"Method {name} Expected {expectedLength} arguments, received {args.Length}");
        }
    }
}
