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

        public override Value Evaluate(RuntimeDomain domain)
        {
            var obj = _accessor.Evaluate(domain).AsObject;
            var index = _index.Evaluate(domain);
            return obj.GetIndex(index);
        }

        public override Value SetVariableValue(RuntimeDomain domain, Value value)
        {
            var obj = _accessor.Evaluate(domain).AsObject;
            #warning We evaluate the object and index twice, again
            var index = _index.Evaluate(domain);
            obj.SetIndex(index, value);
            return value;
        }

        public override string ToString()
        {
            return $"{_accessor}[{_index}]";
        }
    }
}
