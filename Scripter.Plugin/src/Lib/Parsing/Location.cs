namespace ScripterLang
{
    public struct Location
    {
        public int Line;

        public override string ToString()
        {
            return $"line {Line}";
        }
    }
}
