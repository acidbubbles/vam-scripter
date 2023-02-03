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
            return domain.Variables[_name];
        }
    }
}
