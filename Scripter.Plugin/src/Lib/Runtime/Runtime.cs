using System;

namespace ScripterLang
{
    public class Runtime
    {
        public readonly LexicalContext GlobalLexicalContext = new LexicalContext();

        public Runtime()
        {
            GlobalLexicalContext.Functions.Add("print", args =>
            {
                Console.WriteLine(args[0]);
                return Value.Undefined;
            });
            GlobalLexicalContext.Functions.Add("concat", args =>
            {
                var result = "";
                foreach (var arg in args)
                {
                    result += arg.ToString();
                }

                return Value.CreateString(result);
            });
        }

        public Value Evaluate(Expression expression)
        {
            return expression.Evaluate(GlobalLexicalContext);
        }
    }
}
