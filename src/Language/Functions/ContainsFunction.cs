using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
    class ContainsFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(script, Constants.NEXT_OR_END_ARRAY);
            Utils.CheckNotEnd(script, m_name);

            // 2. Get the current value of the variable.
            List<Variable> arrayIndices = Utils.GetArrayIndices(script, varName, (newVarName) => { varName = newVarName; } );

            ParserFunction func = InterpreterInstance.GetVariable(varName, script);
            Utils.CheckNotNull(varName, func, script);
            Variable currentValue = func.GetValue(script);

            // 2b. Special dealings with arrays:
            Variable query = arrayIndices.Count > 0 ?
                Utils.ExtractArrayElement(currentValue, arrayIndices, script) :
                currentValue;

            // 3. Get the value to be looked for.
            Variable searchValue = Utils.GetItem(script);
            Utils.CheckNotEnd(script, m_name);

            // 4. Check if the value to search for exists.
            bool exists = Exists(query, searchValue, true /* notEmpty */);

            script.MoveBackIf(Constants.START_GROUP);
            return new Variable(exists);
        }

        public bool Exists(Variable query, Variable indexVar, bool notEmpty = false)
        {
            if (query.Type != Variable.VarType.ARRAY)
            {
                return false;
            }
            if (indexVar.Type == Variable.VarType.NUMBER)
            {
                if (indexVar.Value < 0 ||
                    indexVar.Value >= query.Tuple.Count ||
                    indexVar.Value - Math.Floor(indexVar.Value) != 0.0)
                {
                    return false;
                }
                if (notEmpty)
                {
                    return query.Tuple[(int)indexVar.Value].Type != Variable.VarType.NONE;
                }
                return true;
            }

            throw new NotSupportedException("Index must be a number");
        }
    }
}
