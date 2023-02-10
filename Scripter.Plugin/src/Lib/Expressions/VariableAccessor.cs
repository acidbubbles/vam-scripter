namespace ScripterLang
{
    public abstract class VariableAccessor : Expression
    {
        public abstract void SetVariableValue(Value value);

        public abstract Value GetAndHold();
        public abstract void Release();
        public abstract void SetAndRelease(Value value);
    }
}
