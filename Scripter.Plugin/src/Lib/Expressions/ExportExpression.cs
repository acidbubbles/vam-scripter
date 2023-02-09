namespace ScripterLang
{
    public class ExportExpression : Expression
    {
        private readonly DeclarationExpression _expression;
        private readonly ScopeLexicalContext _lexicalContext;

        public ExportExpression(DeclarationExpression expression, ScopeLexicalContext lexicalContext)
        {
            _expression = expression;
            _lexicalContext = lexicalContext;
        }

        public override Value Evaluate()
        {
            var value = _expression.Evaluate();
            _lexicalContext.GetModuleContext().Exports.Add(_expression.Name, value);
            return Value.Void;
        }

        public override string ToString()
        {
            return $"export {_expression}";
        }
    }
}
