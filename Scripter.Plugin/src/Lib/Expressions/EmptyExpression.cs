namespace ScripterLang
{
    public class EmptyExpression : Expression
    {
        public EmptyExpression()
        {
        }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            return Value.Undefined;
        }
    }
}
