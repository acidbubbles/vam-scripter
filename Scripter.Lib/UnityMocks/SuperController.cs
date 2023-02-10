using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
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
