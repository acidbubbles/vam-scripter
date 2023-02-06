namespace ScripterLang
{
    public class ReturnExpression : Expression
    {
        private readonly Expression _expression;

        public ReturnExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            domain.IsReturn = true;
            #warning Optional expression
            #warning Allow using undefined (closer to JS)
            return _expression.Evaluate(domain);
        }
    }
}
