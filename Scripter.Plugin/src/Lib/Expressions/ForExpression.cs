namespace ScripterLang
{
    public class ForExpression : Expression
    {
        public ForExpression(Expression start, Expression end, Expression increment, Expression body)
        {
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            throw new System.NotImplementedException();
        }
    }
}
