using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class MapReference : ObjectReference
    {
        private readonly Dictionary<string, Value> _values;

        public MapReference(Dictionary<string, Value> values)
        {
            _values = values;
        }

        public override Value GetProperty(string name)
        {
            Value value;
            if (!_values.TryGetValue(name, out value))
                return Value.Undefined;
            return value;
        }

        public override void SetProperty(string name, Value value)
        {
            _values[name] = value;
        }

        public override Value GetIndex(Value index)
        {
            return GetProperty(index.AsString);
        }

        public override void SetIndex(Value index, Value value)
        {
            SetProperty(index.AsString, value);
        }

        public override string ToString()
        {
            return "{ " + string.Join(", ", _values.Select(x => $"{x.Key}: {x.Value.ToCodeString()}").ToArray()) + " }";
        }
    }
}
