namespace ScripterLang
{
    public class ScopedVariableAccessor : VariableAccessor
    {
        private readonly string _name;
        private readonly LexicalContext _context;
        private VariableReference _variable;

        public ScopedVariableAccessor(string name, LexicalContext context)
        {
            _name = name;
            _context = context;
        }

        public override void Bind()
        {
            _variable = _context.GetVariable(_name);
            _variable.EnsureBound();
        }

        public override Value Evaluate()
        {
            return _variable.GetValue();
        }

        public override Value SetVariableValue(Value value)
        {
            _variable.SetValue(value);
            return value;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
