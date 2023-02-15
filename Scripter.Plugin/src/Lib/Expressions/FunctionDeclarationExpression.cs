using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionDeclarationExpression : DeclarationExpression
    {
        private readonly List<string> _arguments;
        private readonly CodeBlockExpression _body;
        private readonly FunctionLexicalContext _context;

        public readonly Value FunctionValue;
        private readonly List<VariableReference> _argumentsVariables;

        public FunctionDeclarationExpression(string name, List<string> arguments, CodeBlockExpression body, FunctionLexicalContext context)
            : base(name)
        {
            _arguments = arguments;
            _body = body;
            _context = context;
            FunctionValue = Value.CreateFunction(Invoke);
            _argumentsVariables = new List<VariableReference>(arguments.Count);
        }

        public override void Bind()
        {
            for (var i = 0; i < _arguments.Count; i++)
            {
                var arg = _arguments[i];
                var variable = _context.GetVariable(arg);
                _argumentsVariables.Add(variable);
                variable.Bound = true;
            }

            _body.Bind();
        }

        public override Value Evaluate()
        {
            return FunctionValue;
        }

        private Value Invoke(LexicalContext _, Value[] args)
        {
            if (args.Length != _arguments.Count)
                throw new ScripterRuntimeException($"Function {Name} requires {_arguments.Count} arguments, but only {args.Length} where provided");

            for (var i = 0; i < args.Length; i++)
            {
                var variable = _argumentsVariables[i];
                variable.Initialize(args[i]);
            }

            try
            {
                return _body.Evaluate();
            }
            finally
            {
                _context.Exit();
                _context.IsReturn = false;
            }
        }

        public override string ToString()
        {
            return $"function {Name}({string.Join(", ", _arguments.ToArray())}) {{ {_body} }}";
        }
    }
}
