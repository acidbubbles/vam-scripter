namespace ScripterLang
{
    #warning Review whether we need a direct link to ModuleContext, and whether we need a link to global context
    #warning With a FunctionLexicalContext, IsReturn would be easier to reason about
    public class FunctionLexicalContext : ScopeLexicalContext
    {
        private readonly ScopeLexicalContext _lexicalContext;
        public bool IsReturn;

        public FunctionLexicalContext(ScopeLexicalContext lexicalContext)
            : base(lexicalContext)
        {
            _lexicalContext = lexicalContext;
        }

        public override GlobalLexicalContext GetGlobalContext() => _lexicalContext.GetGlobalContext();
        public override ModuleLexicalContext GetModuleContext() => _lexicalContext.GetModuleContext();
        public override FunctionLexicalContext GetFunctionContext() => this;
    }
}
