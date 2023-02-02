using System;

namespace SplitAndMerge
{
    class ReadConsole : ParserFunction
    {
        internal ReadConsole(bool isNumber = false)
        {
            m_isNumber = isNumber;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            script.Forward(); // Skip opening parenthesis.
            string line = Console.ReadLine();

            if (!m_isNumber)
            {
                return new Variable(line);
            }

            double number = Double.NaN;
            if (!Double.TryParse(line, out number))
            {
                throw new ArgumentException("Couldn't parse number [" + line + "]");
            }

            return new Variable(number);
        }

        private bool m_isNumber;
    }
}
