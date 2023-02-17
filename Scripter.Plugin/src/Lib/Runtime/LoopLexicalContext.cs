namespace ScripterLang
{
    public class LoopLexicalContext : ScopeLexicalContext
    {
        public bool isBreak;
        public bool isContinue;

        public LoopLexicalContext(ScopeLexicalContext lexicalContext)
            : base(lexicalContext)
        {
        }

        public override LoopLexicalContext GetLoopContext() => this;
    }
}
