using System.Globalization;

namespace ScripterLang
{
    public struct Value
    {
        public static readonly Value Undefined = new Value { Type = ValueTypes.UndefinedType };

        public int Type;
        public double Number;
        public string String;
        public bool AsBool => Number != 0;

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

        public bool Equals(Value other)
        {
            if (Type != other.Type) return false;
            switch (Type)
            {
                case ValueTypes.NumberType: return Number == other.Number;
                case ValueTypes.BooleanType: return Number == other.Number;
                case ValueTypes.StringType: return String == other.String;
                default: return false;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypes.StringType: return String;
                case ValueTypes.NumberType: return Number.ToString(CultureInfo.InvariantCulture);
                case ValueTypes.BooleanType: return AsBool ? "true" : "false";
                default: return ValueTypes.Name(Type);
            }
        }
    }
}
