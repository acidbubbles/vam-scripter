namespace ScripterLang
{
    public class LoopLexicalContext : ScopeLexicalContext
    {
        public bool IsBreak;
        public bool IsContinue;

        public LoopLexicalContext(ScopeLexicalContext lexicalContext)
            : base(lexicalContext)
        {
        }

        public override LoopLexicalContext GetLoopContext() => this;
    }
}
