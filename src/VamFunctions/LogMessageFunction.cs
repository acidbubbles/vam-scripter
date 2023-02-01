public class LogMessageFunction : ParserFunction
{
    public const string FunctionName = "logMessage";

    protected override Variable Evaluate(string data, ref int from)
    {
        var arg = Utils.GetItem(data, ref from);
        var result = arg.AsString();
        SuperController.LogMessage(result);
        return arg;
    }
}
