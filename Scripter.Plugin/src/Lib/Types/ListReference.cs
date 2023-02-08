using System.Collections.Generic;

namespace ScripterLang
{
    public class ListReference : ObjectReference
    {
        private readonly List<Value> _values;

        public ListReference(List<Value> values)
        {
            _values = values;
        }

        public override Value Get(string name)
        {
            switch (name)
            {
                case "add":
                    return fn(Add);
                default:
                    return base.Get(name);
            }
        }

        public override Value GetIndex(Value index)
        {
            return _values[index.AsInt];
        }

        public override void SetIndex(Value index, Value value)
        {
            _values[index.AsInt] = value;
        }

        private Value Add(RuntimeDomain domain, Value[] args)
        {
            ValidateArgumentsLength(nameof(Add), args, 1);
            _values.Add(args[0]);
            return Value.Void;
        }
    }
}
