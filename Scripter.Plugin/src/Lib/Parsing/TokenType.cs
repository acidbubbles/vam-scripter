namespace ScripterLang
{
    public static class TokenType
    {
        public const int None = 0;

        public const int Undefined = 10;
        public const int Integer = 11;
        public const int Float = 12;
        public const int String = 13;
        public const int Boolean = 14;
        public const int Identifier = 19;

        public const int Comma = 20;
        public const int SemiColon = 21;

        public const int Operator = 30;
        public const int Assignment = 31;
        public const int Negation = 32;
        public const int IncrementDecrement = 33;

        public const int LeftParenthesis = 40;
        public const int RightParenthesis = 41;

        public const int LeftBrace = 50;
        public const int RightBrace = 51;

        public const int Keyword = 100;
    }
}
