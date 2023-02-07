namespace ScripterLang
{
    public abstract class Reference
    {
        public virtual Value Get(string name)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist on the object");
        }

        public virtual void Set(string name, Value value)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist on the object");
        }

        public virtual Value InvokeMethod(string name, Value[] args)
        {
            throw new ScripterRuntimeException($"Method '{name}' does not exist on the object");
        }

        public static void ValidateArgumentsLength(string name, Value[] args, int expectedLength)
        {
            if (args.Length < expectedLength) throw new ScripterRuntimeException($"Method {name} Expected {expectedLength} arguments, received {args.Length}");
        }
    }
}
