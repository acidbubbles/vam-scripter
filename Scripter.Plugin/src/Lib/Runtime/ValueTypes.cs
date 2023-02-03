namespace ScripterLang
{
    public static class ValueTypes
    {
        public const int UndefinedType = 0;
        public const int Uninitialized = -1;
        public const int IntegerType = TokenType.Integer;
        public const int FloatType = TokenType.Float;
        public const int StringType = TokenType.String;
        public const int BooleanType = TokenType.Boolean;

        public static string Name(int type)
        {
            switch (type)
            {
                case UndefinedType: return "undefined";
                case IntegerType: return "int";
                case FloatType: return "float";
                case StringType: return "string";
                case BooleanType: return "boolean";
                default: return "unknown";
            }
        }
    }
}
