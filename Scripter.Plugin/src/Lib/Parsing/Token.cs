using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

        [MethodImpl(0x0100)]
        public bool Match(int type)
        {
            return Type == type;
        }

        [MethodImpl(0x0100)]
        public bool Match(int type, string value)
        {
            return Type == type && Value == value;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        [MethodImpl(0x0100)]
        public Token Expect(int type)
        {
            if (Type != type)
            {
                if (type==TokenType.SemiColon)
                {
                    throw new ScripterParsingException($"Expected semicolon, found token '{Value}'", Location);
                }
                throw new ScripterParsingException($"Unexpected token '{Value}'", Location);
            }
            return this;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        [MethodImpl(0x0100)]
        public void Expect(int type, string value)
        {
            if (Type != type && value != Value)
                throw new ScripterParsingException($"Unexpected token '{Value}'; expected {value}", Location);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
