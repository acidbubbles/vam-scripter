namespace ScripterLang
{
    public class ScopedVariableAccessor : VariableAccessor
    {
        private readonly string _name;

        public ScopedVariableAccessor(string name)
        {
            _name = name;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return domain.GetVariableValue(_name);
        }

        public override Value SetVariableValue(RuntimeDomain domain, Value value)
        {
            return domain.SetVariableValue(_name, value);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
