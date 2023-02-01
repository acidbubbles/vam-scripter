using System;

public class Scripter : MVRScript
{
    private JSONStorableString _scriptJSON = new JSONStorableString("Script", "");

    public override void Init()
    {
        _scriptJSON.valNoCallback = @"
x = 0;
x++;
return x;
";

        CreateTextField(_scriptJSON);
        ProcessScript(_scriptJSON.val);
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
        //
        var output = Interpreter.Instance.Output;
        if (!string.IsNullOrEmpty(output))
        {
            SuperController.LogMessage(output);
        }
        else if (result != null)
        {
            output = result.AsString(false);
            if (!string.IsNullOrEmpty(output))
            {
                SuperController.LogMessage(output);
            }
        }
        //
        if (!string.IsNullOrEmpty(errorMsg))
        {
            SuperController.LogMessage(errorMsg);
        }
    }
}
