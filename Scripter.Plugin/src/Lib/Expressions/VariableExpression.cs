namespace ScripterLang
{
    public class VariableExpression : Expression
    {
        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override Value Evaluate(LexicalContext lexicalContext)
        {
            return lexicalContext.Variables[Name];
        }
    }
}
