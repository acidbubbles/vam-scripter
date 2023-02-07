using UnityEngine;

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
            const float maxTime = 10;
            var max = Time.time + maxTime;
            var iterations = 0;
            var start = _start.Evaluate(domain);
            SuperController.LogMessage("Entering with " + start);
            for (; _end.Evaluate(domain).AsBool; _increment.Evaluate(domain))
            {
                SuperController.LogMessage(domain.GetVariableValue("i") + " is " + _end + " " + _end.Evaluate(domain));
                _body.Evaluate(domain);
                if (iterations++ > 10)
                    throw new ScripterRuntimeException("Too many iterations");
                if (Time.time > max)
                    throw new ScripterRuntimeException($"Spent more than {maxTime} seconds in the for loop");
            }
            return Value.Void;
        }
    }
}
