namespace ScripterLang
{
    public class FloatExpression : Expression
    {
        public FloatExpression(float value)
        {
            Value = value;
        }

        public float Value { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return ScripterLang.Value.CreateFloat(Value);
        }
    }
}
