public class ReturnStatement : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        Utils.MoveForwardIf(data, ref from, Constants.Space);

        var result = Utils.GetItem(data, ref from);

        // If we are in Return, we are done:
        from = data.Length;

        return result;
    }
}
