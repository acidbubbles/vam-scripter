namespace ScripterLang
{
    public abstract class DeclarationExpression : Expression
    {
        public readonly string Name;

        protected DeclarationExpression(string name)
        {
            Name = name;
        }
    }
}
