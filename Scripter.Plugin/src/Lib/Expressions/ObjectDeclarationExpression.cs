using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class ObjectDeclarationExpression : Expression
    {
        private readonly KeyValuePair<string, Expression>[] _values;

        public ObjectDeclarationExpression(Dictionary<string, Expression> values)
        {
            _values = values.Select(x =>
                new KeyValuePair<string, Expression>(string.Intern(x.Key), x.Value)
            ).ToArray();
        }

        public override void Bind()
        {
            foreach (var value in _values)
            {
                value.Value.Bind();
            }
        }

        public override Value Evaluate()
        {
            var dict = new Dictionary<string, Value>();
            for (var i = 0; i < _values.Length; i++)
            {
                var kvp = _values[i];
                var value = kvp.Value.Evaluate();
                dict.Add(kvp.Key, value);
            }
            return Value.CreateObject(new MapReference(dict));
        }
    }
}
