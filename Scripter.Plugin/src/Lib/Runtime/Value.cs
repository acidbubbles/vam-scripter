using System;
using System.Globalization;
using System.Runtime.CompilerServices;

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

        public bool IsBool
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.BooleanType; }
        }

        public bool AsBool
        {
            [MethodImpl(0x0100)]
            get { return FloatValue > Epsilon; }
        }

        public bool IsNumber
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType || Type == ValueTypes.IntegerType; }
        }

        public bool IsInt
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.IntegerType; }
        }

        public bool IsFloat
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType; }
        }

        public int AsInt
        {
            [MethodImpl(0x0100)]
            get { return (int)FloatValue; }
        }

        public bool IsString
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.StringType; }
        }

        [MethodImpl(0x0100)]
        public static Value CreateFloat(float value)
        {
            return new Value { Type = ValueTypes.FloatType, FloatValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateInteger(int value)
        {
            return new Value { Type = ValueTypes.IntegerType, FloatValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateString(string value)
        {
            return new Value { Type = ValueTypes.StringType, StringValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateBoolean(bool value)
        {
            return new Value { Type = ValueTypes.BooleanType, FloatValue = value ? 1 : 0 };
        }

        [MethodImpl(0x0100)]
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

        [MethodImpl(0x0100)]
        public static implicit operator Value(string value) => CreateString(value);

        [MethodImpl(0x0100)]
        public static implicit operator Value(int value) => CreateInteger(value);

        [MethodImpl(0x0100)]
        public static implicit operator Value(float value) => CreateFloat(value);

        [MethodImpl(0x0100)]
        public static implicit operator Value(bool value) => CreateBoolean(value);
    }
}
