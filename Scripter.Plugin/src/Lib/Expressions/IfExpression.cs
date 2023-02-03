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

        public override Value Evaluate(RuntimeDomain domain)
        {
            var condition = _condition.Evaluate(domain);
            return condition.AsBool
                ? _trueBranch.Evaluate(domain)
                : _falseBranch.Evaluate(domain);
        }
    }
}
