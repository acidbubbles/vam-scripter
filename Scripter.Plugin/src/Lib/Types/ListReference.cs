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
                case "push":
                    return Func(Add);
                case "length":
                    return values.Count;
                case "indexOf":
                    return Func(IndexOf);
                case "splice":
                    return Func(Splice);
                case "pop":
                    return Func(Pop);
                case "remove":
                    return Func(Remove);
                case "join":
                    return Func(Join);
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

        private Value Insert(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Insert), args, 2);
            values.Insert(args[0].AsInt, args[1]);
            return Value.Void;
        }

        private Value Splice(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Splice), args, 2);
            var start = args[0].AsInt;
            var count = args[1].AsInt;
            var removedItems = values.GetRange(start, count);
            values.RemoveRange(start, count);
            return new ListReference(removedItems);
        }

        private Value Pop(LexicalContext context, Value[] args)
        {
            if (values.Count <= 0) return Value.Undefined;
            var last = values.Last();
            values.RemoveAt(values.Count - 1);
            return last;
        }

        private Value Join(LexicalContext context, Value[] args)
        {
            var separator = args.Length > 0 ? args[0].AsString : ",";
            return string.Join(separator, values.Select(x => x.ToString()).ToArray());
        }

        private Value Remove(LexicalContext context, Value[] args)
        {
            ValidateArgumentsLength(nameof(Remove), args, 1);
            values.Remove(args[0]);
            return Value.Void;
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", values.Select(x => x.ToCodeString()).ToArray()) + "]";
        }
    }
}
