namespace ScripterLang
{
    public abstract class VariableAccessor : Expression
    {
        public abstract Value SetVariableValue(Value value);
    }
}
