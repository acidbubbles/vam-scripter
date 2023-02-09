namespace ScripterLang
{
    public class StringExpression : Expression
    {
        private readonly string _value;

        public StringExpression(string value)
        {
            _value = value;
        }

        public override Value Evaluate()
        {
            return Value.CreateString(_value);
        }

        public override string ToString()
        {
            return $"\"{_value}\"";
        }
    }
}
