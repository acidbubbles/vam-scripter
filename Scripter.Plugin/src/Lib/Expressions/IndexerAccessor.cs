namespace ScripterLang
{
    public class IndexerAccessor : VariableAccessor
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression _index;

        public IndexerAccessor(VariableAccessor accessor, Expression index)
        {
            _accessor = accessor;
            _index = index;
        }

        public override Value Evaluate()
        {
            var obj = _accessor.Evaluate().AsObject;
            var index = _index.Evaluate();
            return obj.GetIndex(index);
        }

        public override Value SetVariableValue(Value value)
        {
            var obj = _accessor.Evaluate().AsObject;
            #warning We evaluate the object and index twice, again
            var index = _index.Evaluate();
            obj.SetIndex(index, value);
            return value;
        }

        public override string ToString()
        {
            return $"{_accessor}[{_index}]";
        }
    }
}
