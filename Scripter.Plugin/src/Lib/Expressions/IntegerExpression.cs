namespace ScripterLang
{
    public class IntegerExpression : Expression
    {
        public IntegerExpression(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return ScripterLang.Value.CreateInteger(Value);
        }
    }
}
