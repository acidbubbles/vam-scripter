using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class FunctionCallExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression[] _arguments;
        private readonly LexicalContext _context;

        private readonly Value[] _argumentValues;

        public FunctionCallExpression(VariableAccessor accessor, IEnumerable<Expression> arguments, LexicalContext context)
        {
            _accessor = accessor;
            _arguments = arguments.ToArray();
            _argumentValues = new Value[_arguments.Length];
            _context = context;
        }

        public override Value Evaluate()
        {
            var value = _accessor.Evaluate();
            var fn = value.AsFunction;
            for(var i = 0; i < _arguments.Length; i++)
                _argumentValues[i] = _arguments[i].Evaluate();
            return fn(_context, _argumentValues);
        }

        public override string ToString()
        {
            return $"{_accessor}({string.Join(", ", _arguments.Select(a => a.ToString()).ToArray())})";
        }
    }
}
