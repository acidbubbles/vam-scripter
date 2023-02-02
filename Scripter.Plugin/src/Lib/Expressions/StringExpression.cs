namespace ScripterLang
{
    public class StringExpression : Expression
    {
        public StringExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return ScripterLang.Value.CreateString(Value);
        }
    }
}
