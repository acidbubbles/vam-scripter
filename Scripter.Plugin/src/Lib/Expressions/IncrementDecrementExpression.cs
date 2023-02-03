namespace ScripterLang
{
    public class IncrementDecrementExpression : Expression
    {
        private readonly string _name;
        private readonly string _op;
        private readonly bool _returnOriginal;

        public IncrementDecrementExpression(string name, string op, bool returnOriginal)
        {
            _op = op;
            _name = name;
            _returnOriginal = returnOriginal;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var original = domain.GetVariableValue(_name);
            Value value;
            switch (_op)
            {
                case "++":
                    switch (original.Type)
                    {
                        case ValueTypes.IntegerType:
                            value = Value.CreateInteger(original.IntValue + 1);
                            break;
                        case ValueTypes.FloatType:
                            value = Value.CreateFloat(original.FloatValue + 1);
                            break;
                        default:
                            throw new ScripterRuntimeException($"Cannot increment variable of type {ValueTypes.Name(original.Type)}");
                    }
                    break;
                case "--":
                    switch (original.Type)
                    {
                        case ValueTypes.IntegerType:
                            value = Value.CreateInteger(original.IntValue - 1);
                            break;
                        case ValueTypes.FloatType:
                            value = Value.CreateFloat(original.FloatValue - 1);
                            break;
                        default:
                            throw new ScripterRuntimeException($"Cannot decrement variable of type {ValueTypes.Name(original.Type)}");
                    }
                    break;
                default:
                    throw new ScripterParsingException($"Unexpected operator {_op}");
            }
            domain.SetVariableValue(_name, value);
            return _returnOriginal ? original : value;
        }
    }
}
