using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionDeclaration
    {
        private readonly string _name;
        private readonly List<string> _arguments;
        private readonly FunctionBlockExpression _body;

        public FunctionDeclaration(string name, List<string> arguments, FunctionBlockExpression body)
        {
            _name = name;
            _arguments = arguments;
            _body = body;
        }

        public Value Invoke(RuntimeDomain domain, Value[] args)
        {
            if (args.Length != _arguments.Count)
                throw new ScripterRuntimeException($"Function {_name} requires {_arguments.Count} arguments, but only {args.Length} where provided");

            for (var i = 0; i < args.Length; i++)
            {
                var name = _arguments[i];
                domain.Variables.Add(name, args[i]);
            }

            try
            {
                return _body.Evaluate(domain);
            }
            finally
            {
                for (var i = 0; i < _arguments.Count; i++)
                {
                    domain.ClearVariable(_arguments[i]);
                }
            }
        }
    }
}
