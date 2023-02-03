namespace ScripterLang
{
    public abstract class Expression
    {
        public abstract Value Evaluate(RuntimeLexicalContext lexicalContext);
    }
}
