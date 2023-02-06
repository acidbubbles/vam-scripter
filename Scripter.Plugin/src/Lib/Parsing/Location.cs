namespace ScripterLang
{
    public struct Location
    {
        public static readonly Location Empty = new Location { Line = -1 };

        public int Line;

        public override string ToString()
        {
            return $"line {Line}";
        }
    }
}
