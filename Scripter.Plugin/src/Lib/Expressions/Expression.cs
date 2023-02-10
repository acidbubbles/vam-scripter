namespace ScripterLang
{
    public abstract class Expression
    {
        public virtual void Bind() {}
        public abstract Value Evaluate();
    }
}
