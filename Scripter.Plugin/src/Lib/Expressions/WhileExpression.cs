namespace ScripterLang
{
    public class WhileExpression : Expression
    {
        public WhileExpression(Expression condition, Expression body)
        {
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            throw new System.NotImplementedException();
        }
    }
}
