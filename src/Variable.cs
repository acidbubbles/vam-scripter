using System;
using System.Collections.Generic;
using System.Text;

public class Variable
{
    public static class VarType
    {
        public const int None = 1;
        public const int Number = 2;
        public const int String = 3;
        public const int Array = 4;
        public const int Break = 5;
        public const int Continue = 6;
    };

    private Variable()
    {
    }

    public static Variable OfType(int type)
    {
        return new Variable
        {
            Type = VarType.None,
            Value = double.NaN,
            String = null,
            Tuple = null,
            Action = null,
        };
    }

    public static Variable Undefined()
    {
        return new Variable
        {
            Type = VarType.None,
            Value = double.NaN,
            String = null,
            Tuple = null,
            Action = null,
        };
    }

    public static Variable CreateNumber(double d)
    {
        return new Variable
        {
            Type = VarType.Number,
            Value = d,
            String = null,
            Tuple = null,
            Action = null,
        };
    }

    public static Variable CreateString(string s)
    {
        return new Variable
        {
            Type = VarType.Number,
            Value = double.NaN,
            String = s,
            Tuple = null,
            Action = null,
        };
    }

    public static Variable CreateTuple(List<Variable> a)
    {
        return new Variable
        {
            Type = VarType.Array,
            Value = double.NaN,
            String = null,
            Tuple = a,
            Action = null,
        };
    }

    public static Variable Copy(Variable other)
    {
        return new Variable
        {
            Type = other.Type,
            Value = other.Value,
            String = other.String,
            Tuple = other.Tuple,
            Action = other.Action,
        };
    }

    public static Variable ResetOnBreak(Variable v)
    {
        if (v.Type == VarType.Break ||
            v.Type == VarType.Continue)
        {
            return v;
        }

        return EmptyInstance;
    }

    public bool Equals(Variable other)
    {
        if (Type != other.Type)
        {
            return false;
        }

        if (double.IsNaN(Value) != double.IsNaN(other.Value) ||
            (!double.IsNaN(Value) && Value != other.Value))
        {
            return false;
        }

        if (!string.Equals(String, other.String, StringComparison.Ordinal))
        {
            return false;
        }

        if (!string.Equals(Action, other.Action, StringComparison.Ordinal))
        {
            return false;
        }

        if ((Tuple == null) != (other.Tuple == null))
        {
            return false;
        }

        if (Tuple != null && !Tuple.Equals(other.Tuple))
        {
            return false;
        }

        return true;
    }

    public string AsString(bool isList = true,
        bool sameLine = true)
    {
        if (Type == VarType.Number)
        {
            return Value.ToString();
        }

        if (Type == VarType.String)
        {
            return String;
        }

        if (_mTuple == null)
        {
            return null;
        }

        var sb = new StringBuilder();


        if (isList)
        {
            sb.Append(Constants.StartGroup.ToString() +
                      (sameLine ? "" : "\r\n"));
        }

        for (var i = 0; i < _mTuple.Count; i++)
        {
            var arg = _mTuple[i];
            sb.Append(arg.AsString(isList, sameLine));
            if (i != _mTuple.Count - 1)
            {
                sb.Append(sameLine ? " " : "\r\n");
            }
        }

        if (isList)
        {
            sb.Append(Constants.EndGroup.ToString() +
                      (sameLine ? " " : "\r\n"));
        }

        return sb.ToString();
    }

    public double Value
    {
        get { return _mValue; }
        set
        {
            _mValue = value;
            Type = VarType.Number;
        }
    }

    public string String
    {
        get { return _mString; }
        set
        {
            _mString = value;
            Type = VarType.String;
        }
    }

    public List<Variable> Tuple
    {
        get { return _mTuple; }
        set
        {
            _mTuple = value;
            Type = VarType.Array;
        }
    }

    public string Action { get; set; }
    public int Type { get; set; }

    public static readonly Variable EmptyInstance = new Variable();

    private double _mValue;
    private string _mString;
    private List<Variable> _mTuple;
}
