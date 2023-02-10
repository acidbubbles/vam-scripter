using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SuperController
{
    public static readonly SuperController singleton = new SuperController();

    public static void LogMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void ClearMessages()
    {
    }
}
