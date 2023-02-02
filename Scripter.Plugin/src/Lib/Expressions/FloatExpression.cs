namespace ScripterLang
{
    public class FloatExpression : Expression
    {
        public FloatExpression(float value)
        {
            Value = value;
        }

        public float Value { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return ScripterLang.Value.CreateFloat(Value);
        }
    }
}
