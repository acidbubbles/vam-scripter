namespace ScripterLang
{
    public class ScopedVariableAccessor : VariableAccessor
    {
        private readonly string _name;
        private readonly LexicalContext _context;

        public ScopedVariableAccessor(string name, LexicalContext context)
        {
            _name = name;
            _context = context;
        }

        public override Value Evaluate()
        {
            return _context.GetVariableValue(_name);
        }

        public override Value SetVariableValue(Value value)
        {
            return _context.SetVariableValue(_name, value);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
