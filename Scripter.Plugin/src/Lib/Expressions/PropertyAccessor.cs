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

        public override Value Evaluate()
        {
            return _left.Evaluate().AsObject.Get(_property);
        }

        public override Value SetVariableValue(Value value)
        {
            #warning This is dangerous, we should not re-evaluate
            _left.Evaluate().AsObject.Set(_property, value);
            return value;
        }

        public override string ToString()
        {
            return $"{_left}.{_property}";
        }
    }
}
