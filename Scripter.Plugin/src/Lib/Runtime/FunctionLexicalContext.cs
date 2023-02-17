namespace ScripterLang
{
    public class FunctionLexicalContext : ScopeLexicalContext
    {
        public bool isReturn;

        public FunctionLexicalContext(ScopeLexicalContext lexicalContext)
            : base(lexicalContext)
        {
        }

        public override FunctionLexicalContext GetFunctionContext() => this;
    }
}
