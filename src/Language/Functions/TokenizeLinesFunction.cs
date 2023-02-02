using System.Collections.Generic;

namespace SplitAndMerge
{
    class TokenizeLinesFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            string varName = Utils.GetSafeString(args, 0);
            Variable lines = Utils.GetSafeVariable(args, 1);
            int fromLine = Utils.GetSafeInt(args, 2, 0);
            string sepStr = Utils.GetSafeString(args, 3, "\t");
            if (sepStr == "\\t")
            {
                sepStr = "\t";
            }
            char[] sep = sepStr.ToCharArray();

            // var function = ParserFunction.GetVariable(varName, script);
            Variable allTokensVar = new Variable(Variable.VarType.ARRAY);

            for (int counter = fromLine; counter < lines.Tuple.Count; counter++)
            {
                Variable lineVar = lines.Tuple[counter];
#pragma warning disable 219
                // bugbug - the toAdd has side effects
                Variable toAdd = new Variable(counter - fromLine);
#pragma warning restore 219
                string line = lineVar.AsString();
                var tokens = line.Split(sep);
                Variable tokensVar = new Variable(Variable.VarType.ARRAY);
                foreach (string token in tokens)
                {
                    tokensVar.Tuple.Add(new Variable(token));
                }
                allTokensVar.Tuple.Add(tokensVar);
            }

            InterpreterInstance.AddGlobalOrLocalVariable(varName,
                new GetVarFunction(allTokensVar), script);

            return Variable.EmptyInstance;
        }
    }
}
