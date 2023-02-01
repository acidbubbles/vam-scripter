public class IdentityFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        return Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
    }
}