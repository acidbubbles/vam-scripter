namespace ScripterLang
{
    public class StringExpression : Expression
    {
        private readonly string _value;

        public StringExpression(string value)
        {
            _value = value;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return ScripterLang.Value.CreateString(_value);
        }
    }
}
