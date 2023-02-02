namespace SplitAndMerge
{
    public abstract class ActionFunction : ParserFunction
    {
        protected string m_action;
        public string Action { set { m_action = value; } }
    }
}
