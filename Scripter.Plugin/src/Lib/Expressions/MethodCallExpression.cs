using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class MethodCallExpression : Expression
    {
        private readonly Expression _left;
        private readonly string _name;
        private readonly Expression[] _arguments;

        public MethodCallExpression(Expression left, string name, IEnumerable<Expression> arguments)
        {
            _left = left;
            _name = name;
            _arguments = arguments.ToArray();
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _left.Evaluate(domain);
            if (!value.IsObject)
                throw new ScripterRuntimeException($"Cannot call method {_name} of value {value} because it is not an object");
            var reference = (Reference)value.AsObject;
            var args = _arguments.Select(arg => arg.Evaluate(domain)).ToArray();
            return reference.Method(_name, args);
        }

        public override string ToString()
        {
            return $"{_left}.{_name}";
        }
    }
}
