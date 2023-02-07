namespace ScripterLang
{
    public abstract class Reference
    {
        public virtual Value Get(string name)
        {
            throw new ScripterRuntimeException($"Property '{name}' does not exist on the object");
        }

        public virtual Value Method(string name, Value[] args)
        {
            throw new ScripterRuntimeException($"Method '{name}' does not exist on the object");
        }

        protected static void ValidateArgumentsLength(string name, Value[] args, int expectedLength)
        {
            if (args.Length < expectedLength) throw new ScripterRuntimeException($"Method {name} Expected {expectedLength} arguments, received {args.Length}");
        }
    }
}
