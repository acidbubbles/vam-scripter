namespace ScripterLang
{
    public class EmptyExpression : Expression
    {
        public EmptyExpression()
        {
        }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return Value.Undefined;
        }
    }
}
