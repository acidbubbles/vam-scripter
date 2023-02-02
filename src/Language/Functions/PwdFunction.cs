using System.IO;

namespace SplitAndMerge
{
    class PwdFunction : ParserFunction, IStringFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            string path = Directory.GetCurrentDirectory();
            return new Variable(path);
        }
    }
}
