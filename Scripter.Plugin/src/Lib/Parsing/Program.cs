namespace ScripterLang
{
    public class Program
    {
        public readonly GlobalLexicalContext GlobalContext = new GlobalLexicalContext();

        public Value Add(string module, string source)
        {
            var expression = Parser.Parse(source, GlobalContext);
            GlobalContext.DeclareModule(module, expression.Context);
            return expression.Evaluate();
        }
    }
}
