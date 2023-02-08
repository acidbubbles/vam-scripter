using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class FunctionCallExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression[] _arguments;

        public FunctionCallExpression(VariableAccessor accessor, IEnumerable<Expression> arguments)
        {
            _accessor = accessor;
            _arguments = arguments.ToArray();
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _accessor.Evaluate(domain);
            var fn = value.AsFunction;
            var args = _arguments.Select(arg => arg.Evaluate(domain)).ToArray();
            return fn(domain, args);
        }

        public override string ToString()
        {
            return $"{_accessor}({string.Join(", ", _arguments.Select(a => a.ToString()).ToArray())})";
        }
    }
}
