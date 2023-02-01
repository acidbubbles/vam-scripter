using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Program
{
    private static readonly string _ext = "cscs";

    private enum NextCmd
    {
        None = 0,
        Prev = -1,
        Next = 1,
        Tab = 2
    };


    private static void Main(string[] args)
    {
        string script = null;
        script = GetFileContents("scripts/bug.cscs");
        if (args.Length > 0)
        {
            if (args[0].EndsWith(_ext))
            {
                var filename = args[0];
                Console.WriteLine("Reading script from " + filename);
                script = GetFileContents(filename);
            }
            else
            {
                script = args[0];
            }
        }

        // Subscribe to the printing events from the interpreter.
        // A printing event will be triggered after each successful statement
        // execution. On error an exception will be thrown.
        Interpreter.Instance.GetOutput += Print;

        if (!string.IsNullOrWhiteSpace(script))
        {
            ProcessScript(script);
            return;
        }

        RunLoop();
    }

    private static void SplitByLast(string str, string sep, ref string a, ref string b)
    {
        var it = str.LastIndexOfAny(sep.ToCharArray());
        a = it == -1 ? "" : str.Substring(0, it + 1);
        b = it == -1 ? str : str.Substring(it + 1);
    }

    private static string CompleteTab(string script, string init, ref int tabFileIndex,
        ref string start, ref string baseStr, ref string startsWith)
    {
        if (tabFileIndex > 0 && !script.Equals(init))
        {
            // The user has changed something in the input field
            tabFileIndex = 0;
        }

        if (tabFileIndex == 0 || script.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            // The user pressed tab first time or pressed it on a directory
            var path = "";
            SplitByLast(script, " ", ref start, ref path);
            SplitByLast(path, "/\\", ref baseStr, ref startsWith);
        }

        tabFileIndex++;
        var result = Utils.GetFileEntry(baseStr, tabFileIndex, startsWith);
        result = result.Length == 0 ? startsWith : result;
        return start + baseStr + result;
    }

    private static void RunLoop()
    {
        var commands = new List<string>();
        var sb = new StringBuilder();
        var cmdPtr = 0;
        var tabFileIndex = 0;
        var arrowMode = false;
        string start = "", baseCmd = "", startsWith = "", init = "", script;

        while (true)
        {
            sb.Clear();

            var nextCmd = NextCmd.None;
            script = GetConsoleLine(ref nextCmd, init);

            if (nextCmd == NextCmd.Prev || nextCmd == NextCmd.Next)
            {
                if (arrowMode || nextCmd == NextCmd.Next)
                {
                    cmdPtr += (int)nextCmd;
                }

                cmdPtr = cmdPtr < 0 ? cmdPtr + commands.Count : cmdPtr % commands.Count;
                init = commands.Count == 0 ? script : commands[cmdPtr];
                arrowMode = true;
                continue;
            }
            else if (nextCmd == NextCmd.Tab)
            {
                init = CompleteTab(script, init, ref tabFileIndex,
                    ref start, ref baseCmd, ref startsWith);
                continue;
            }

            init = "";
            tabFileIndex = 0;
            arrowMode = false;

            if (string.IsNullOrWhiteSpace(script))
            {
                continue;
            }
            else if (script.StartsWith("quit") || script.StartsWith("exit"))
            {
                break;
            }

            if (commands.Count == 0 || !commands[commands.Count - 1].Equals(script))
            {
                commands.Add(script);
            }

            if (!script.EndsWith(Constants.EndStatement.ToString()))
            {
                script += Constants.EndStatement;
            }

            ProcessScript(script);
            cmdPtr = commands.Count - 1;
        }
    }

    private static string GetConsoleLine(ref NextCmd cmd, string init = "",
        bool enhancedMode = true)
    {
        //string line = init;
        var sb = new StringBuilder(init);
        var delta = init.Length - 1;
        if (!enhancedMode)
        {
            return Console.ReadLine();
        }

        var prompt = GetPrompt();
        Console.Write(prompt);
        Console.Write(init);

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow)
            {
                cmd = NextCmd.Prev;
                ClearLine(prompt, sb.ToString());
                return sb.ToString();
            }

            if (key.Key == ConsoleKey.DownArrow)
            {
                cmd = NextCmd.Next;
                ClearLine(prompt, sb.ToString());
                return sb.ToString();
            }

            if (key.Key == ConsoleKey.RightArrow)
            {
                delta = Math.Max(-1, Math.Min(++delta, sb.Length - 1));
                SetCursor(prompt, sb.ToString(), delta + 1);
                continue;
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                delta = Math.Max(-1, Math.Min(--delta, sb.Length - 1));
                SetCursor(prompt, sb.ToString(), delta + 1);
                continue;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                cmd = NextCmd.Tab;
                ClearLine(prompt, sb.ToString());
                return sb.ToString();
            }

            if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete)
            {
                if (sb.Length > 0)
                {
                    delta = key.Key == ConsoleKey.Backspace ? Math.Max(-1, Math.Min(--delta, sb.Length - 2)) : delta;
                    if (delta < sb.Length - 1)
                    {
                        sb.Remove(delta + 1, 1);
                    }

                    SetCursor(prompt, sb.ToString(), Math.Max(0, delta + 1));
                }

                continue;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return sb.ToString();
            }

            if (key.KeyChar == Constants.Empty)
            {
                continue;
            }

            ++delta;
            Console.Write(key.KeyChar);
            if (delta < sb.Length)
            {
                delta = Math.Max(0, Math.Min(delta, sb.Length - 1));
                sb.Insert(delta, key.KeyChar.ToString());
            }
            else
            {
                sb.Append(key.KeyChar);
            }

            SetCursor(prompt, sb.ToString(), delta + 1);
        }
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

        if (!_sPrintingCompleted)
        {
            var output = Interpreter.Instance.Output;
            if (!string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine(output);
            }
            else if (result != null)
            {
                output = result.AsString(false);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    Console.WriteLine(output);
                }
            }
        }

        _sPrintingCompleted = false;

        if (!string.IsNullOrWhiteSpace(errorMsg))
        {
            Console.WriteLine(errorMsg);
            errorMsg = string.Empty;
        }
    }

    private static string GetPrompt()
    {
        const int maxSize = 30;
        var path = Directory.GetCurrentDirectory();
        if (path.Length > maxSize)
        {
            path = "..." + path.Substring(path.Length - maxSize);
        }

        return string.Format("{0}>>", path);
    }

    private static void ClearLine(string part1, string part2 = "")
    {
        var spaces = new string(' ', part1.Length + part2.Length + 1);
        Console.Write("\r{0}\r", spaces);
    }

    private static void SetCursor(string prompt, string line, int pos)
    {
        ClearLine(prompt, line);
        Console.Write("{0}{1}\r{2}{3}",
            prompt, line, prompt, line.Substring(0, pos));
    }

    private static string GetFileContents(string filename)
    {
        var readText = File.ReadAllLines(filename);
        var text = string.Join("\n", readText);

        return text;
    }

    private static void Print(object sender, OutputAvailableEventArgs e)
    {
        Console.Write(e.Output);
        _sPrintingCompleted = true;
    }

    private static bool _sPrintingCompleted = false;
}
