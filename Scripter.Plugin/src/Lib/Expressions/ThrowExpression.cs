namespace ScripterLang
{
    public class ThrowExpression : Expression
    {
        public ThrowExpression(Expression message)
        {
            Message = message;
        }

        public Expression Message { get; }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var message = Message.Evaluate(domain);
            throw new ScripterRuntimeException(message.ToString());
        }
    }
}
