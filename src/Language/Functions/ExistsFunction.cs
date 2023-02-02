using System;
using System.IO;

namespace SplitAndMerge
{
    class ExistsFunction : ParserFunction, INumericFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            string pathname = Utils.GetItem(script).AsString();

            bool exists = false;
            try
            {
                exists = File.Exists(pathname);
                if (!exists)
                {
                    exists = Directory.Exists(pathname);
                }
            }
            catch (Exception)
            {
            }

            return new Variable(exists);
        }
    }
}
