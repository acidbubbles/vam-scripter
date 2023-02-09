namespace ScripterLang
{
    public class BooleanExpression : Expression
    {
        private readonly bool _value;

        public BooleanExpression(bool value)
        {
            _value = value;
        }

        public override Value Evaluate()
        {
            return Value.CreateBoolean(_value);
        }

        public override string ToString()
        {
            return $"{_value.ToString().ToLowerInvariant()}";
        }
    }
}
