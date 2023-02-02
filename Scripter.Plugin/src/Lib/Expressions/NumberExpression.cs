namespace ScripterLang
{
    public class NumberExpression : Expression
    {
        public NumberExpression(double value)
        {
            Value = value;
        }

        public double Value { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return ScripterLang.Value.CreateNumber(Value);
        }
    }
}
