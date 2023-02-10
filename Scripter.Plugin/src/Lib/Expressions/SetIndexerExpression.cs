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

        public override void Bind()
        {
            _accessor.Bind();
            _index.Bind();
            _value.Bind();
        }

        public override Value Evaluate()
        {
            var obj = _accessor.Evaluate().AsObject;
            var index = _index.Evaluate();
            var value = _value.Evaluate();
            obj.SetIndex(index, value);
            return Value.Void;
        }

        public override string ToString()
        {
            return $"{_accessor}[{_index}] = {_value}";
        }
    }
}
