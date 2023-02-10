namespace ScripterLang
{
    public class ModuleLexicalContext : FunctionLexicalContext
    {
        private readonly GlobalLexicalContext _globalContext;

        public readonly ModuleReference Module = new ModuleReference();

        public ModuleLexicalContext(GlobalLexicalContext globalContext)
            : base(globalContext)
        {
            _globalContext = globalContext;
        }

        public override GlobalLexicalContext GetGlobalContext() => _globalContext;
        public override ModuleLexicalContext GetModuleContext() => this;

        public override void Exit()
        {
            // TODO: We could clear references to variables neither exported nor used in child contexts, but it wouldn't be worth the effort
        }
    }
}
