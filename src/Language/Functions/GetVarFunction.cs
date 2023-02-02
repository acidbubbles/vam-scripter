using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
    public class GetVarFunction : ParserFunction
    {
        public GetVarFunction(Variable value)
        {
            m_value = value;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            if (script == null)
            {
                return m_value;
            }
            // First check if this element is part of an array:
            if (script.TryPrev() == Constants.START_ARRAY)
            {
                // There is an index given - it must be for an element of the tuple.
                if (m_value.Tuple == null || m_value.Tuple.Count == 0)
                {
                    throw new ArgumentException("No tuple exists for the index");
                }

                if (m_arrayIndices == null)
                {
                    string startName = script.Substr(script.Pointer - 1);
                    m_arrayIndices = Utils.GetArrayIndices(script, startName, m_delta, (newStart, newDelta) => { startName = newStart; m_delta = newDelta; } );
                }

                script.Forward(m_delta);
                while (script.MoveForwardIf(Constants.END_ARRAY))
                {
                }

                Variable result = Utils.ExtractArrayElement(m_value, m_arrayIndices, script);
                if (script.Prev == '.')
                {
                    script.Backward();
                }

                if (script.TryCurrent() != '.')
                {
                    return result;
                }
                script.Forward();

                m_propName = Utils.GetToken(script, Constants.TOKEN_SEPARATION);
                Variable propValue = result.GetProperty(m_propName, script);
                Utils.CheckNotNull(propValue, m_propName, script);
                return propValue;
            }

            // Now check that this is an object:
            if (!string.IsNullOrWhiteSpace(m_propName))
            {
                string temp = m_propName;
                m_propName = null; // Need this to reset for recursive calls
                Variable propValue = m_value.Type == Variable.VarType.ENUM ?
                    m_value.GetEnumProperty(temp, script) :
                    m_value.GetProperty(temp, script);
                Utils.CheckNotNull(propValue, temp, script);
                return EvaluateFunction(propValue, script, m_propName);
            }

            // Otherwise just return the stored value.
            return m_value;
        }


        public static Variable EvaluateFunction(Variable var, ParsingScript script, string m_propName)
        {
            if (var.CustomFunctionGet != null)
            {
                List<Variable> args = script.Prev == '(' ? script.GetFunctionArgs() : new List<Variable>();
                if (var.StackVariables != null)
                {
                    args.AddRange(var.StackVariables);
                }
                return var.CustomFunctionGet.Run(args, script);
            }
            if (!string.IsNullOrWhiteSpace(var.CustomGet))
            {
                return ParsingScript.RunString(script.InterpreterInstance, var.CustomGet);
            }
            return var;
        }

        public int Delta
        {
            set { m_delta = value; }
        }
        public Variable Value
        {
            get { return m_value; }
        }
        public List<Variable> Indices
        {
            set { m_arrayIndices = value; }
        }
        public string PropertyName
        {
            set { m_propName = value; }
        }

        Variable m_value;
        int m_delta = 0;
        List<Variable> m_arrayIndices = null;
        string m_propName;
    }
}
