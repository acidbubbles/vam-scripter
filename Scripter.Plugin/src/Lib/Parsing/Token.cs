namespace ScripterLang
{
    public class Token
    {
        public static readonly Token None = new Token(TokenType.None, null, 0);

        public readonly int Type;
        public readonly string Value;
        public readonly int Line;

        public Token(int type, string value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }

        public bool Match(int type)
        {
            return Type == type;
        }

        public bool Match(int type, string value)
        {
            return Type == type && Value == value;
        }

        public Token Expect(int type)
        {
            if (Type != type)
                throw new ScripterParsingException($"Unexpected token '{Value}'");
            return this;
        }

        public Token Expect(int type, string value)
        {
            if (Type != type && value != Value)
                throw new ScripterParsingException($"Unexpected token '{Value}'; expected {value}");
            return this;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
