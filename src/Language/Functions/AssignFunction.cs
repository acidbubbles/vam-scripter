using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
    class AssignFunction : ActionFunction
    {
        public AssignFunction()
        {

        }

        public AssignFunction(Interpreter interpreter)
        {
            InterpreterInstance = interpreter;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            return Assign(script, m_name);
        }

        public Variable Assign(ParsingScript script, string varName, bool localIfPossible = false)
        {
            m_name = Constants.GetRealName(varName);
            script.CurrentAssign = m_name;
            Variable varValue = Utils.GetItem(script);

            script.MoveBackIfPrevious(Constants.END_ARG);

            if (script.Current == ' ' || script.Prev == ' ')
            {
                Utils.ThrowErrorMsg("Can't process expression [" + script.Rest + "].",
                    script, m_name);
            }

            // First try processing as an object (with a dot notation):
            Variable result = ProcessObject(script, varValue);
            if (result != null)
            {
                InterpreterInstance.AddGlobalOrLocalVariable(m_name, new GetVarFunction(result), script, localIfPossible);
                return result;
            }

            // Check if the variable to be set has the form of x[a][b]...,
            // meaning that this is an array element.
            List<Variable> arrayIndices = Utils.GetArrayIndices(script, m_name, (string name) => { m_name = name; });

            if (arrayIndices.Count == 0)
            {
                InterpreterInstance.AddGlobalOrLocalVariable(m_name, new GetVarFunction(varValue), script, localIfPossible);
                Variable retVar = varValue.DeepClone();
                retVar.CurrentAssign = m_name;
                return retVar;
            }

            Variable array;

            ParserFunction pf = InterpreterInstance.GetVariable(m_name, script);
            array = pf != null ? (pf.GetValue(script)) : new Variable();

            ExtendArray(array, arrayIndices, 0, varValue);

            InterpreterInstance.AddGlobalOrLocalVariable(m_name, new GetVarFunction(array), script, localIfPossible);
            return array;
        }

        Variable ProcessObject(ParsingScript script, Variable varValue)
        {
            string varName = m_name;

            int ind = varName.IndexOf('.');
            if (ind <= 0)
            {
                return null;
            }

            Utils.CheckForValidName(varName, script);

            string name = varName.Substring(0, ind);
            string prop = varName.Substring(ind + 1);

            if (InterpreterInstance.TryAddToNamespace(prop, name, varValue))
            {
                return varValue.DeepClone();
            }

            ParserFunction existing = InterpreterInstance.GetVariable(name, script);
            Variable baseValue = existing != null ? existing.GetValue(script) : new Variable(Variable.VarType.ARRAY);

            InterpreterInstance.AddGlobalOrLocalVariable(name, new GetVarFunction(baseValue), script);
            //ParserFunction.AddGlobal(name, new GetVarFunction(baseValue), false);

            return varValue.DeepClone();
        }

        override public ParserFunction NewInstance()
        {
            return new AssignFunction(InterpreterInstance);
        }

        public static void ExtendArray(Variable parent,
            List<Variable> arrayIndices,
            int indexPtr,
            Variable varValue)
        {
            if (arrayIndices.Count <= indexPtr)
            {
                return;
            }

            Variable index = arrayIndices[indexPtr];
            int currIndex = ExtendArrayHelper(parent, index);

            if (arrayIndices.Count - 1 == indexPtr)
            {
                parent.Tuple[currIndex] = varValue;
                return;
            }

            Variable son = parent.Tuple[currIndex];
            ExtendArray(son, arrayIndices, indexPtr + 1, varValue);
        }

        private static int ExtendArrayHelper(Variable parent, Variable indexVar)
        {
            parent.SetAsArray();

            int arrayIndex = parent.GetArrayIndex(indexVar);
            if (arrayIndex < 0)
            {
                // This is not a "normal index" but a new string for the dictionary.
                throw new NotSupportedException("Index cannot be a string");
            }

            if (parent.Tuple.Count <= arrayIndex)
            {
                for (int i = parent.Tuple.Count; i <= arrayIndex; i++)
                {
                    parent.Tuple.Add(Variable.NewEmpty());
                }
            }
            return arrayIndex;
        }
    }
}
