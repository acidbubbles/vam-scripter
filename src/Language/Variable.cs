using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
    public class Variable
    {
        public enum VarType
        {
            NONE, UNDEFINED, NUMBER, STRING, ARRAY,
            ARRAY_NUM, ARRAY_STR, ARRAY_INT, INT,
            BREAK, CONTINUE, VARIABLE, CUSTOM,
        };
        public enum OriginalType
        {
            NONE, UNDEFINED, INT, LONG, BOOL, DOUBLE, STRING, ARRAY
        };

        public Variable()
        {
            Reset();
        }
        public Variable(VarType type)
        {
            Type = type;
            if (Type == VarType.ARRAY)
            {
                SetAsArray();
            }
        }
        public Variable(double d)
        {
            Value = d;
            Original = OriginalType.DOUBLE;
        }
        public Variable(int d)
        {
            Value = d;
            Original = OriginalType.INT;
        }
        public Variable(long d)
        {
            Value = d;
            Original = OriginalType.LONG;
        }
        public Variable(bool d)
        {
            Value = d ? 1.0 : 0.0;
            Original = OriginalType.BOOL;
        }
        public Variable(string s)
        {
            String = s;
            Original = OriginalType.STRING;
        }
        public Variable(List<Variable> a)
        {
            this.Tuple = a;
            Original = OriginalType.ARRAY;
        }
        public Variable(List<string> a)
        {
            List<Variable> tuple = new List<Variable>(a.Count);
            for (int i = 0; i < a.Count; i++)
            {
                tuple.Add(new Variable(a[i]));
            }
            this.Tuple = tuple;
            Original = OriginalType.ARRAY;
        }
        public Variable(List<double> a)
        {
            List<Variable> tuple = new List<Variable>(a.Count);
            for (int i = 0; i < a.Count; i++)
            {
                tuple.Add(new Variable(a[i]));
            }
            this.Tuple = tuple;
            Original = OriginalType.ARRAY;
        }

        public static Variable NewEmpty()
        {
            return new Variable();
        }

        public static Variable ConvertToVariable(object obj, Type objectType = null)
        {
            if (obj == null)
            {
                return Variable.EmptyInstance;
            }
            if (obj is Variable)
            {
                return (Variable)obj;
            }
            if (obj is string || obj is char)
            {
                return new Variable(Convert.ToString(obj));
            }
            if (obj is double || obj is float || obj is int || obj is long)
            {
                return new Variable(Convert.ToDouble(obj));
            }
            if (obj is bool)
            {
                return new Variable(((bool)obj));
            }
            if (obj is List<string>)
            {
                return new Variable(((List<string>)obj));
            }
            if (obj is List<double>)
            {
                return new Variable(((List<double>)obj));
            }

            throw new ArgumentException($"Unexpected argument type: {obj.GetType()}", nameof(obj));
        }

        public void Reset()
        {
            m_value = Double.NaN;
            m_string = null;
            m_tuple = null;
            Action = null;
            IsReturn = false;
            Type = VarType.NONE;
        }

        public bool Equals(Variable other)
        {
            if (Type != other.Type)
            {
                return false;
            }

            if (Type == VarType.NUMBER && Value == other.Value)
            {
                return true;
            }
            bool stringsEqual = String.Equals(this.String, other.String, StringComparison.Ordinal);
            if (Type == VarType.STRING && stringsEqual)
            {
                return true;
            }

            if (Double.IsNaN(Value) != Double.IsNaN(other.Value) ||
              (!Double.IsNaN(Value) && Value != other.Value))
            {
                return false;
            }
            if (!String.Equals(this.Action, other.Action, StringComparison.Ordinal))
            {
                return false;
            }
            if ((this.Tuple == null) != (other.Tuple == null))
            {
                return false;
            }
            if (this.Tuple != null && !this.Tuple.Equals(other.Tuple))
            {
                return false;
            }
            if (!stringsEqual)
            {
                return false;
            }
            return AsString() == other.AsString();
        }

        public virtual bool Preprocess()
        {
            return false;
        }

        public int GetArrayIndex(Variable indexVar)
        {
            if (this.Type != VarType.ARRAY)
            {
                return -1;
            }

            if (indexVar.Type == VarType.NUMBER)
            {
                Utils.CheckNonNegativeInt(indexVar, null);
                return (int)indexVar.Value;
            }

            return -1;
        }

        public void AddVariable(Variable v, int index = -1)
        {
            SetAsArray();
            if (index < 0 || m_tuple.Count <= index)
            {
                m_tuple.Add(v);
            }
            else
            {
                m_tuple.Insert(index, v);
            }
        }

        public Variable GetVariable(int index)
        {
            if (index < 0 || m_tuple == null || m_tuple.Count <= index)
            {
                return Variable.EmptyInstance;
            }
            return m_tuple[index];
        }

        public int FindIndex(string val)
        {
            if (this.Type != VarType.ARRAY)
            {
                return -1;
            }
            int result = m_tuple.FindIndex(item => item.AsString() == val);
            return result;
        }

        public virtual bool AsBool()
        {
            if (Type == VarType.NUMBER && Value != 0.0)
            {
                return true;
            }
            if (Type == VarType.STRING)
            {
                if (String.Compare(m_string, "true", true) == 0)
                    return true;
            }

            return false;
        }

        public virtual int AsInt()
        {
            int result = 0;
            if (Type == VarType.NUMBER || Value != 0.0)
            {
                return (int)Value;
            }
            if (Type == VarType.STRING)
            {
                Int32.TryParse(String, out result);
            }

            return result;
        }
        public virtual float AsFloat()
        {
            float result = 0;
            if (Type == VarType.NUMBER || Value != 0.0)
            {
                return (float)Value;
            }
            if (Type == VarType.STRING)
            {
                float.TryParse(String, out result);
            }

            return result;
        }
        public virtual long AsLong()
        {
            long result = 0;
            if (Type == VarType.NUMBER || Value != 0.0)
            {
                return (long)Value;
            }
            if (Type == VarType.STRING)
            {
                long.TryParse(String, out result);
            }
            return result;
        }
        public virtual double AsDouble()
        {
            double result = 0.0;
            if (Type == VarType.NUMBER)
            {// || (Value != 0.0 && Value != Double.NaN)) {
                return Value;
            }
            if (Type == VarType.STRING)
            {
                Double.TryParse(String, out result);
            }

            return result;
        }
        public override string ToString()
        {
            return AsString();
        }

        public virtual string AsString(bool isList = true,
                                       bool sameLine = true,
                                       int maxCount = -1)
        {
            var result = BaseAsString();
            if (result != null)
            {
                return result;
            }
            StringBuilder sb = new StringBuilder();
            if (isList)
            {
                sb.Append(Constants.START_ARRAY.ToString() +
                         (sameLine ? "" : Environment.NewLine));
            }

            int count = maxCount < 0 ? m_tuple.Count : Math.Min(maxCount, m_tuple.Count);
            int i = 0;
            HashSet<int> arrayKeys = new HashSet<int>();

                for (; i < count; i++)
                {
                    Variable arg = m_tuple[i];
                    var quote = arg.Type == VarType.STRING ? "\"" : "";
                    sb.Append(quote + arg.AsString(isList, sameLine, maxCount) + quote);
                    if (i != count - 1)
                    {
                        sb.Append(sameLine ? ", " : Environment.NewLine);
                    }
                }
            if (count < m_tuple.Count)
            {
                for (int j = 0; j < m_tuple.Count; j++)
                {
                    if (arrayKeys.Contains(j))
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append(sameLine ? ", " : Environment.NewLine);
                    }
                    Variable arg = m_tuple[j];
                    var quote = arg.Type == VarType.STRING ? "\"" : "";
                    sb.Append(quote + arg.AsString(isList, sameLine, maxCount) + quote);
                }
                //sb.Append(" ...");
            }
            if (isList)
            {
                sb.Append(Constants.END_ARRAY.ToString() +
                         (sameLine ? "" : Environment.NewLine));
            }

            return sb.ToString();
        }

        public string BaseAsString()
        {
            if (Type == VarType.NUMBER)
            {
                return Value.ToString();
            }
            if (Type == VarType.STRING)
            {
                return m_string == null ? "" : m_string;
            }
            if (Type == VarType.UNDEFINED)
            {
                return Constants.UNDEFINED;
            }
            if (Type == VarType.NONE || m_tuple == null)
            {
                return string.Empty;
            }

            return null;
        }

        public void SetAsArray()
        {
            Type = VarType.ARRAY;
            if (m_tuple == null)
            {
                m_tuple = new List<Variable>();
            }
        }

        public int Count
        {
            get
            {
                return Type == VarType.ARRAY ? m_tuple.Count :
                       Type == VarType.NONE ? 0 : 1;
            }
        }

        bool ProcessForEach(ParsingScript script)
        {
            var token = Utils.GetNextToken(script, true);
            Utils.CheckNotEmpty(token, Constants.FOREACH);

            CustomFunction customFunc = Utils.GetFunction(script, "", token);
            script.MoveForwardIf(Constants.END_ARG);

            if (customFunc == null)
            {
                customFunc = script.InterpreterInstance.GetFunction(token) as CustomFunction;
            }
            if (customFunc == null)
            {
                Utils.ThrowErrorMsg("No function found for [" + Constants.FOREACH + "].",
                                    script, token);
            }
            if (Tuple == null)
            {
                Utils.ThrowErrorMsg("No array found for [" + Constants.FOREACH + "].",
                                    script, token);
            }

            var args = script.InterpreterInstance.VariablesSnaphot(script);
            string propArg = customFunc.RealArgs[0];
            List<Variable> funcArgs = new List<Variable>();

            int index = 0;
            foreach (var item in Tuple)
            {
                funcArgs.Clear();
                funcArgs.Add(item);
                funcArgs.Add(new Variable(index++));
                funcArgs.Add(this);
                customFunc.Run(funcArgs, script);
            }
            return true;
        }

        public int GetSize()
        {
            int size = Type == Variable.VarType.ARRAY ? Tuple.Count : 0;
            return size;
        }

        public int GetLength()
        {
            int len = Type == Variable.VarType.ARRAY ?
                  Tuple.Count : AsString().Length;
            return len;
        }

        public Variable GetValue(int index)
        {
            if (index >= Count)
            {
                throw new ArgumentException("There are only [" + Count +
                                             "] but " + index + " requested.");

            }
            if (Type == VarType.ARRAY)
            {
                return m_tuple[index];
            }
            return this;
        }

        public virtual double Value
        {
            get { return m_value; }
            set { m_value = value; Type = VarType.NUMBER; }
        }

        public virtual string String
        {
            get { return m_string; }
            set { m_string = value; Type = VarType.STRING; }
        }

        public List<Variable> Tuple
        {
            get { return m_tuple; }
            set { m_tuple = value; Type = VarType.ARRAY; }
        }

        public string Action { get; set; }
        public VarType Type { get; set; }
        public OriginalType Original { get; set; }
        public bool IsReturn { get; set; }
        public string ParsingToken { get; set; }
        public int Index { get; set; }
        public string CurrentAssign { get; set; } = "";
        public string ParamName { get; set; } = "";

        public bool Writable { get; set; } = true;
        public bool Enumerable { get; set; } = true;
        public bool Configurable { get; set; } = true;

        public List<Variable> StackVariables { get; set; }

        public static Variable EmptyInstance = new Variable();
        public static Variable Undefined = new Variable(VarType.UNDEFINED);

        public virtual Variable Default()
        {
            return EmptyInstance;
        }

        protected double m_value;
        protected string m_string;
        protected List<Variable> m_tuple;

        public Variable DeepClone()
        {
            return new Variable
            {
                m_value = m_value,
                m_string = m_string,
                m_tuple = m_tuple,
                Action = Action,
                IsReturn = IsReturn,
                Type = VarType.NONE,
            };
        }
    }

    // A Variable supporting "dot-notation" must have an object implementing this interface.
}
