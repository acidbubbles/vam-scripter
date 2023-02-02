namespace ScripterLang
{
    public class IfExpression : Expression
    {
        private readonly Expression _condition;
        private readonly Expression _trueBranch;
        private readonly Expression _falseBranch;

        public IfExpression(Expression condition, Expression trueBranch, Expression falseBranch)
        {
            _condition = condition;
            _trueBranch = trueBranch;
            _falseBranch = falseBranch;
        }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            var condition = _condition.Evaluate(lexicalContext);
            return condition.AsBool
                ? _trueBranch.Evaluate(lexicalContext)
                : _falseBranch.Evaluate(lexicalContext);
        }
    }
}
