namespace ScripterLang
{
    public class EmptyExpression : Expression
    {
        public EmptyExpression()
        {
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return Value.Undefined;
        }
    }
}
