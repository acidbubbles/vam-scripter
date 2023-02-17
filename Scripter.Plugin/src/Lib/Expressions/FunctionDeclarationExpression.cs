using System.Collections.Generic;

namespace ScripterLang
{
    public class FunctionDeclarationExpression : DeclarationExpression
    {
        private readonly List<string> _arguments;
        private readonly CodeBlockExpression _body;
        private readonly FunctionLexicalContext _context;

        public readonly Value functionValue;
        private readonly List<VariableReference> _argumentsVariables;

        public FunctionDeclarationExpression(string name, List<string> arguments, CodeBlockExpression body, FunctionLexicalContext context)
            : base(name)
        {
            _arguments = arguments;
            _body = body;
            _context = context;
            functionValue = Value.CreateFunction(Invoke);
            _argumentsVariables = new List<VariableReference>(arguments.Count);
        }

        public override void Bind()
        {
            for (var i = 0; i < _arguments.Count; i++)
            {
                var arg = _arguments[i];
                var variable = _context.GetVariable(arg);
                _argumentsVariables.Add(variable);
                variable.bound = true;
            }

            _body.Bind();
        }

        public override Value Evaluate()
        {
            return functionValue;
        }

        private Value Invoke(LexicalContext _, Value[] args)
        {
            if (args.Length != _arguments.Count)
                throw new ScripterRuntimeException($"Function {name} requires {_arguments.Count} arguments, but only {args.Length} where provided");

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
                _context.isReturn = false;
            }
        }

        public override string ToString()
        {
            return $"function {name}({string.Join(", ", _arguments.ToArray())}) {{ {_body} }}";
        }
    }
}
