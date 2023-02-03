namespace ScripterLang
{
    public class WhileExpression : Expression
    {
        public WhileExpression(Expression condition, Expression body)
        {
        }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
