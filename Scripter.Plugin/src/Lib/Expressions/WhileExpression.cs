namespace ScripterLang
{
    public class WhileExpression : Expression
    {
        private readonly Expression _condition;
        private readonly Expression _body;

        public WhileExpression(Expression condition, Expression body)
        {
            _condition = condition;
            _body = body;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            while (_condition.Evaluate(domain).AsBool)
            {
                _body.Evaluate(domain);
            }
            return Value.Void;
        }
    }
}
