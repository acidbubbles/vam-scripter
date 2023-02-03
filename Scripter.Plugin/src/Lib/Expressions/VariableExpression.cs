namespace ScripterLang
{
    public class VariableExpression : Expression
    {
        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override Value Evaluate(RuntimeLexicalContext lexicalContext)
        {
            return lexicalContext.Declarations[Name];
        }
    }
}
