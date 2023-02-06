namespace ScripterLang
{
    public class ForExpression : Expression
    {
        private readonly Expression _start;
        private readonly Expression _end;
        private readonly Expression _increment;
        private readonly Expression _body;

        public ForExpression(Expression start, Expression end, Expression increment, Expression body)
        {
            _start = start;
            _end = end;
            _increment = increment;
            _body = body;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            for (_start.Evaluate(domain); _end.Evaluate(domain).AsBool; _increment.Evaluate(domain))
            {
                _body.Evaluate(domain);
            }
            return Value.Void;
        }
    }
}
