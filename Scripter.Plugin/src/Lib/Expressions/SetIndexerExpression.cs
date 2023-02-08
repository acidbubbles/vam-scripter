namespace ScripterLang
{
    public class SetIndexerExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly Expression _index;
        private readonly Expression _value;

        public SetIndexerExpression(VariableAccessor accessor, Expression index, Expression value)
        {
            _accessor = accessor;
            _index = index;
            _value = value;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var obj = _accessor.Evaluate(domain).AsObject;
            var index = _index.Evaluate(domain);
            var value = _value.Evaluate(domain);
            obj.SetIndex(index, value);
            return Value.Void;
        }

        public override string ToString()
        {
            return $"{_accessor}[{_index}] = {_value}";
        }
    }
}
