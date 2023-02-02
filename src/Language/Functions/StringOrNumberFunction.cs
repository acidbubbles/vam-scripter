namespace SplitAndMerge
{
    class StringOrNumberFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            // First check if the passed expression is a string between quotes.
            if (Item.Length > 1 &&
                ((Item[0] == Constants.QUOTE  && Item[Item.Length - 1] == Constants.QUOTE) ||
                 (Item[0] == Constants.QUOTE1 && Item[Item.Length - 1] == Constants.QUOTE1)))
            {
                return new Variable(Item.Substring(1, Item.Length - 2));
            }

            // Otherwise this should be a number.
            double num = Utils.ConvertToDouble(Item, script);
            return new Variable(num);
        }

        public string Item { private get; set; }
    }
}
