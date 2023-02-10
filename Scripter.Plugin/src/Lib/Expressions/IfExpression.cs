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

        public override void Bind()
        {
            _condition.Bind();
            _trueBranch.Bind();
            _falseBranch?.Bind();
        }

        public override Value Evaluate()
        {
            var condition = _condition.Evaluate();
            if (condition.Boolify)
                return _trueBranch.Evaluate();
            if (_falseBranch != null)
                return _falseBranch.Evaluate();
            return Value.Void;
        }

        public override string ToString()
        {
            return _falseBranch == null
                ? $"if({_condition}) {{\n{_trueBranch}\n}}"
                : $"if({_condition}) {{\n{_trueBranch}\n}} else {{\n{_falseBranch}\n}}";
        }
    }
}
