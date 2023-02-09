using System.Collections.Generic;

namespace ScripterLang
{
    public class GlobalLexicalContext : ScopeLexicalContext
    {
        public GlobalLexicalContext() : base(null)
        {
        }

        public override GlobalLexicalContext GetGlobalContext() => this;

        public override Value SetVariableValue(string name, Value value)
        {
            throw new ScripterRuntimeException("Cannot set variable value in the global context");
        }

        public override ModuleLexicalContext GetModuleContext()
        {
            throw new ScripterRuntimeException("Cannot access module context from the global context");
        }

        public override FunctionLexicalContext GetFunctionContext()
        {
            throw new ScripterRuntimeException("Cannot access function context from the global context");
        }
    }
}
