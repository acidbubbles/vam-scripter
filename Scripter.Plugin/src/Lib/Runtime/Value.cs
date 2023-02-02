using System.Globalization;

namespace ScripterLang
{
    public struct Value
    {
        public static readonly Value Undefined = new Value { Type = ValueTypes.UndefinedType };

        public int Type;
        public double Number;
        public string String;

        public static Value CreateNumber(double value)
        {
            return new Value { Type = ValueTypes.NumberType, Number = value };
        }

        public static Value CreateString(string value)
        {
            return new Value { Type = ValueTypes.StringType, String = value };
        }

        public static Value CreateBoolean(bool value)
        {
            return new Value { Type = ValueTypes.BooleanType, Number = value ? 1 : 0 };
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypes.StringType: return String;
                case ValueTypes.NumberType: return Number.ToString(CultureInfo.InvariantCulture);
                case ValueTypes.BooleanType: return Number == 0 ? "false" : "true";
                default: return ValueTypes.Name(Type);
            }
        }
    }
}
