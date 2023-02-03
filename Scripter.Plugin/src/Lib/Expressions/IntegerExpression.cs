namespace ScripterLang
{
    public class IntegerExpression : Expression
    {
        private readonly int _value;

        public IntegerExpression(int value)
        {
            _value = value;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Value.CreateInteger(_value);
        }
    }
}
