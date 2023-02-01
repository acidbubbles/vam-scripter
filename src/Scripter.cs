using System;

public class Scripter : MVRScript
{
    private JSONStorableString _scriptJSON = new JSONStorableString("Script", "");

    public override void Init()
    {
        _scriptJSON.valNoCallback = @"
x = 0;
x++;
";

        // ProcessScript(_scriptJSON.val);
    }

    private static void ProcessScript(string script)
    {
        string errorMsg = null;
        Variable result = null;
        try
        {
            result = Interpreter.Instance.Process(script);
        }
        catch (Exception exc)
        {
            errorMsg = exc.Message;
            ParserFunction.InvalidateStacksAfterLevel(0);
        }

        var output = Interpreter.Instance.Output;
        if (!string.IsNullOrEmpty(output))
        {
            Console.WriteLine(output);
        }
        else if (result != null)
        {
            output = result.AsString(false);
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            Console.WriteLine(errorMsg);
        }
    }
}
