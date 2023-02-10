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

        public override void Bind()
        {
            _start.Bind();
            _end.Bind();
            _increment.Bind();
            _body.Bind();
        }

        public override Value Evaluate()
        {
            #warning break and continue statements;
            #warning foreach
            #if SCRIPTER_DUMMY_MODE
            const float maxTime = 5;
            var max = Time.time + maxTime;
            #endif
            for (_start.Evaluate(); _end.Evaluate().AsBool; _increment.Evaluate())
            {
                _body.Evaluate();
                #if SCRIPTER_DUMMY_MODE
                if (Time.time > max)
                    throw new ScripterRuntimeException($"Spent more than {maxTime} seconds in the for loop");
                #endif
            }
            return Value.Void;
        }
    }
}
