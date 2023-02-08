namespace ScripterLang
{
    public class VariableExpression : Expression
    {
        private readonly VariableAccessor _accessor;

        public VariableExpression(VariableAccessor accessor)
        {
            _accessor = accessor;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return _accessor.Evaluate(domain);
        }

        public override string ToString()
        {
            return _accessor.ToString();
        }
    }
}
