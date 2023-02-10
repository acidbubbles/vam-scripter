namespace ScripterLang
{
    public class ThrowExpression : Expression
    {
        private readonly Expression _message;

        public ThrowExpression(Expression message)
        {
            _message = message;
        }

        public override void Bind()
        {
            _message.Bind();
        }

        public override Value Evaluate()
        {
            var message = _message.Evaluate();
            throw new ScripterRuntimeException(message.Stringify);
        }

        public override string ToString()
        {
            return $"throw {_message}";
        }
    }
}
