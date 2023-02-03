namespace ScripterLang
{
    public class ParenthesesExpression : Expression
    {
        public ParenthesesExpression(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            return Expression.Evaluate(lexicalContext);
        }
    }
}
