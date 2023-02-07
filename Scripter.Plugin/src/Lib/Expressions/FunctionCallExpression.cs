using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class FunctionCallExpression : Expression
    {
        private readonly LexicalContext _lexicalContext;
        private readonly string _name;
        private readonly Expression[] _arguments;

        public FunctionCallExpression(string name, IEnumerable<Expression> arguments, LexicalContext lexicalContext)
        {
            _name = name;
            _arguments = arguments.ToArray();

            _lexicalContext = lexicalContext;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var args = _arguments.Select(arg => arg.Evaluate(domain)).ToArray();
            var func = domain.GetFunction(_lexicalContext, _name);
            return func(domain, args);
        }

        public override string ToString()
        {
            return $"{_name}({string.Join(", ", _arguments.Select(a => a.ToString()).ToArray())})";
        }
    }
}
