using System.Diagnostics.CodeAnalysis;

namespace ScripterLang
{
    public class Token
    {
        public static readonly Token None = new Token(TokenType.None, null, new Location());

        public readonly int Type;
        public readonly string Value;
        public readonly Location Location;

        public Token(int type, string value, Location location)
        {
            Type = type;
            Value = value;
            Location = location;
        }

        public bool Match(int type)
        {
            return Type == type;
        }

        public bool Match(int type, string value)
        {
            return Type == type && Value == value;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        public Token Expect(int type)
        {
            if (Type != type)
                throw new ScripterParsingException($"Unexpected token '{Value}'", Location);
            return this;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        public Token Expect(int type, string value)
        {
            if (Type != type && value != Value)
                throw new ScripterParsingException($"Unexpected token '{Value}'; expected {value}", Location);
            return this;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
