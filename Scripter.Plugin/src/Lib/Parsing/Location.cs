namespace ScripterLang
{
    public struct Location
    {
        public static readonly Location Empty = new Location { line = -1 };

        public int line;

        public override string ToString()
        {
            return $"line {line}";
        }
    }
}
