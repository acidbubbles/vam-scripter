namespace ScripterLang
{
    public class VariableExpression : Expression
    {
        private readonly string _name;

        public VariableExpression(string name)
        {
            _name = name;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return domain.GetVariableValue(_name);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
