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

    public Variable()

    {
        Reset();
    }

    public Variable(int type)
    {
        Type = type;
    }

    public Variable(double d)
    {
        Value = d;
    }

    public Variable(string s)
    {
        String = s;
    }

    public Variable(List<Variable> a)
    {
        Tuple = a;
    }

    public Variable(Variable other)
    {
        Copy(other);
    }

    public void Copy(Variable other)
    {
        Reset();
        Action = other.Action;
        Type = other.Type;

        switch (other.Type)
        {
            case VarType.Number:
                Value = other.Value;
                break;
            case VarType.String:
                String = other.String;
                break;
            case VarType.Array:
                Tuple = other.Tuple;
                break;
        }
    }

    public void Reset()
    {
        Value = Double.NaN;
        String = null;
        Tuple = null;
        Action = null;
        Type = VarType.None;
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

        if (Double.IsNaN(Value) != Double.IsNaN(other.Value) ||
            (!Double.IsNaN(Value) && Value != other.Value))
        {
            return false;
        }

        if (!String.Equals(String, other.String, StringComparison.Ordinal))
        {
            return false;
        }

        if (!String.Equals(Action, other.Action, StringComparison.Ordinal))
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

    public static Variable EmptyInstance = new Variable();

    private double _mValue;
    private string _mString;
    private List<Variable> _mTuple;
}
