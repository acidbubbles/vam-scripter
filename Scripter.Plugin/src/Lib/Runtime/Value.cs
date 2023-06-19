using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ScripterLang
{
    [StructLayout(LayoutKind.Explicit, Size=16)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public struct Value
    {
        public const float Epsilon = 1.17549435E-38f;

        public static readonly Value Undefined = new Value { Type = ValueTypes.UndefinedType };
        public static readonly Value Void = new Value { Type = ValueTypes.Uninitialized };
        public static readonly Value[] EmptyValues = new Value[0];

        [FieldOffset(0)] public ushort Type;
        [FieldOffset(4)] private float _floatValue;
        [FieldOffset(4)] private int _intValue;
        [FieldOffset(8)] private object _objectValue;

        public bool IsUndefined
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.UndefinedType; }
        }

        public bool IsBool
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.BooleanType; }
        }

        public bool RawBool
        {
            [MethodImpl(0x0100)]
            get { return _intValue > 0; }
        }

        public bool AsBool
        {
            [MethodImpl(0x0100)]
            get { return IsBool ? RawBool : ThrowInvalidType<bool>(ValueTypes.BooleanType); }
        }

        public bool IsNumber
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType || Type == ValueTypes.IntegerType; }
        }

        public float AsNumber
        {
            [MethodImpl(0x0100)]
            get
            {
                switch (Type)
                {
                    case ValueTypes.FloatType:
                        return _floatValue;
                    case ValueTypes.IntegerType:
                        return _intValue;
                    default:
                        return ThrowInvalidType<float>(ValueTypes.FloatType);
                }
            }
        }

        public bool IsInt
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.IntegerType; }
        }

        public int RawInt
        {
            [MethodImpl(0x0100)]
            get { return _intValue; }
        }

        public int AsInt
        {
            [MethodImpl(0x0100)]
            get { return IsInt ? RawInt : ThrowInvalidType<int>(ValueTypes.IntegerType); }
        }

        public bool IsFloat
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FloatType; }
        }

        public float RawFloat
        {
            [MethodImpl(0x0100)]
            get { return _floatValue; }
        }

        public float AsFloat
        {
            [MethodImpl(0x0100)]
            get { return IsFloat ? RawFloat : ThrowInvalidType<float>(ValueTypes.FloatType); }
        }

        public bool IsObject
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.ObjectType; }
        }

        public ObjectReference RawObject
        {
            [MethodImpl(0x0100)]
            get { return (ObjectReference)_objectValue; }
        }

        public ObjectReference AsObject
        {
            [MethodImpl(0x0100)]
            get { return IsObject ? RawObject : ThrowInvalidType<ObjectReference>(ValueTypes.ObjectType); }
        }

        public bool IsString
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.StringType; }
        }

        public string RawString
        {
            [MethodImpl(0x0100)]
            get { return (string)_objectValue; }
        }

        public string AsString
        {
            [MethodImpl(0x0100)]
            get { return IsString ? RawString : ThrowInvalidType<string>(ValueTypes.StringType); }
        }

        public bool IsFunction
        {
            [MethodImpl(0x0100)]
            get { return Type == ValueTypes.FunctionType; }
        }

        public FunctionReference RawFunction
        {
            [MethodImpl(0x0100)]
            get { return (FunctionReference)_objectValue; }
        }

        public FunctionReference AsFunction
        {
            [MethodImpl(0x0100)]
            get { return IsFunction ? RawFunction : ThrowInvalidType<FunctionReference>(ValueTypes.FunctionType); }
        }

        public string Stringify
        {
            [MethodImpl(0x0100)]
            get { return ToString(); }
        }

        public bool Boolify
        {
            [MethodImpl(0x0100)]
            get
            {
                switch (Type)
                {
                    case ValueTypes.BooleanType:
                        return RawBool;
                    case ValueTypes.IntegerType:
                        return RawInt > 0;
                    case ValueTypes.FloatType:
                        return RawFloat == 0;
                    case ValueTypes.ObjectType:
                    case ValueTypes.FunctionType:
                        return RawObject != null;
                    case ValueTypes.StringType:
                        return !string.IsNullOrEmpty(RawString);
                    default:
                        return false;
                }
            }
        }

        [MethodImpl(0x0100)]
        public static Value CreateFloat(float value)
        {
            return new Value { Type = ValueTypes.FloatType, _floatValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateInteger(int value)
        {
            return new Value { Type = ValueTypes.IntegerType, _intValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateString(string value)
        {
            if (value == null) throw new ScripterRuntimeException("String cannot be null");
            return new Value { Type = ValueTypes.StringType, _objectValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateBoolean(bool value)
        {
            return new Value { Type = ValueTypes.BooleanType, _intValue = value ? 1 : 0 };
        }

        [MethodImpl(0x0100)]
        public static Value CreateObject(ObjectReference value)
        {
            return new Value { Type = ValueTypes.ObjectType, _objectValue = value };
        }

        [MethodImpl(0x0100)]
        public static Value CreateFunction(FunctionReference fn)
        {
            return new Value { Type = ValueTypes.FunctionType, _objectValue = fn };
        }

        private T ThrowInvalidType<T>(ushort expectedType)
        {
            throw new ScripterRuntimeException($"Unexpected type {ValueTypes.Name(Type)}, expected {ValueTypes.Name(expectedType)}");
        }

        [MethodImpl(0x0100)]
        public bool Equals(Value other)
        {
            if (Type != other.Type) return false;
            // throw new Exception( (ObjectValue == null) + " " + (ObjectValue == null));
            switch (Type)
            {
                case ValueTypes.FloatType:
                    return other.Type == ValueTypes.FloatType && Math.Abs(RawFloat - other.AsNumber) < Epsilon;
                case ValueTypes.IntegerType:
                    return other.Type == ValueTypes.IntegerType ? RawInt == other.RawInt : Math.Abs(AsNumber - other.AsNumber) < Epsilon;
                case ValueTypes.BooleanType:
                    return RawBool == other.RawBool;
                case ValueTypes.ObjectType:
                case ValueTypes.FunctionType:
                    return AsObject == other.RawObject;
                case ValueTypes.StringType:
                    return AsString == other.Stringify;
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
                case ValueTypes.StringType: return RawString;
                case ValueTypes.FloatType: return RawFloat.ToString(CultureInfo.InvariantCulture);
                case ValueTypes.IntegerType: return RawInt.ToString();
                case ValueTypes.BooleanType: return RawBool ? "true" : "false";
                case ValueTypes.ObjectType: return RawObject.ToString();
                default: return ValueTypes.Name(Type);
            }
        }

        public string ToCodeString()
        {
            if (Type == ValueTypes.StringType)
                return "\"" + RawString + "\"";
            else
                return ToString();
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
        public static implicit operator Value(ObjectReference value) => CreateObject(value);

        [MethodImpl(0x0100)]
        public static implicit operator Value(FunctionReference value) => CreateFunction(value);
    }
}
