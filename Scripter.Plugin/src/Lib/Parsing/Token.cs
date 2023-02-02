namespace ScripterLang
{
    public class Token
    {
        public int Type;
        public string Value;
        public int Line;

        public Token(int type, string value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }
    }
}
