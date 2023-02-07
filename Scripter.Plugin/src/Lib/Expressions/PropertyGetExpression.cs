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
            if (!value.IsObject) throw new ScripterRuntimeException($"Value is not an object");
            ReferenceCounted reference;
            if (!domain.Objects.TryGetValue((int)value.FloatValue, out reference))
                throw new ScripterRuntimeException("Null object reference");
            return reference.Reference.Get(_property);
        }

        public override string ToString()
        {
            return $"{_left}.{_property}";
        }
    }
}
