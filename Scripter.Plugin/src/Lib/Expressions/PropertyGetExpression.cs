namespace ScripterLang
{
    public class PropertyGetExpression : Expression
    {
        private readonly Expression _left;
        private readonly string _property;

        public PropertyGetExpression(Expression left, string property)
        {
            _left = left;
            _property = property;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var value = _left.Evaluate(domain);
            if (!value.IsObject)
                throw new ScripterRuntimeException($"Cannot get property {_property} of value {value} because it is not an object");
            var reference = (Reference)value.AsObject;
            return reference.Get(_property);
        }

        public override string ToString()
        {
            return $"{_left}.{_property}";
        }
    }
}
