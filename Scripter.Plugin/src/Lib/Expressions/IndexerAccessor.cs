namespace ScripterLang
{
    public class IndexerAccessor : VariableAccessor
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression _index;

        private ObjectReference _object;
        private Value _indexValue;

        public IndexerAccessor(VariableAccessor accessor, Expression index)
        {
            _accessor = accessor;
            _index = index;
        }

        public override void Bind()
        {
            _accessor.Bind();
            _index.Bind();
        }

        public override Value Evaluate()
        {
            var obj = _accessor.Evaluate().AsObject;
            var index = _index.Evaluate();
            return obj.GetIndex(index);
        }

        public override void SetVariableValue(Value value)
        {
            var obj = _accessor.Evaluate().AsObject;
            var index = _index.Evaluate();
            obj.SetIndex(index, value);
        }

        public override Value GetAndHold()
        {
            _object = _accessor.Evaluate().AsObject;
            _indexValue = _index.Evaluate();
            return _object.GetIndex(_indexValue);
        }

        public override void Release()
        {
            _object = null;
            _indexValue = Value.Undefined;
        }

        public override void SetAndRelease(Value value)
        {
            _object.SetIndex(_indexValue, value);
            _object = null;
            _indexValue = Value.Undefined;
        }

        public override string ToString()
        {
            return $"{_accessor}[{_index}]";
        }
    }
}
