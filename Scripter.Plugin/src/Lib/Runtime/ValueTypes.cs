namespace ScripterLang
{
    public static class ValueTypes
    {
        public const ushort UndefinedType = 0;
        public const ushort Uninitialized = 1;
        public const ushort IntegerType = 2;
        public const ushort FloatType = 3;
        public const ushort StringType = 4;
        public const ushort BooleanType = 5;
        public const ushort ObjectType = 6;
        public const ushort FunctionType = 7;

        public static string Name(ushort type)
        {
            switch (type)
            {
                case UndefinedType: return "undefined";
                case IntegerType: return "int";
                case FloatType: return "float";
                case StringType: return "string";
                case BooleanType: return "boolean";
                case ObjectType: return "object";
                case FunctionType: return "function";
                default: return "unknown";
            }
        }
    }
}
