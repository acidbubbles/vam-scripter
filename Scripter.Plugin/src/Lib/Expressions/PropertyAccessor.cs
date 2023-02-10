namespace ScripterLang
{
    public class PropertyAccessor : VariableAccessor
    {
        private readonly Expression _left;
        private readonly string _property;
        private ObjectReference _object;

        public PropertyAccessor(Expression left, string property)
        {
            _left = left;
            _property = property;
        }

        public override void Bind()
        {
            _left.Bind();
        }

        public override Value Evaluate()
        {
            return _left.Evaluate().AsObject.Get(_property);
        }

        public override void SetVariableValue(Value value)
        {
            _left.Evaluate().AsObject.Set(_property, value);
        }

        public override Value GetAndHold()
        {
            _object = _left.Evaluate().AsObject;
            return _object.Get(_property);
        }

        public override void Release()
        {
            _object = null;
        }

        public override void SetAndRelease(Value value)
        {
            _object.Set(_property, value);
            _object = null;
        }

        public override string ToString()
        {
            return $"{_left}.{_property}";
        }
    }
}
