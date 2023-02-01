using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Returns process info
public class PsInfoFunction : ParserFunction
{
    internal PsInfoFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var pattern = Utils.GetItem(data, ref from).String;
        if (string.IsNullOrWhiteSpace(pattern))
        {
            throw new ArgumentException("Couldn't extract process name");
        }

        var maxProcName = 26;
        _mInterpreter.AppendOutput(Utils.GetLine());
        _mInterpreter.AppendOutput(String.Format("{0} {1} {2} {3} {4}",
            "Process Id".PadRight(15), "Process Name".PadRight(maxProcName),
            "Working Set".PadRight(15), "Virt Mem".PadRight(15), "Start Time".PadRight(15), "CPU Time".PadRight(25)));

        var processes = Process.GetProcessesByName(pattern);
        var results = new List<Variable>(processes.Length);
        for (var i = 0; i < processes.Length; i++)
        {
            var pr = processes[i];
            var workingSet = (int)(pr.WorkingSet64 / 1000000.0);
            var virtMemory = (int)(pr.VirtualMemorySize64 / 1000000.0);
            var procTitle = pr.ProcessName + " " + pr.MainWindowTitle.Split(null)[0];
            var startTime = pr.StartTime.ToString(CultureInfo.InvariantCulture);
            if (procTitle.Length > maxProcName)
            {
                procTitle = procTitle.Substring(0, maxProcName);
            }

            var procTime = string.Empty;
            try
            {
                procTime = pr.TotalProcessorTime.ToString().Substring(0, 11);
            }
            catch (Exception)
            {
            }

            results.Add(new Variable(
                string.Format("{0,15} {1," + maxProcName + "} {2,15} {3,15} {4,15} {5,25}",
                    pr.Id, procTitle,
                    workingSet, virtMemory, startTime, procTime)));
            _mInterpreter.AppendOutput(results.Last().String);
        }

        _mInterpreter.AppendOutput(Utils.GetLine());

        if (data.Length > from && data[from] == Constants.NextArg)
        {
            from++; // eat end of statement semicolon
        }

        return new Variable(results);
    }

    private readonly Interpreter _mInterpreter;
}

// Kills a process with specified process id
public class KillFunction : ParserFunction
{
    internal KillFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var id = Utils.GetItem(data, ref from);
        Utils.CheckPosInt(id);

        var processId = (int)id.Value;
        try
        {
            var process = Process.GetProcessById(processId);
            process.Kill();
            _mInterpreter.AppendOutput("Process " + processId + " killed");
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't kill process " + processId +
                                        " (" + exc.Message + ")");
        }

        return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
}

// Starts running a new process, returning its ID
public class RunFunction : ParserFunction
{
    internal RunFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var processName = Utils.GetItem(data, ref from).String;

        if (string.IsNullOrWhiteSpace(processName))
        {
            throw new ArgumentException("Couldn't extract process name");
        }

        var args = Utils.GetFunctionArgs(data, ref from);
        var processId = -1;

        try
        {
            var pr = Process.Start(processName, string.Join("", args.ToArray()));
            processId = pr.Id;
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't start [" + processName + "]: " + exc.Message);
        }

        _mInterpreter.AppendOutput("Process " + processName + " started, id: " + processId);
        return new Variable(processId);
    }

    private readonly Interpreter _mInterpreter;
}

// Starts running an "echo" server
public class ServerSocket : ParserFunction
{
    internal ServerSocket(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var portRes = Utils.GetItem(data, ref from);
        Utils.CheckPosInt(portRes);
        var port = (int)portRes.Value;

        try
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var localEndPoint = new IPEndPoint(ipAddress, port);

            var listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            Socket handler = null;
            while (true)
            {
                _mInterpreter.AppendOutput("Waiting for connections on " + port + " ...");
                handler = listener.Accept();

                // Data buffer for incoming data.
                var bytes = new byte[1024];
                var bytesRec = handler.Receive(bytes);
                var received = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                _mInterpreter.AppendOutput("Received from " + handler.RemoteEndPoint.ToString() +
                                           ": [" + received + "]");

                var msg = Encoding.UTF8.GetBytes(received);
                handler.Send(msg);

                if (received.Contains("<EOF>"))
                {
                    break;
                }
            }

            if (handler != null)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't start server: (" + exc.Message + ")");
        }

        return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
}

// Starts running an "echo" client
public class ClientSocket : ParserFunction
{
    internal ClientSocket(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        // Data buffer for incoming data.
        var bytes = new byte[1024];

        var isList = false;
        var args = Utils.GetArgs(data, ref from,
            Constants.StartArg, Constants.EndArg, out isList);

        Utils.CheckArgs(args.Count, 3, Constants.Connectsrv);
        Utils.CheckPosInt(args[1]);

        var hostname = args[0].String;
        var port = (int)args[1].Value;
        var msgToSend = args[2].String;

        if (string.IsNullOrWhiteSpace(hostname) || hostname.Equals("localhost"))
        {
            hostname = Dns.GetHostName();
        }

        try
        {
            var ipHostInfo = Dns.GetHostEntry(hostname);
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEp = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP  socket.
            var sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(remoteEp);

            _mInterpreter.AppendOutput("Connected to [" + sender.RemoteEndPoint.ToString() + "]");

            var msg = Encoding.UTF8.GetBytes(msgToSend);
            sender.Send(msg);

            // Receive the response from the remote device.
            var bytesRec = sender.Receive(bytes);
            var received = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            _mInterpreter.AppendOutput("Received [" + received + "]");

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't connect to server: (" + exc.Message + ")");
        }

        return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
}

// Returns an environment variable
public class GetEnvFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var varName = Utils.GetToken(data, ref from, Constants.EndArgArray);
        var res = Environment.GetEnvironmentVariable(varName);

        return new Variable(res);
    }
}

// Sets an environment variable
public class SetEnvFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't set env variable");
        }

        var varValue = Utils.GetItem(data, ref from);
        var strValue = varValue.AsString();
        Environment.SetEnvironmentVariable(varName, strValue);

        return new Variable(varName);
    }
}

// Prints passed list of arguments
public class PrintFunction : ParserFunction
{
    internal PrintFunction(Interpreter interpreter, bool newLine = true)
    {
        _mInterpreter = interpreter;
        _mNewLine = newLine;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        bool isList;
        var args = Utils.GetArgs(data, ref from,
            Constants.StartArg, Constants.EndArg, out isList);

        var output = string.Empty;
        for (var i = 0; i < args.Count; i++)
        {
            output += args[i].AsString();
        }

        _mInterpreter.AppendOutput(output, _mNewLine);

        return Variable.EmptyInstance;
    }

    private readonly Interpreter _mInterpreter;
    private readonly bool _mNewLine;
}

// Reads either a string or a number from Console
public class ReadConsole : ParserFunction
{
    internal ReadConsole(bool isNumber = false)
    {
        _mIsNumber = isNumber;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        from++; // Skip opening parenthesis.
        var line = Console.ReadLine();

        var number = Double.NaN;
        if (_mIsNumber)
        {
            if (!Double.TryParse(line, out number))
            {
                throw new ArgumentException("Couldn't parse number [" + line + "]");
            }

            return new Variable(number);
        }

        return new Variable(line);
    }

    private readonly bool _mIsNumber;
}

// Returns how much processor time has been spent on the current process
public class ProcessorTimeFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var pr = Process.GetCurrentProcess();
        var ts = pr.TotalProcessorTime;

        return new Variable(ts.TotalMilliseconds);
    }
}

// Returns current directory name
public class PwdFunction : ParserFunction
{
    internal PwdFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var path = Directory.GetCurrentDirectory();
        _mInterpreter.AppendOutput(path);

        return new Variable(path);
    }

    private readonly Interpreter _mInterpreter;
}

// Reads a file and returns all lines of that file as a "tuple" (list)
public class ReadFileFunction : ParserFunction
{
    internal ReadFileFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var lines = Utils.GetFileLines(filename);

        var results = Utils.ConvertToResults(lines);
        _mInterpreter.AppendOutput("Read " + lines.Length + " line(s).");

        return new Variable(results);
    }

    private readonly Interpreter _mInterpreter;
}

// View the contents of a text file
public class MoreFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var size = Constants.DefaultFileLines;

        var sizeAvailable = Utils.SeparatorExists(data, from);
        if (sizeAvailable)
        {
            var length = Utils.GetItem(data, ref from);
            Utils.CheckPosInt(length);
            size = (int)length.Value;
        }

        var lines = Utils.GetFileLines(filename, 0, size);
        var results = Utils.ConvertToResults(lines);

        return new Variable(results);
    }
}

// View the last Constants.DEFAULT_FILE_LINES lines of a file
public class TailFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var size = Constants.DefaultFileLines;

        var sizeAvailable = Utils.SeparatorExists(data, from);
        if (sizeAvailable)
        {
            var length = Utils.GetItem(data, ref from);
            Utils.CheckPosInt(length);
            size = (int)length.Value;
        }

        var lines = Utils.GetFileLines(filename, -1, size);
        var results = Utils.ConvertToResults(lines);

        return new Variable(results);
    }
}

// Append a line to a file
public class AppendLineFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var line = Utils.GetItem(data, ref from);
        Utils.AppendFileText(filename, line.AsString() + Environment.NewLine);

        return Variable.EmptyInstance;
    }
}

// Apend a list of lines to a file
public class AppendLinesFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var lines = Utils.GetLinesFromList(data, ref from);
        Utils.AppendFileText(filename, lines);

        return Variable.EmptyInstance;
    }
}

// Write a line to a file
public class WriteLineFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var line = Utils.GetItem(data, ref from);
        Utils.WriteFileText(filename, line.AsString() + Environment.NewLine);

        return Variable.EmptyInstance;
    }
}

// Write a list of lines to a file
public class WriteLinesFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        //string filename = Utils.ResultToString(Utils.GetItem(data, ref from));
        var filename = Utils.GetStringOrVarValue(data, ref from);
        var lines = Utils.GetLinesFromList(data, ref from);
        Utils.WriteFileText(filename, lines);

        return Variable.EmptyInstance;
    }
}

// Find a string in files
public class FindstrFunction : ParserFunction
{
    internal FindstrFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var search = Utils.GetStringOrVarValue(data, ref from);
        var patterns = Utils.GetFunctionArgs(data, ref from);

        var ignoreCase = true;
        if (patterns.Count > 0 && patterns.Last().Equals("case"))
        {
            ignoreCase = false;
            patterns.RemoveAt(patterns.Count - 1);
        }

        if (patterns.Count == 0)
        {
            patterns.Add("*.*");
        }

        List<Variable> results = null;
        try
        {
            var pwd = Directory.GetCurrentDirectory();
            var files = Utils.GetStringInFiles(pwd, search, patterns.ToArray(), ignoreCase);

            results = Utils.ConvertToResults(files.ToArray(), _mInterpreter);
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't find pattern: " + exc.Message);
        }

        return new Variable(results);
    }

    private readonly Interpreter _mInterpreter;
}

// Find files having a given pattern
public class FindfilesFunction : ParserFunction
{
    internal FindfilesFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var patterns = Utils.GetFunctionArgs(data, ref from);
        if (patterns.Count == 0)
        {
            patterns.Add("*.*");
        }

        List<Variable> results = null;
        try
        {
            var pwd = Directory.GetCurrentDirectory();
            var files = Utils.GetFiles(pwd, patterns.ToArray());

            results = Utils.ConvertToResults(files.ToArray(), _mInterpreter);
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't list directory: " + exc.Message);
        }

        return new Variable(results);
    }

    private readonly Interpreter _mInterpreter;
}

// Copy a file or a directiry
public class CopyFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var source = Utils.GetStringOrVarValue(data, ref from);
        Utils.MoveForwardIf(data, ref from, Constants.NextArg, Constants.Space);
        var dest = Utils.GetStringOrVarValue(data, ref from);

        var src = Path.GetFullPath(source);
        var dst = Path.GetFullPath(dest);

        var isFile = File.Exists(src);
        var isDir = Directory.Exists(src);
        if (!isFile && !isDir)
        {
            throw new ArgumentException("[" + src + "] doesn't exist");
        }

        if (isFile && Directory.Exists(dst))
        {
            // If filename is missing in the destination file,
            // add it from the source.
            dst = Path.Combine(dst, Path.GetFileName(src));
        }

        try
        {
            if (isFile)
            {
                File.Copy(src, dst, true);
            }
            else
            {
                Utils.DirectoryCopy(src, dst);
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't copy: " + exc.Message);
        }

        return Variable.EmptyInstance;
    }
}

// Move a file or a directiry
public class MoveFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var source = Utils.GetStringOrVarValue(data, ref from);
        Utils.MoveForwardIf(data, ref from, Constants.NextArg, Constants.Space);
        var dest = Utils.GetStringOrVarValue(data, ref from);

        var src = Path.GetFullPath(source);
        var dst = Path.GetFullPath(dest);

        var isFile = File.Exists(src);
        var isDir = Directory.Exists(src);
        if (!isFile && !isDir)
        {
            throw new ArgumentException("[" + src + "] doesn't exist");
        }

        if (isFile && Directory.Exists(dst))
        {
            // If filename is missing in the destination file,
            // add it from the source.
            dst = Path.Combine(dst, Path.GetFileName(src));
        }

        try
        {
            if (isFile)
            {
                File.Move(src, dst);
            }
            else
            {
                Directory.Move(src, dst);
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't copy: " + exc.Message);
        }

        return Variable.EmptyInstance;
    }
}

// Make a directory
public class MkdirFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var dirname = Utils.GetStringOrVarValue(data, ref from);
        try
        {
            Directory.CreateDirectory(dirname);
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't create [" + dirname + "] :" + exc.Message);
        }

        return Variable.EmptyInstance;
    }
}

// Delete a file or a directory
public class DeleteFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var pathname = Utils.GetStringOrVarValue(data, ref from);

        var isFile = File.Exists(pathname);
        var isDir = Directory.Exists(pathname);
        if (!isFile && !isDir)
        {
            throw new ArgumentException("[" + pathname + "] doesn't exist");
        }

        try
        {
            if (isFile)
            {
                File.Delete(pathname);
            }
            else
            {
                Directory.Delete(pathname, true);
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't delete [" + pathname + "] :" + exc.Message);
        }

        return Variable.EmptyInstance;
    }
}

// Checks if a directory or a file exists
public class ExistsFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        var pathname = Utils.GetStringOrVarValue(data, ref from);

        var isFile = File.Exists(pathname);
        var isDir = Directory.Exists(pathname);
        if (!isFile && !isDir)
        {
            throw new ArgumentException("[" + pathname + "] doesn't exist");
        }

        var exists = false;
        try
        {
            if (isFile)
            {
                exists = File.Exists(pathname);
            }
            else
            {
                exists = Directory.Exists(pathname);
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Couldn't delete [" + pathname + "] :" + exc.Message);
        }

        return new Variable(Convert.ToDouble(exists));
    }
}

// List files in a directory
public class DirFunction : ParserFunction
{
    internal DirFunction(Interpreter interpreter)
    {
        _mInterpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        var dirname = (data.Length <= from || data[from] == Constants.EndStatement)
            ? Directory.GetCurrentDirectory()
            : Utils.GetToken(data, ref from, Constants.NextOrEndArray);

        var pattern = Constants.AllFiles;
        var results = new List<Variable>();

        try
        {
            // Special dealing if there is a pattern (only * is supported at the moment)
            var index = dirname.IndexOf('*');
            if (index >= 0)
            {
                pattern = Path.GetFileName(dirname);

                if (index > 0)
                {
                    var prefix = dirname.Substring(0, index);
                    var di = Directory.GetParent(prefix);
                    dirname = di.FullName;
                }
                else
                {
                    dirname = ".";
                }
            }

            var dir = Path.GetFullPath(dirname);

            // First get contents of the directory (unless there is a pattern)
            var dirInfo = new DirectoryInfo(dir);
            if (pattern == Constants.AllFiles)
            {
                _mInterpreter.AppendOutput(Utils.GetPathDetails(dirInfo, "."));
                _mInterpreter.AppendOutput(Utils.GetPathDetails(dirInfo.Parent, ".."));
            }

            // Then get contents of all of the files in the directory
            var fileNames = dirInfo.GetFiles(pattern);
            foreach (var fi in fileNames)
            {
                _mInterpreter.AppendOutput(Utils.GetPathDetails(fi, fi.Name));
                results.Add(new Variable(fi.Name));
            }

            // Then get contents of all of the subdirs in the directory
            var dirInfos = dirInfo.GetDirectories(pattern);
            foreach (var di in dirInfos)
            {
                _mInterpreter.AppendOutput(Utils.GetPathDetails(di, di.Name));
                results.Add(new Variable(di.Name));
            }
        }
        catch (Exception exc)
        {
            throw new ArgumentException("Could not list directory: " + exc.Message);
        }

        return new Variable(results);
    }

    private readonly Interpreter _mInterpreter;
}

// Append a string to another string
public class AppendFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't append variable");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Get the value to be added or appended.
        var newValue = Utils.GetItem(data, ref from);

        // 4. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var arg1 = currentValue.AsString();
        var arg2 = newValue.AsString();

        // 5. The variable becomes a string after adding a string to it.
        newValue.Reset();
        newValue.String = arg1 + arg2;

        AddGlobalOrLocalVariable(varName, new GetVarFunction(newValue));

        return newValue;
    }
}

// Convert a string to the upper case
public class ToUpperFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.EndArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't get variable");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var arg = currentValue.AsString();

        var newValue = new Variable(arg.ToUpper());
        return newValue;
    }
}

// Convert a string to the lower case
public class ToLowerFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.EndArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't get variable");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var arg = currentValue.AsString();

        var newValue = new Variable(arg.ToLower());
        return newValue;
    }
}

// Get a substring of a string
public class SubstrFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        string substring;

        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't get variable");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var arg = currentValue.AsString();
        // 4. Get the initial index of the substring.
        var init = Utils.GetItem(data, ref from);
        Utils.CheckNonNegativeInt(init);

        // 5. Get the length of the substring if available.
        var lengthAvailable = Utils.SeparatorExists(data, from);
        if (lengthAvailable)
        {
            var length = Utils.GetItem(data, ref from);
            Utils.CheckPosInt(length);
            if (init.Value + length.Value > arg.Length)
            {
                throw new ArgumentException("The total substring length is larger than [" +
                                            arg + "]");
            }

            substring = arg.Substring((int)init.Value, (int)length.Value);
        }
        else
        {
            substring = arg.Substring((int)init.Value);
        }

        var newValue = new Variable(substring);

        return newValue;
    }
}

// Get an index of a substring in a string. Return -1 if not found.
public class IndexOfFunction : ParserFunction
{
    protected override Variable Evaluate(string data, ref int from)
    {
        // 1. Get the name of the variable.
        var varName = Utils.GetToken(data, ref from, Constants.NextArgArray);
        if (from >= data.Length)
        {
            throw new ArgumentException("Couldn't extract variable name");
        }

        // 2. Get the current value of the variable.
        var func = GetFunction(varName);
        var currentValue = func.GetValue(data, ref from);

        // 3. Get the value to be looked for.
        var searchValue = Utils.GetItem(data, ref from);

        // 4. Take either the string part if it is defined,
        // or the numerical part converted to a string otherwise.
        var basePart = currentValue.AsString();
        var search = searchValue.AsString();

        var result = basePart.IndexOf(search);
        return new Variable(result);
    }
}
