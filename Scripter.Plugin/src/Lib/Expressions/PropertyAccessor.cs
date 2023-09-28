using System;
using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class PropertyAccessor : VariableAccessor
    {
        private readonly Expression _left;
        private readonly string _property;
        private ObjectReference _object;

        public PropertyAccessor(Expression left, string property)
        {
            _left = left;
            _property = string.Intern(property);
        }

        public override void Bind()
        {
            _left.Bind();
        }

        public override Value Evaluate()
        {
            var value = _left.Evaluate();
            if (value.IsString)
            {
                return EvaluateStringFunction(value);
            }

            if (_property == "toString")
            {
                return new FunctionReference(((context, args) => value.ToString()));
            }

            return value.AsObject.GetProperty(_property);
        }

        private Value EvaluateStringFunction(Value value)
        {
            var s = value.AsString;
            switch (_property)
            {
                case "length":
                    return s.Length;
                case "startsWith":
                    return new FunctionReference(((context, args) => s.StartsWith(args[0].AsString)));
                case "endsWith":
                    return new FunctionReference(((context, args) => s.EndsWith(args[0].AsString)));
                case "contains":
                    return new FunctionReference(((context, args) => s.Contains(args[0].AsString)));
                case "split":
                    return new FunctionReference(((context, args) =>
                    {
                        if (args.Length == 0) return new ListReference(new List<Value> { s });
                        var splitChars = args[0].AsString.ToCharArray();
                        return new ListReference(s.Split(splitChars).Select(Value.CreateString).ToList());
                    }));
                case "trim":
                    return new FunctionReference(((context, args) =>  s.Trim()));
                case "indexOf":
                    return new FunctionReference(((context, args) => s.IndexOf(args[0].AsString, StringComparison.InvariantCulture)));
                case "substring":
                    return new FunctionReference(((context, args) =>
                    {
                        var start = args[0].AsInt;
                        var end = args.Length > 1 ? args[1].AsInt : s.Length;
                        if (start < 0)
                            start = 0;
                        if (end < 0)
                            end = 0;
                        if (start > end)
                            return string.Empty;
                        if (end > s.Length)
                            end = s.Length;
                        return s.Substring(start, end - start);
                    }));
                case "substr":
                    return new FunctionReference(((context, args) =>
                    {
                        var start = args[0].AsInt;
                        var length = args.Length > 1 ? args[1].AsInt : s.Length;
                        if (start < 0)
                            start = s.Length + start;
                        if (length < 0)
                            length = 0;
                        if (start > s.Length)
                            return string.Empty;
                        if (start + length > s.Length)
                            length = s.Length - start;
                        return s.Substring(start, length);
                    }));
                case "replace":
                    return new FunctionReference(((context, args) =>
                    {
                        var oldStr = args[0].AsString;
                        var newStr = args[1].AsString;
                        return s.Replace(oldStr, newStr);
                    }));
                case "toLowerCase":
                    return new FunctionReference(((context, args) => s.ToLowerInvariant()));
                case "toUpperCase":
                    return new FunctionReference(((context, args) => s.ToUpperInvariant()));
                case "toString":
                    return new FunctionReference(((context, args) => value));
                default:
                    throw new ScripterRuntimeException("There is no property or function named " + _property + " on type string.");
            }
        }

        public override void SetVariableValue(Value setValue)
        {
            var value = _left.Evaluate();
            value.AsObject.SetProperty(_property, setValue);
        }

        public override Value GetAndHold()
        {
            var value = _left.Evaluate();
            _object = value.AsObject;
            return _object.GetProperty(_property);
        }

        public override void Release()
        {
            _object = null;
        }

        public override void SetAndRelease(Value value)
        {
            _object.SetProperty(_property, value);
            _object = null;
        }

        public override string ToString()
        {
            return $"{_left}.{_property}";
        }
    }
}
