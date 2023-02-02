namespace ScripterLang
{
    public class WhileExpression : Expression
    {
        public WhileExpression(Expression condition, Expression body)
        {
        }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
