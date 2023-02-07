namespace ScripterLang
{
    public class ThrowExpression : Expression
    {
        private readonly Expression _message;

        public ThrowExpression(Expression message)
        {
            _message = message;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var message = _message.Evaluate(domain);
            throw new ScripterRuntimeException(message.ToString());
        }

        public override string ToString()
        {
            return $"throw {_message}";
        }
    }
}
