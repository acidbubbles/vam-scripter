namespace ScripterLang
{
    public class PropertyAccessor : VariableAccessor
    {
        private readonly Expression _left;
        private readonly string _property;

        public PropertyAccessor(Expression left, string property)
        {
            _left = left;
            _property = property;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            return _left.Evaluate(domain).AsObject;
        }

        public override Value SetVariableValue(RuntimeDomain domain, Value value)
        {
            #warning This is dangerous, we should not re-evaluate
            _left.Evaluate(domain).AsObject.Set(_property, value);
            return value;
        }

        public override string ToString()
        {
            return _property;
        }
    }
}
