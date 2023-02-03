namespace ScripterLang
{
    public class ForExpression : Expression
    {
        public ForExpression(Expression start, Expression end, Expression increment, Expression body)
        {
        }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
