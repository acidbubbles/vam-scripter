namespace ScripterLang
{
    public class StringExpression : Expression
    {
        public StringExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return ScripterLang.Value.CreateString(Value);
        }
    }
}
