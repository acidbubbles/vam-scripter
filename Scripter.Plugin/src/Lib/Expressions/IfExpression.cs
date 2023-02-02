namespace ScripterLang
{
    public class IfExpression : Expression
    {
        public IfExpression(Expression condition, Expression trueBranch, Expression falseBranch)
        {
        }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
