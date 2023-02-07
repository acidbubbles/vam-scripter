namespace ScripterLang
{
    public class FloatExpression : Expression
    {
        private readonly float _value;

        public FloatExpression(float value)
        {
            _value = value;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Value.CreateFloat(_value);
        }

        public override string ToString()
        {
            return $"{_value}";
        }
    }
}
