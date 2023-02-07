using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ScripterLang
{
    [StructLayout(LayoutKind.Explicit, Size=16)]
    public struct Value
    {
        public const float Epsilon = 1.17549435E-38f;

        public static readonly Value Undefined = new Value { Type = ValueTypes.UndefinedType };
        public static readonly Value Void = new Value { Type = ValueTypes.Uninitialized };

        [FieldOffset(0)] public int Type;
        [FieldOffset(4)] private float FloatValue;
        [FieldOffset(4)] private int IntValue;
        [FieldOffset(8)] private object ObjectValue;

        public bool IsBool
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.BooleanType; }
        }

        public bool AsBool
        {
            [MethodImpl(0x0100)]
            get { return IntValue > 0; }
        }

        public bool IsNumber
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType || Type == ValueTypes.IntegerType; }
        }

        public float AsNumber
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType ? FloatValue : IntValue; }
        }

        public bool IsInt
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.IntegerType; }
        }

        public int AsInt
        {
            [MethodImpl(0x0100)]
            get { return IntValue; }
        }

        public bool IsFloat
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType; }
        }

        public float AsFloat
        {
            [MethodImpl(0x0100)]
            get { return FloatValue; }
        }

        public bool IsObject
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.ObjectType; }
        }

        public object AsObject
        {
            [MethodImpl(0x0100)]
            get { return ObjectValue; }
        }

        public bool IsString
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.StringType; }
        }

        public string AsString
        {
            [MethodImpl(0x0100)]
            get { return IsString ? (string)ObjectValue : ToString(); }
        }

        [MethodImpl(0x0100)]
        public static Value CreateFloat(float value)
        {
            return new Value { Type = ValueTypes.FloatType, FloatValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateInteger(int value)
        {
            return new Value { Type = ValueTypes.IntegerType, IntValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateString(string value)
        {
            return new Value { Type = ValueTypes.StringType, ObjectValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateBoolean(bool value)
        {
            return new Value { Type = ValueTypes.BooleanType, IntValue = value ? 1 : 0 };
        }

        [MethodImpl(0x0100)]
        public static Value CreateObject(Reference value)
        {
            return new Value { Type = ValueTypes.ObjectType, ObjectValue = value };
        }

        [MethodImpl(0x0100)]
        public bool Equals(Value other)
        {
            if (Type != other.Type) return false;
            switch (Type)
            {
                case ValueTypes.FloatType:
                    return other.Type == ValueTypes.FloatType && Math.Abs(AsFloat - other.AsNumber) < Epsilon;
                case ValueTypes.IntegerType:
                    return other.Type == ValueTypes.IntegerType ? AsInt == other.AsInt : Math.Abs(AsNumber - other.AsNumber) < Epsilon;
                case ValueTypes.BooleanType:
                    return AsBool == other.AsBool;
                case ValueTypes.ObjectType:
                case ValueTypes.StringType:
                    return AsObject.Equals(other.AsObject);
                case ValueTypes.UndefinedType:
                    return other.Type == ValueTypes.UndefinedType;
                case ValueTypes.Uninitialized:
                    throw new ScripterRuntimeException("Variable was not initialized");
                default: return false;
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypes.StringType: return $"\"{ObjectValue}\"";
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

        [MethodImpl(0x0100)]
        public static implicit operator Value(Reference value) => CreateObject(value);
    }
}
