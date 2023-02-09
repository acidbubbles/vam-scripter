namespace ScripterLang
{
    public class IncrementDecrementExpression : Expression
    {
        private readonly VariableAccessor _accessor;
        private readonly string _op;
        private readonly bool _returnOriginal;

        public IncrementDecrementExpression(VariableAccessor accessor, string op, bool returnOriginal)
        {
            _accessor = accessor;
            _op = op;
            _returnOriginal = returnOriginal;
        }

        public override Value Evaluate()
        {
            var original = _accessor.Evaluate();
            Value value;
            switch (_op)
            {
                case "++":
                    switch (original.Type)
                    {
                        case ValueTypes.IntegerType:
                            value = Value.CreateInteger(original.RawInt + 1);
                            break;
                        case ValueTypes.FloatType:
                            value = Value.CreateFloat(original.RawFloat + 1);
                            break;
                        default:
                            throw new ScripterRuntimeException($"Cannot increment variable of type {ValueTypes.Name(original.Type)}");
                    }
                    break;
                case "--":
                    switch (original.Type)
                    {
                        case ValueTypes.IntegerType:
                            value = Value.CreateInteger(original.RawInt - 1);
                            break;
                        case ValueTypes.FloatType:
                            value = Value.CreateFloat(original.RawFloat - 1);
                            break;
                        default:
                            throw new ScripterRuntimeException($"Cannot decrement variable of type {ValueTypes.Name(original.Type)}");
                    }
                    break;
                default:
                    throw new ScripterParsingException($"Unexpected operator {_op}");
            }
            _accessor.SetVariableValue(value);
            return _returnOriginal ? original : value;
        }

        public override string ToString()
        {
            return
                _returnOriginal
                    ? $"{_accessor}{_op}"
                    : $"{_op}{_accessor}";
        }
    }
}
