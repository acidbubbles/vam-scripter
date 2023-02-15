using System.Collections.Generic;

namespace ScripterLang
{
    public class ObjectDeclarationExpression : Expression
    {
        private readonly Dictionary<string, Expression> _values;

        public ObjectDeclarationExpression(Dictionary<string, Expression> values)
        {
            _values = values;
        }

        public override void Bind()
        {
            foreach (var value in _values.Values)
            {
                value.Bind();
            }
        }

        public override Value Evaluate()
        {
            var dict = new Dictionary<string, Value>();
            foreach (var kvp in _values)
            {
                var value = kvp.Value.Evaluate();
                dict.Add(kvp.Key, value);
            }
            return Value.CreateObject(new MapReference(dict));
        }
    }
}
