using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionDeclarationExpression : DeclarationExpression
    {
        private readonly List<string> _arguments;
        private readonly CodeBlockExpression _body;
        private readonly FunctionLexicalContext _context;

        public readonly Value Value;

        public FunctionDeclarationExpression(string name, List<string> arguments, CodeBlockExpression body, FunctionLexicalContext context)
            : base(name)
        {
            _arguments = arguments;
            _body = body;
            _context = context;
            Value = Value.CreateFunction(Invoke);
        }

        public override Value Evaluate()
        {
            return Value;
        }

        private Value Invoke(LexicalContext _, Value[] args)
        {
            if (args.Length != _arguments.Count)
                throw new ScripterRuntimeException($"Function {Name} requires {_arguments.Count} arguments, but only {args.Length} where provided");

            for (var i = 0; i < args.Length; i++)
            {
                var name = _arguments[i];
                _context.CreateVariableValue(name, args[i]);
            }

            try
            {
                return _body.Evaluate();
            }
            finally
            {
                _context.IsReturn = false;
                for (var i = 0; i < _arguments.Count; i++)
                {
                    _context.ClearVariable(_arguments[i]);
                }
            }
        }

        public override string ToString()
        {
            return $"function {Name}({string.Join(", ", _arguments.ToArray())}) {{ {_body} }}";
        }
    }
}
