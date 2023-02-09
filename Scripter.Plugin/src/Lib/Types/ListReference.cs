using System.Collections.Generic;
using System.Linq;

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
                    return Func(Add);
                case "length":
                    return _values.Count;
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

        private Value Add(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Add), args, 1);
            _values.Add(args[0]);
            return Value.Void;
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", _values.Select(x => x.ToString()).ToArray()) + "]";
        }
    }
}
