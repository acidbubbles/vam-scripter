namespace ScripterLang
{
    public static class ValueTypes
    {
        public const int UndefinedType = 0;
        public const int NumberType = 1;
        public const int StringType = 2;
        public const int BooleanType = 3;

        public static string Name(int type)
        {
            switch (type)
            {
                case UndefinedType: return "undefined";
                case NumberType: return "number";
                case StringType: return "string";
                case BooleanType: return "boolean";
                default: return "unknown";
            }
        }
    }
}
