namespace ScripterLang
{
    public abstract class DeclarationExpression : Expression
    {
        public readonly string name;

        protected DeclarationExpression(string name)
        {
            this.name = name;
        }
    }
}
