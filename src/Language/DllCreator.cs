namespace SplitAndMerge
{
#if __ANDROID__ == false && __IOS__ == false
    public class DLLCreator : ParserFunction
    {
        bool m_scriptInCSharp = false;

        public DLLCreator(bool scriptInCSharp)
        {
            m_scriptInCSharp = scriptInCSharp;
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            var precompiler = Precompiler.ImplementCustomDLL(script, m_scriptInCSharp, true);
            return new Variable(precompiler.OutputDLL);
        }
    }

#endif
}
