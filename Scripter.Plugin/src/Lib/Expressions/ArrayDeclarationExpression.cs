using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class ArrayDeclarationExpression : Expression
    {
        private readonly List<Expression> _expressions;

        public ArrayDeclarationExpression(List<Expression> expressions)
        {
            _expressions = expressions;
        }

        public override void Bind()
        {
            for (var i = 0; i < _expressions.Count; i++)
                _expressions[i].Bind();
        }

        public override Value Evaluate()
        {
            var values = new List<Value>(_expressions.Count);
            for (var i = 0; i < _expressions.Count; i++)
                values.Add(_expressions[i].Evaluate());
            return Value.CreateObject(new ListReference(values));
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _expressions.Select(a => a.ToString()).ToArray())}]";
        }
    }
}
