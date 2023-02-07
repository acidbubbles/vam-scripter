using System;
using System.Globalization;

namespace ScripterLang
{
    public struct Value
    {
        public const float Epsilon = 1.17549435E-38f;

        public static readonly Value Undefined = new Value { Type = ValueTypes.UndefinedType };
        public static readonly Value Void = new Value { Type = ValueTypes.Uninitialized };

        public int Type;
        public float FloatValue;
        public string StringValue;

        public bool IsBool => Type == ValueTypes.BooleanType;
        public bool AsBool => FloatValue > Epsilon;
        public bool IsNumber => Type == ValueTypes.FloatType || Type == ValueTypes.IntegerType;
        public bool IsInt => Type == ValueTypes.IntegerType;
        public bool IsFloat => Type == ValueTypes.FloatType;
        public float AsFloat => FloatValue;
        public int AsInt => (int)FloatValue;
        public bool IsString => Type == ValueTypes.StringType;

        public static Value CreateFloat(float value)
        {
            return new Value { Type = ValueTypes.FloatType, FloatValue = value };
        }

        public static Value CreateInteger(int value)
        {
            return new Value { Type = ValueTypes.IntegerType, FloatValue = value };
        }

        public static Value CreateString(string value)
        {
            return new Value { Type = ValueTypes.StringType, StringValue = value };
        }

        public static Value CreateBoolean(bool value)
        {
            return new Value { Type = ValueTypes.BooleanType, FloatValue = value ? 1 : 0 };
        }

        public bool Equals(Value other)
        {
            if (Type != other.Type) return false;
            switch (Type)
            {
                case ValueTypes.FloatType:
                case ValueTypes.IntegerType:
                case ValueTypes.BooleanType:
                    return Math.Abs(FloatValue - other.FloatValue) < Epsilon;
                case ValueTypes.StringType:
                    return StringValue == other.StringValue;
                case ValueTypes.UndefinedType:
                    return true;
                case ValueTypes.Uninitialized:
                    throw new ScripterRuntimeException("Variable was not initialized");
                default: return false;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypes.StringType: return StringValue;
                case ValueTypes.FloatType: return FloatValue.ToString(CultureInfo.InvariantCulture);
                case ValueTypes.IntegerType: return AsInt.ToString();
                case ValueTypes.BooleanType: return AsBool ? "true" : "false";
                default: return ValueTypes.Name(Type);
            }
        }

        public static implicit operator Value(string value) => CreateString(value);
        public static implicit operator Value(int value) => CreateInteger(value);
        public static implicit operator Value(float value) => CreateFloat(value);
        public static implicit operator Value(bool value) => CreateBoolean(value);
    }
}
