using System;
using System.Collections.Generic;

namespace ScripterLang
{
    public class Runtime
    {
        private readonly LexicalContext _lexicalContext = new LexicalContext();

        public Runtime()
        {
            _lexicalContext.Functions.Add("print", args =>
            {
                Console.WriteLine(args[0]);
                return Value.Undefined;
            });
            _lexicalContext.Functions.Add("concat", args =>
            {
                var result = "";
                foreach (var arg in args)
                {
                    result += arg.ToString();
                }

                return Value.CreateString(result);
            });
        }

        public Value Evaluate(IEnumerable<Expression> expressions)
        {
            var last = Value.Undefined;
            foreach (var expression in expressions)
            {
                last = expression.Evaluate(_lexicalContext);
            }
            return last;
        }
    }
}
