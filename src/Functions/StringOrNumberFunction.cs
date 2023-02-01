using System;

public class StringOrNumberFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // First check if the passed expression is a string between quotes.
        if (Item.Length > 1 &&
            Item[0] == Constants.Quote &&
            Item[Item.Length - 1] == Constants.Quote)
        {
            return Variable.CreateString(Item.Substring(1, Item.Length - 2));
        }


        // Otherwise this should be a number.
        double num;
        if (!double.TryParse(Item, out num))
        {
            throw new ArgumentException($"Couldn't parse token [{Item}]");
        }

        return Variable.CreateNumber(num);
    }

    public string Item { private get; set; }
}