using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Utils
{
    public static void CheckArgs(int args, int expected, string msg)
    {
        if (args < expected)
        {
            throw new ArgumentException("Expecting " + expected +
                                        " arguments but got " + args + " in " + msg);
        }
    }

    public static void CheckPosInt(Variable variable)
    {
        CheckInteger(variable);
        if (variable.Value <= 0)
        {
            throw new ArgumentException("Expecting a positive integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckNonNegativeInt(Variable variable)
    {
        CheckInteger(variable);
        if (variable.Value < 0)
        {
            throw new ArgumentException("Expecting a non  negative integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckInteger(Variable variable)
    {
        CheckNumber(variable);
        if (variable.Value % 1 != 0)
        {
            throw new ArgumentException("Expecting an integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckNumber(Variable variable)
    {
        if (variable.Type != Variable.VarType.Number)
        {
            throw new ArgumentException("Expecting a number instead of [" +
                                        variable.String + "]");
        }
    }

    public static string GetPathDetails(FileSystemInfo fs, string name)
    {
        var pathname = fs.FullName;
        var isDir = (fs.Attributes & FileAttributes.Directory) != 0;

        var d = isDir ? 'd' : '-';
        var last = fs.LastWriteTime.ToString("MMM dd yyyy HH:mm");

#if __MonoCS__
			Mono.Unix.UnixFileSystemInfo info;
			if (isDir) {
				info = new Mono.Unix.UnixDirectoryInfo(pathname);
			} else {
				info = new Mono.Unix.UnixFileInfo(pathname);
			}

			char ur = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.UserRead)     != 0 ? 'r' : '-';
			char uw = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.UserWrite)    != 0 ? 'w' : '-';
			char ux = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.UserExecute)  != 0 ? 'x' : '-';
			char gr = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.GroupRead)    != 0 ? 'r' : '-';
			char gw = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.GroupWrite)   != 0 ? 'w' : '-';
			char gx = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.GroupExecute) != 0 ? 'x' : '-';
			char or = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.OtherRead)    != 0 ? 'r' : '-';
			char ow = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.OtherWrite)   != 0 ? 'w' : '-';
			char ox = (info.FileAccessPermissions & Mono.Unix.FileAccessPermissions.OtherExecute) != 0 ? 'x' : '-';

			string permissions = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}",
				ur, uw, ux, gr, gw, gx, or, ow, ox);

			string user = info.OwnerUser.UserName;
			string group = info.OwnerGroup.GroupName;
			string links = info.LinkCount.ToString();

			long size = info.Length;

			if (info.IsSymbolicLink) {
				d = 's';
			}

#else

        var user = string.Empty;
        var group = string.Empty;
        string links = null;
        var permissions = "rwx";

        long size = 0;

        if (isDir)
        {
            user = Directory.GetAccessControl(fs.FullName).GetOwner(
                typeof(System.Security.Principal.NTAccount)).ToString();

            var di = fs as DirectoryInfo;
            size = di.GetFileSystemInfos().Length;
        }
        else
        {
            user = File.GetAccessControl(fs.FullName).GetOwner(
                typeof(System.Security.Principal.NTAccount)).ToString();
            var fi = fs as FileInfo;
            size = fi.Length;

            var execs = new string[] { "exe", "bat", "msi" };
            var x = execs.Contains(fi.Extension.ToLower()) ? 'x' : '-';
            var w = !fi.IsReadOnly ? 'w' : '-';
            permissions = string.Format("r{0}{1}", w, x);
            ;
        }

#endif

        var infoStr = string.Format("{0}{1} {2,4} {3,8} {4,8} {5,9} {6,23} {7}",
            d, permissions, links, user, group, size, last, name);

        return infoStr;
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
    {
        // Get the subdirectories for the specified directory.
        var dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                sourceDirName + " directory doesn't exist");
        }

        var dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        var files = dir.GetFiles();
        foreach (var file in files)
        {
            var temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, true);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (var subdir in dirs)
            {
                var tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }

    public static List<string> GetFiles(string path, string[] patterns, bool addDirs = true)
    {
        var files = new List<string>();
        GetFiles(path, patterns, ref files, addDirs);
        return files;
    }

    public static string GetFileEntry(string dir, int i, string startsWith)
    {
        var files = new List<string>();
        string[] patterns = { startsWith + "*" };
        GetFiles(dir, patterns, ref files, true, false);

        if (files.Count == 0)
        {
            return "";
        }

        i = i % files.Count;

        var pathname = files[i];
        if (files.Count == 1)
        {
            pathname += Directory.Exists(Path.Combine(dir, pathname)) ? Path.DirectorySeparatorChar.ToString() : " ";
        }

        return pathname;
    }

    public static void GetFiles(string path, string[] patterns, ref List<string> files,
        bool addDirs = true, bool recursive = true)
    {
        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        if (string.IsNullOrEmpty(path))
        {
            path = Directory.GetCurrentDirectory();
        }

        var dirs = patterns.SelectMany(
            i => Directory.EnumerateDirectories(path, i, option)
        ).ToList<string>();

        var extraFiles = patterns.SelectMany(
            i => Directory.EnumerateFiles(path, i, option)
        ).ToList<string>();

        if (addDirs)
        {
            files.AddRange(dirs);
        }

        files.AddRange(extraFiles);

        if (!recursive)
        {
            files = files.Select(p => Path.GetFileName(p)).ToList<string>();
            files.Sort();
            return;
        }
        /*foreach (string dir in dirs) {
          GetFiles (dir, patterns, addDirs);
        }*/
    }

    public static List<Variable> ConvertToResults(string[] items,
        Interpreter interpreter = null)
    {
        var results = new List<Variable>(items.Length);
        foreach (var item in items)
        {
            results.Add(new Variable(item));
            if (interpreter != null)
            {
                interpreter.AppendOutput(item);
            }
        }

        return results;
    }

    public static List<string> GetStringInFiles(string path, string search,
        string[] patterns, bool ignoreCase = true)
    {
        var allFiles = GetFiles(path, patterns, false /* no dirs */);
        var results = new List<string>();

        if (allFiles == null && allFiles.Count == 0)
        {
            return results;
        }

        var caseSense = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        Parallel.ForEach(allFiles, (currentFile) =>
        {
            var contents = GetFileText(currentFile);
            if (contents.IndexOf(search, caseSense) >= 0)
            {
                lock (_sMutexLock)
                {
                    results.Add(currentFile);
                }
            }
        });

        return results;
    }

    private static void WriteBlinkingText(string text, int delay, bool visible)
    {
        if (visible)
        {
            Console.Write(text);
        }
        else
        {
            Console.Write(new string(' ', text.Length));
        }

        Console.CursorLeft -= text.Length;
        System.Threading.Thread.Sleep(delay);
    }

    public static string GetLine(int chars = 40)
    {
        return string.Format("-").PadRight(chars, '-');
    }

    public static string GetFileText(string filename)
    {
        var fileContents = string.Empty;
        if (File.Exists(filename))
        {
            fileContents = File.ReadAllText(filename);
        }

        return fileContents;
    }

    public static string[] GetFileLines(string filename)
    {
        try
        {
            var lines = File.ReadAllLines(filename);
            return lines;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Couldn't read file from disk: " + ex.Message);
        }
    }

    public static string[] GetFileLines(string filename, int from, int count)
    {
        try
        {
            var allLines = File.ReadLines(filename).ToArray();
            if (allLines.Length <= count)
            {
                return allLines;
            }

            if (from < 0)
            {
                // last n lines
                from = allLines.Length - count;
            }

            var lines = allLines.Skip(from).Take(count).ToArray();
            return lines;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Couldn't read file from disk: " + ex.Message);
        }
    }

    public static void WriteFileText(string filename, string text)
    {
        try
        {
            File.WriteAllText(filename, text);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Couldn't write file to disk: " + ex.Message);
        }
    }

    public static void AppendFileText(string filename, string text)
    {
        try
        {
            File.AppendAllText(filename, text);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Couldn't write file to disk: " + ex.Message);
        }
    }

    public static void PrintList(List<Variable> list, int from)
    {
        Console.Write("Merging list:");
        for (var i = from; i < list.Count; i++)
        {
            Console.Write(" ({0}, '{1}')", list[i].Value, list[i].Action);
        }

        Console.WriteLine();
    }

    private static readonly object _sMutexLock = new object();
}
