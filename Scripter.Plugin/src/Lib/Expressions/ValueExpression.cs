namespace ScripterLang
{
    public class ValueExpression : Expression
    {
        private readonly Value _value;

        public ValueExpression(Value value)
        {
            _value = value;
        }

        public override Value Evaluate()
        {
            return _value;
        }

        public override string ToString()
        {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            return _value.ToCodeString();
        }
    }
}
