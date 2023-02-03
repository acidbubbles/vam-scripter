using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class FunctionCallExpression : Expression
    {
        public FunctionCallExpression(string name, IEnumerable<Expression> arguments)
        {
            Name = name;
            Arguments = arguments.ToArray();
        }

        public string Name { get; }
        public Expression[] Arguments { get; }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            var args = Arguments.Select(arg => arg.Evaluate(lexicalContext)).ToArray();
            var func = lexicalContext.GetFunction(Name);
            return func(args);
        }
    }
}
