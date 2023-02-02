namespace ScripterLang
{
    public class ReturnExpression : Expression
    {
        public ReturnExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return Expression.Evaluate(lexicalContext);
        }
    }
}
