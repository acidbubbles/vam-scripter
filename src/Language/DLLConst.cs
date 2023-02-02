namespace SplitAndMerge
{
    public class DLLConst
    {
        public const int MaxWorkMethods = 10;
        public const string WorkerName = "DoWork";
        public const string GetArgsName = "GetArgData";
        public const string ClassName = "CustomPrecompiler";
        public const string ClassHeader = "  public class " + ClassName + " : ICustomDLL {";

        public static string GetWorkerMethod(int id = 1)
        {
            return WorkerName + id;
        }
        public static string GetArgsMethod(int id)
        {
            return GetArgsName + id;
        }
    }
}