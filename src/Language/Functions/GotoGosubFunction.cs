using System.Collections.Generic;

namespace SplitAndMerge
{
    class GotoGosubFunction : ParserFunction
    {
        bool m_isGoto = true;

        public GotoGosubFunction(bool gotoMode = true)
        {
            m_isGoto = gotoMode;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            var labelName = Utils.GetToken(script, Constants.TOKEN_SEPARATION);

            Dictionary<string, int> labels;
            if (script.AllLabels == null || script.LabelToFile == null |
                !script.AllLabels.TryGetValue(script.FunctionName, out labels))
            {
                Utils.ThrowErrorMsg("Couldn't find labels in function [" + script.FunctionName + "].",
                    script, m_name);
                return Variable.EmptyInstance;
            }

            int gotoPointer;
            if (!labels.TryGetValue(labelName, out gotoPointer))
            {
                Utils.ThrowErrorMsg("Couldn't find label [" + labelName + "].",
                    script, m_name);
                return Variable.EmptyInstance;
            }

            if (!m_isGoto)
            {
                script.PointersBack.Add(script.Pointer);
            }

            script.Pointer = gotoPointer;
            if (string.IsNullOrWhiteSpace(script.FunctionName))
            {
                script.Backward();
            }

            return Variable.EmptyInstance;
        }
    }
}
