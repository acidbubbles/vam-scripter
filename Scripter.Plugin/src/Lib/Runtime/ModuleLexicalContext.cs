namespace ScripterLang
{
    public class ModuleLexicalContext : FunctionLexicalContext
    {
        public readonly GlobalLexicalContext GlobalContext;

        public readonly ModuleReference Module = new ModuleReference();

        public ModuleLexicalContext(GlobalLexicalContext globalContext)
            : base(globalContext)
        {
            GlobalContext = globalContext;
        }

        public override GlobalLexicalContext GetGlobalContext() => GlobalContext;
        public override ModuleLexicalContext GetModuleContext() => this;
    }
}
