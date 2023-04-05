using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class ListReference : ObjectReference
    {
        public readonly List<Value> values;

        public ListReference(List<Value> values)
        {
            this.values = values;
        }

        public override Value GetProperty(string name)
        {
            switch (name)
            {
                case "add":
                    return Func(Add);
                case "length":
                    return values.Count;
                case "indexOf":
                    return Func(IndexOf);
                default:
                    return base.GetProperty(name);
            }
        }

        public override Value GetIndex(Value index)
        {
            return values[index.AsInt];
        }

        public override void SetIndex(Value index, Value value)
        {
            values[index.AsInt] = value;
        }

        private Value Add(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Add), args, 1);
            values.Add(args[0]);
            return Value.Void;
        }

        private Value IndexOf(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(IndexOf), args, 1);
            var expected = args[0];
            for(var i = 0; i < values.Count; i++)
            {
                if (values[i].Equals(expected))
                    return i;
            }
            return -1;
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", values.Select(x => x.ToCodeString()).ToArray()) + "]";
        }
    }
}
