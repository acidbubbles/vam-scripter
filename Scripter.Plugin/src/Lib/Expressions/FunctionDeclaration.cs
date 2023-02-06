using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionDeclaration
    {
        public FunctionDeclaration(string name, List<string> arguments, FunctionBlockExpression body)
        {
        }

        public Value Invoke(Value[] arg)
        {
            return Value.Undefined;
        }
    }
}
