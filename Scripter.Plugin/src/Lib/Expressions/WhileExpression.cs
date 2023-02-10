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

        public override void Bind()
        {
            _condition.Bind();
            _body.Bind();
        }

        public override Value Evaluate()
        {
            while (_condition.Evaluate().AsBool)
            {
                _body.Evaluate();
            }
            return Value.Void;
        }
    }
}
