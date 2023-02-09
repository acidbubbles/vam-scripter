using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class FunctionCallExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression[] _arguments;
        private readonly LexicalContext _context;

        public FunctionCallExpression(VariableAccessor accessor, IEnumerable<Expression> arguments, LexicalContext context)
        {
            _accessor = accessor;
            _arguments = arguments.ToArray();
            _context = context;
        }

        public override Value Evaluate()
        {
            var value = _accessor.Evaluate();
            var fn = value.AsFunction;
            var args = _arguments.Select(arg => arg.Evaluate()).ToArray();
            return fn(_context, args);
        }

        public override string ToString()
        {
            return $"{_accessor}({string.Join(", ", _arguments.Select(a => a.ToString()).ToArray())})";
        }
    }
}
