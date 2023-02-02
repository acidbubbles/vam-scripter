using System.Collections.Generic;

namespace SplitAndMerge
{
    public partial class Utils
    {
        public static List<Variable> ConvertToResults(string[] items, Interpreter interpreter = null)
        {
            List<Variable> results = new List<Variable>(items.Length);
            foreach (string item in items)
            {
                results.Add(new Variable(item));
            }

            return results;
        }
    }
}
