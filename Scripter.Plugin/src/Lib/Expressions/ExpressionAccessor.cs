using System;

namespace ScripterLang
{
    public class ExpressionAccessor : VariableAccessor
    {
        private readonly Expression _expression;

        public ExpressionAccessor(Expression expression)
        {
            _expression = expression;
        }

        public override void Bind()
        {
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            return _expression.Evaluate();
        }

        public override void SetVariableValue(Value value)
        {
            throw new NotSupportedException("Cannot set the value of an expression.");
        }

        public override Value GetAndHold()
        {
            throw new NotSupportedException("Cannot set the value of an expression.");
        }

        public override void Release()
        {
            throw new NotSupportedException("Cannot set the value of an expression.");
        }

        public override void SetAndRelease(Value value)
        {
            throw new NotSupportedException("Cannot set the value of an expression.");
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}
