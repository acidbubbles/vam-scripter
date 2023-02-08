namespace ScripterLang
{
    public abstract class VariableAccessor : Expression
    {
        public abstract Value SetVariableValue(RuntimeDomain domain, Value value);
    }
}
