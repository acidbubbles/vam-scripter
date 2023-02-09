using System.Collections.Generic;

namespace ScripterLang
{
    public class Program
    {
        public readonly GlobalLexicalContext GlobalContext = new GlobalLexicalContext();

        public Value Add(string moduleName, string source)
        {
            var tokens = new List<Token>(Tokenizer.Tokenize(source));
            var expression = new Parser(tokens).Parse(GlobalContext, moduleName);
            GlobalContext.DeclareModule(expression.ModuleName, expression);
            return expression.Evaluate();
        }
    }
}
