namespace ScripterLang
{
    public class ExportExpression : Expression
    {
        private readonly DeclarationExpression _expression;
        private readonly ModuleLexicalContext _moduleLexicalContext;

        public ExportExpression(DeclarationExpression expression, LexicalContext lexicalContext)
        {
            _expression = expression;
            _moduleLexicalContext = lexicalContext.GetModuleContext();
        }

        public override Value Evaluate()
        {
            var value = _expression.Evaluate();
            _moduleLexicalContext.Module.Exports.Add(_expression.Name, value);
            return Value.Void;
        }

        public override string ToString()
        {
            return $"export {_expression}";
        }
    }
}
