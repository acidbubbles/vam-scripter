
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
  // Returns process info
  class PsInfoFunction : ParserFunction
  {
    internal PsInfoFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string pattern = Utils.GetItem(data, ref from).String;
      if (string.IsNullOrWhiteSpace(pattern))
      {
        throw new ArgumentException("Couldn't extract process name");
      }

      int MAX_PROC_NAME = 26;
      m_interpreter.AppendOutput(Utils.GetLine());
      m_interpreter.AppendOutput(String.Format("{0} {1} {2} {3} {4}",
        "Process Id".PadRight(15), "Process Name".PadRight(MAX_PROC_NAME),
        "Working Set".PadRight(15), "Virt Mem".PadRight(15), "Start Time".PadRight(15), "CPU Time".PadRight(25)));

      Process[] processes = Process.GetProcessesByName(pattern);
      List<Variable> results = new List<Variable>(processes.Length);
      for (int i = 0; i < processes.Length; i++)
      {
        Process pr = processes[i];
        int workingSet = (int)(((double)pr.WorkingSet64) / 1000000.0);
        int virtMemory = (int)(((double)pr.VirtualMemorySize64) / 1000000.0);
        string procTitle = pr.ProcessName + " " + pr.MainWindowTitle.Split(null)[0];
        string startTime = pr.StartTime.ToString();
        if (procTitle.Length > MAX_PROC_NAME)
        {
          procTitle = procTitle.Substring(0, MAX_PROC_NAME);
        }
        string procTime = string.Empty;
        try
        {
          procTime = pr.TotalProcessorTime.ToString().Substring(0, 11);
        }
        catch (Exception) { }

        results.Add(new Variable(
          string.Format("{0,15} {1," + MAX_PROC_NAME + "} {2,15} {3,15} {4,15} {5,25}",
            pr.Id, procTitle,
            workingSet, virtMemory, startTime, procTime)));
        m_interpreter.AppendOutput(results.Last().String);
      }
      m_interpreter.AppendOutput(Utils.GetLine());

      if (data.Length > from && data[from] == Constants.NEXT_ARG)
      {
        from++; // eat end of statement semicolon
      }

      return new Variable(results);
    }

    private Interpreter m_interpreter;
  }

  // Kills a process with specified process id
  class KillFunction : ParserFunction
  {
    internal KillFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      Variable id = Utils.GetItem(data, ref from);
      Utils.CheckPosInt(id);

      int processId = (int)id.Value;
      try
      {
        Process process = Process.GetProcessById(processId);
        process.Kill();
        m_interpreter.AppendOutput("Process " + processId + " killed");
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Couldn't kill process " + processId +
          " (" + exc.Message + ")");
      }

      return Variable.EmptyInstance;
    }

    private Interpreter m_interpreter;
  }

  // Starts running a new process, returning its ID
  class RunFunction : ParserFunction
  {
    internal RunFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string processName = Utils.GetItem(data, ref from).String;

      if (string.IsNullOrWhiteSpace(processName))
      {
        throw new ArgumentException("Couldn't extract process name");
      }

      List<string> args = Utils.GetFunctionArgs(data, ref from);
      int processId = -1;

      try
      {
        Process pr = Process.Start(processName, string.Join("", args.ToArray()));
        processId = pr.Id;
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Couldn't start [" + processName + "]: " + exc.Message);
      }

      m_interpreter.AppendOutput("Process " + processName + " started, id: " + processId);
      return new Variable(processId);
    }

    private Interpreter m_interpreter;
  }

  // Starts running an "echo" server
  class ServerSocket : ParserFunction
  {
    internal ServerSocket(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      Variable portRes = Utils.GetItem (data, ref from);
      Utils.CheckPosInt(portRes);
      int port = (int)portRes.Value;

      try {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList [0];
        IPEndPoint localEndPoint = new IPEndPoint (ipAddress, port);

        Socket listener = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);

        listener.Bind (localEndPoint);
        listener.Listen(10);

        Socket handler = null;
        while (true) {
          m_interpreter.AppendOutput("Waiting for connections on " + port + " ...");
          handler = listener.Accept ();

          // Data buffer for incoming data.
          byte[] bytes = new byte[1024];
          int bytesRec = handler.Receive(bytes);
          string received = Encoding.UTF8.GetString (bytes, 0, bytesRec);

          m_interpreter.AppendOutput("Received from " + handler.RemoteEndPoint.ToString() +
            ": [" + received + "]");

          byte[] msg = Encoding.UTF8.GetBytes(received);
          handler.Send(msg);

          if (received.Contains ("<EOF>")) {
            break;
          }
        }

        if (handler != null) {
          handler.Shutdown (SocketShutdown.Both);
          handler.Close ();
        }
      } catch (Exception exc) {
        throw new ArgumentException ("Couldn't start server: (" + exc.Message + ")");
      }

      return Variable.EmptyInstance;
    }

    private Interpreter m_interpreter;
  }

  // Starts running an "echo" client
  class ClientSocket : ParserFunction
  {
    internal ClientSocket(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      // Data buffer for incoming data.
      byte[] bytes = new byte[1024];

      bool isList = false;
      List<Variable> args = Utils.GetArgs(data, ref from,
        Constants.START_ARG, Constants.END_ARG, out isList);

      Utils.CheckArgs(args.Count, 3, Constants.CONNECTSRV);
      Utils.CheckPosInt(args [1]);

      string hostname = args[0].String;
      int port = (int)args[1].Value;
      string msgToSend = args[2].String;

      if (string.IsNullOrWhiteSpace(hostname) || hostname.Equals ("localhost")) {
        hostname = Dns.GetHostName ();
      }

      try {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

        // Create a TCP/IP  socket.
        Socket sender = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp );

        sender.Connect(remoteEP);

        m_interpreter.AppendOutput ("Connected to [" + sender.RemoteEndPoint.ToString() + "]");

        byte[] msg = Encoding.UTF8.GetBytes(msgToSend);
        sender.Send(msg);

        // Receive the response from the remote device.
        int bytesRec = sender.Receive(bytes);
        string received = Encoding.UTF8.GetString(bytes, 0, bytesRec);
        m_interpreter.AppendOutput ("Received [" + received + "]");

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();

      } catch (Exception exc) {
        throw new ArgumentException ("Couldn't connect to server: (" + exc.Message + ")");
      }

      return Variable.EmptyInstance;
    }

    private Interpreter m_interpreter;
  }

  // Returns an environment variable
  class GetEnvFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
      string res = Environment.GetEnvironmentVariable(varName);

      return new Variable(res);
    }
  }

  // Sets an environment variable
  class SetEnvFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't set env variable");
      }

      Variable varValue = Utils.GetItem(data, ref from);
      string strValue = varValue.AsString();
      Environment.SetEnvironmentVariable(varName, strValue);

      return new Variable(varName);
    }
  }

  // Prints passed list of arguments
  class PrintFunction : ParserFunction
  {
    internal PrintFunction(Interpreter interpreter, bool newLine = true)
    {
      m_interpreter = interpreter;
      m_newLine     = newLine;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      bool isList;
      List<Variable> args = Utils.GetArgs(data, ref from,
        Constants.START_ARG, Constants.END_ARG, out isList);

      string output = string.Empty;
      for (int i = 0; i < args.Count; i++) {
        output += args[i].AsString();
      }

      m_interpreter.AppendOutput(output, m_newLine);

      return Variable.EmptyInstance;
    }

    private Interpreter m_interpreter;
    private bool m_newLine;
  }

  // Reads either a string or a number from Console
  class ReadConsole : ParserFunction
  {
    internal ReadConsole(bool isNumber = false)
    {
      m_isNumber = isNumber;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      from++; // Skip opening parenthesis.
      string line = Console.ReadLine();

      double number = Double.NaN;
      if (m_isNumber) {
        if (!Double.TryParse(line, out number))
        {
          throw new ArgumentException("Couldn't parse number [" + line + "]");
        }
        return new Variable(number);
      }

      return new Variable(line);
    }

    private bool m_isNumber;
  }

  // Returns how much processor time has been spent on the current process
  class ProcessorTimeFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      Process pr = Process.GetCurrentProcess();
      TimeSpan ts = pr.TotalProcessorTime;

      return new Variable(ts.TotalMilliseconds);
    }
  }

  // Returns current directory name
  class PwdFunction : ParserFunction
  {
    internal PwdFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string path = Directory.GetCurrentDirectory();
      m_interpreter.AppendOutput(path);

      return new Variable(path);
    }

    private Interpreter m_interpreter;
  }

  // Equivalent to cd.. on Windows: one directory up
  class Cd__Function : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string newDir = null;

      try
      {
        string pwd = Directory.GetCurrentDirectory();
        newDir = Directory.GetParent(pwd).FullName;
        Directory.SetCurrentDirectory(newDir);
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Could not change directory: " + exc.Message);
      }

      return new Variable(newDir);
    }
  }

  // Changes directory to the passed one
  class CdFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      if (data.Substring(from).StartsWith (" ..")) {
        from++;
      }
      string newDir = Utils.GetStringOrVarValue(data, ref from);

      try
      {
        if (newDir == "..") {
          string pwd = Directory.GetCurrentDirectory();
          newDir = Directory.GetParent(pwd).FullName;
        }
        if (newDir.Length == 0) {
          newDir = Environment.GetEnvironmentVariable("HOME");
        }
        Directory.SetCurrentDirectory(newDir);

        newDir = Directory.GetCurrentDirectory();
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Couldn't change directory: " + exc.Message);
      }

      return new Variable(newDir);
    }
  }

  // Reads a file and returns all lines of that file as a "tuple" (list)
  class ReadFileFunction : ParserFunction
  {
    internal ReadFileFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      string[] lines = Utils.GetFileLines(filename);

      List<Variable> results = Utils.ConvertToResults(lines);
      m_interpreter.AppendOutput("Read " + lines.Length + " line(s).");

      return new Variable(results);
    }

    private Interpreter m_interpreter;
  }

  // View the contents of a text file
  class MoreFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      int size = Constants.DEFAULT_FILE_LINES;

      bool sizeAvailable = Utils.SeparatorExists(data, from);
      if (sizeAvailable)
      {
        Variable length = Utils.GetItem(data, ref from);
        Utils.CheckPosInt(length);
        size = (int)length.Value;
      }

      string[] lines = Utils.GetFileLines(filename, 0, size);
      List<Variable> results = Utils.ConvertToResults(lines);

      return new Variable(results);
    }
  }

  // View the last Constants.DEFAULT_FILE_LINES lines of a file
  class TailFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      int size = Constants.DEFAULT_FILE_LINES;

      bool sizeAvailable = Utils.SeparatorExists(data, from);
      if (sizeAvailable)
      {
        Variable length = Utils.GetItem(data, ref from);
        Utils.CheckPosInt(length);
        size = (int)length.Value;
      }

      string[] lines = Utils.GetFileLines(filename, -1, size);
      List<Variable> results = Utils.ConvertToResults(lines);

      return new Variable(results);
    }
  }

  // Append a line to a file
  class AppendLineFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      Variable line = Utils.GetItem(data, ref from);
      Utils.AppendFileText(filename, line.AsString() + Environment.NewLine);

      return Variable.EmptyInstance;
    }
  }

  // Apend a list of lines to a file
  class AppendLinesFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      string lines = Utils.GetLinesFromList(data, ref from);
      Utils.AppendFileText(filename, lines);

      return Variable.EmptyInstance;
    }
  }

  // Write a line to a file
  class WriteLineFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string filename = Utils.GetStringOrVarValue(data, ref from);
      Variable line = Utils.GetItem(data, ref from);
      Utils.WriteFileText(filename, line.AsString() + Environment.NewLine);

      return Variable.EmptyInstance;
    }
  }

  // Write a list of lines to a file
  class WriteLinesFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      //string filename = Utils.ResultToString(Utils.GetItem(data, ref from));
      string filename = Utils.GetStringOrVarValue(data, ref from);
      string lines = Utils.GetLinesFromList(data, ref from);
      Utils.WriteFileText(filename, lines);

      return Variable.EmptyInstance;
    }
  }

  // Find a string in files
  class FindstrFunction : ParserFunction
  {
    internal FindstrFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string search = Utils.GetStringOrVarValue(data, ref from);
      List<string> patterns = Utils.GetFunctionArgs(data, ref from);

      bool ignoreCase = true;
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
        string pwd = Directory.GetCurrentDirectory();
        List<string> files = Utils.GetStringInFiles(pwd, search, patterns.ToArray(), ignoreCase);

        results = Utils.ConvertToResults(files.ToArray(), m_interpreter);
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Couldn't find pattern: " + exc.Message);
      }

      return new Variable(results);
    }

    private Interpreter m_interpreter;
  }

  // Find files having a given pattern
  class FindfilesFunction : ParserFunction
  {
    internal FindfilesFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      List<string> patterns = Utils.GetFunctionArgs(data, ref from);
      if (patterns.Count == 0)
      {
        patterns.Add("*.*");
      }

      List<Variable> results = null;
      try
      {
        string pwd = Directory.GetCurrentDirectory();
        List<string> files = Utils.GetFiles(pwd, patterns.ToArray());

        results = Utils.ConvertToResults(files.ToArray(), m_interpreter);
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Couldn't list directory: " + exc.Message);
      }

      return new Variable(results);
    }

    private Interpreter m_interpreter;
  }

  // Copy a file or a directiry
  class CopyFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string source = Utils.GetStringOrVarValue(data, ref from);
      Utils.MoveForwardIf (data, ref from, Constants.NEXT_ARG, Constants.SPACE);
      string dest   = Utils.GetStringOrVarValue(data, ref from);

      string src = Path.GetFullPath(source);
      string dst = Path.GetFullPath(dest);

      bool isFile = File.Exists(src);
      bool isDir  = Directory.Exists(src);
      if (!isFile && !isDir) {
        throw new ArgumentException("[" + src + "] doesn't exist");
      }

      if (isFile && Directory.Exists (dst)) {
        // If filename is missing in the destination file,
        // add it from the source.
        dst = Path.Combine(dst, Path.GetFileName(src));
      }

      try
      {
        if (isFile) { 
          File.Copy(src, dst, true);
        } else {
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
  class MoveFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string source = Utils.GetStringOrVarValue(data, ref from);
      Utils.MoveForwardIf (data, ref from, Constants.NEXT_ARG, Constants.SPACE);
      string dest   = Utils.GetStringOrVarValue(data, ref from);

      string src = Path.GetFullPath(source);
      string dst = Path.GetFullPath(dest);

      bool isFile = File.Exists(src);
      bool isDir  = Directory.Exists(src);
      if (!isFile && !isDir) {
        throw new ArgumentException("[" + src + "] doesn't exist");
      }

      if (isFile && Directory.Exists (dst)) {
        // If filename is missing in the destination file,
        // add it from the source.
        dst = Path.Combine(dst, Path.GetFileName(src));
      }

      try
      {
        if (isFile) { 
          File.Move(src, dst);
        } else {
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
  class MkdirFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string dirname = Utils.GetStringOrVarValue(data, ref from);
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
  class DeleteFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string pathname = Utils.GetStringOrVarValue(data, ref from);

      bool isFile = File.Exists(pathname);
      bool isDir  = Directory.Exists(pathname);
      if (!isFile && !isDir) {
        throw new ArgumentException("[" + pathname + "] doesn't exist");
      }
      try
      {
        if (isFile) { 
          File.Delete(pathname);
        } else {
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
  class ExistsFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string pathname = Utils.GetStringOrVarValue(data, ref from);

      bool isFile = File.Exists(pathname);
      bool isDir  = Directory.Exists(pathname);
      if (!isFile && !isDir) {
        throw new ArgumentException("[" + pathname + "] doesn't exist");
      }
      bool exists = false;
      try
      {
        if (isFile) { 
          exists = File.Exists(pathname);
        } else {
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
  class DirFunction : ParserFunction
  {
    internal DirFunction(Interpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
      string dirname = (data.Length <= from || data[from] == Constants.END_STATEMENT) ?
        Directory.GetCurrentDirectory() :
        Utils.GetToken(data, ref from, Constants.NEXT_OR_END_ARRAY);

      string pattern = Constants.ALL_FILES;
      List<Variable> results = new List<Variable>();

      try
      {
        // Special dealing if there is a pattern (only * is supported at the moment)
        int index = dirname.IndexOf('*');
        if (index >= 0) {
          pattern = Path.GetFileName(dirname);

          if (index > 0) {
            string prefix = dirname.Substring (0, index);
            DirectoryInfo di = Directory.GetParent (prefix);
            dirname = di.FullName;
          } else {
            dirname = ".";
          }
        }

        string dir = Path.GetFullPath(dirname);

        // First get contents of the directory (unless there is a pattern)
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (pattern == Constants.ALL_FILES) {
          m_interpreter.AppendOutput(Utils.GetPathDetails (dirInfo, "."));
          m_interpreter.AppendOutput(Utils.GetPathDetails (dirInfo.Parent, ".."));
        }

        // Then get contents of all of the files in the directory
        FileInfo[] fileNames = dirInfo.GetFiles(pattern);
        foreach (FileInfo fi in fileNames)
        {
          m_interpreter.AppendOutput(Utils.GetPathDetails(fi, fi.Name));
          results.Add (new Variable(fi.Name));
        }

        // Then get contents of all of the subdirs in the directory
        DirectoryInfo[] dirInfos = dirInfo.GetDirectories(pattern);
        foreach (DirectoryInfo di in dirInfos)
        {
          m_interpreter.AppendOutput(Utils.GetPathDetails(di, di.Name));
          results.Add (new Variable(di.Name));
        }
      }
      catch (Exception exc)
      {
        throw new ArgumentException("Could not list directory: " + exc.Message);
      }

      return new Variable(results);
    }

    private Interpreter m_interpreter;
  }

  // Append a string to another string
  class AppendFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't append variable");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Get the value to be added or appended.
      Variable newValue = Utils.GetItem(data, ref from);

      // 4. Take either the string part if it is defined,
      // or the numerical part converted to a string otherwise.
      string arg1 = currentValue.AsString();
      string arg2 = newValue.AsString();

      // 5. The variable becomes a string after adding a string to it.
      newValue.Reset();
      newValue.String = arg1 + arg2;

      ParserFunction.AddGlobalOrLocalVariable(varName, new GetVarFunction(newValue));

      return newValue;
    }
  }

  // Convert a string to the upper case
  class ToUpperFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't get variable");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Take either the string part if it is defined,
      // or the numerical part converted to a string otherwise.
      string arg = currentValue.AsString();

      Variable newValue = new Variable(arg.ToUpper());
      return newValue;
    }
  }

  // Convert a string to the lower case
  class ToLowerFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't get variable");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Take either the string part if it is defined,
      // or the numerical part converted to a string otherwise.
      string arg = currentValue.AsString();

      Variable newValue = new Variable(arg.ToLower());
      return newValue;
    }
  }

  // Get a substring of a string
  class SubstrFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      string substring;

      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't get variable");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Take either the string part if it is defined,
      // or the numerical part converted to a string otherwise.
      string arg = currentValue.AsString();
      // 4. Get the initial index of the substring.
      Variable init = Utils.GetItem(data, ref from);
      Utils.CheckNonNegativeInt(init);

      // 5. Get the length of the substring if available.
      bool lengthAvailable = Utils.SeparatorExists(data, from);
      if (lengthAvailable)
      {
        Variable length = Utils.GetItem(data, ref from);
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
      Variable newValue = new Variable(substring);

      return newValue;
    }
  }

  // Get an index of a substring in a string. Return -1 if not found.
  class IndexOfFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      // 1. Get the name of the variable.
      string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
      if (from >= data.Length)
      {
        throw new ArgumentException("Couldn't extract variable name");
      }

      // 2. Get the current value of the variable.
      ParserFunction func = ParserFunction.GetFunction(varName);
      Variable currentValue = func.GetValue(data, ref from);

      // 3. Get the value to be looked for.
      Variable searchValue = Utils.GetItem(data, ref from);

      // 4. Take either the string part if it is defined,
      // or the numerical part converted to a string otherwise.
      string basePart = currentValue.AsString();
      string search = searchValue.AsString();

      int result = basePart.IndexOf(search);
      return new Variable(result);
    }
  }
}
