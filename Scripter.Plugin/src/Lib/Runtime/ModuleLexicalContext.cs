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

        public override void Exit()
        {
            // TODO: We could clear references to variables neither exported nor used in child contexts, but it wouldn't be worth the effort
        }
    }
}
