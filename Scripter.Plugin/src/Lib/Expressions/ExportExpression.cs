namespace ScripterLang
{
    public class ExportExpression : Expression
    {
        private readonly DeclarationExpression _expression;
        private readonly ModuleLexicalContext _moduleLexicalContext;

        public ExportExpression(DeclarationExpression expression, ModuleLexicalContext lexicalContext)
        {
            _expression = expression;
            _moduleLexicalContext = lexicalContext;
        }

        public override void Bind()
        {
            _expression.Bind();
        }

        public override Value Evaluate()
        {
            var value = _expression.Evaluate();
            _moduleLexicalContext.module.exports.Add(_expression.name, value);
            return Value.Void;
        }

        public override string ToString()
        {
            return $"export {_expression}";
        }
    }
}
