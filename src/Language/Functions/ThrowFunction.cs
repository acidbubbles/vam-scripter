using System;

namespace SplitAndMerge
{
    class ThrowFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // 1. Extract what to throw.
            Variable arg = Utils.GetItem(script);

            // 2. Convert it to a string.
            string result = arg.AsString();

            // 3. Throw it!
            throw new ArgumentException(result);
        }

        public override string Description()
        {
            return "Throws an exception, e.g. throw \"value must be positive\".";
        }
    }
}
