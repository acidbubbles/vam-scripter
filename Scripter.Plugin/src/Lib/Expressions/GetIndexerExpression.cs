namespace ScripterLang
{
    public class GetIndexerExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression _index;

        public GetIndexerExpression(VariableAccessor accessor, Expression index)
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

        public override string ToString()
        {
            return $"{_accessor}[{_index}]";
        }
    }
}
