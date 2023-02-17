using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ScripterLang
{
    public class Token
    {
        public static readonly Token None = new Token(TokenType.None, "EOF", new Location());

        public readonly int type;
        public readonly string value;
        public readonly Location location;

        public Token(int type, string value, Location location)
        {
            this.type = type;
            this.value = value;
            this.location = location;
        }

        [MethodImpl(0x0100)]
        public bool Match(int type)
        {
            return this.type == type;
        }

        [MethodImpl(0x0100)]
        public bool Match(int type, string value)
        {
            return this.type == type && this.value == value;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        [MethodImpl(0x0100)]
        public Token Expect(int type)
        {
            if (this.type != type)
            {
                if (type==TokenType.SemiColon)
                {
                    throw new ScripterParsingException($"Expected semicolon, found token '{value}'", location);
                }
                throw new ScripterParsingException($"Unexpected token '{value}'", location);
            }
            return this;
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        [MethodImpl(0x0100)]
        public void Expect(int type, string value)
        {
            if (this.type != type && value != this.value)
                throw new ScripterParsingException($"Unexpected token '{this.value}'; expected '{value}'", location);
        }

        public override string ToString()
        {
            return value;
        }
    }
}
