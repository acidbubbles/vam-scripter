using System.Collections.Generic;
using System.Linq;

public static class Constants
{
    public const char StartArg = '(';
    public const char EndArg = ')';
    public const char StartArray = '[';
    public const char EndArray = ']';
    public const char EndLine = '\n';
    public const char NextArg = ',';
    public const char Quote = '"';
    public const char Space = ' ';
    public const char StartGroup = '{';
    public const char EndGroup = '}';
    public const char EndStatement = ';';
    public const char Empty = '\0';

    public const string Assignment = "=";
    public const string And = "&&";
    public const string Or = "||";
    public const string Not = "!";
    public const string Increment = "++";
    public const string Decrement = "--";
    public const string Equal = "==";
    public const string NotEqual = "!=";
    public const string Less = "<";
    public const string LessEq = "<=";
    public const string Greater = ">";
    public const string GreaterEq = ">=";
    public const string AddAssign = "+=";
    public const string SubtAssign = "-=";
    public const string MultAssign = "*=";
    public const string DivAssign = "/=";

    public const string If = "if";
    public const string Else = "else";
    public const string ElseIf = "elif";
    public const string While = "while";
    public const string Break = "break";
    public const string Continue = "continue";
    public const string Function = "function";
    public const string Return = "return";
    public const string Try = "try";
    public const string Catch = "catch";
    public const string Throw = "throw";
    public const string Comment = "//";

    public const string Abs = "abs";
    public const string Acos = "acos";
    public const string Asin = "asin";
    public const string Ceil = "ceil";
    public const string Cos = "cos";
    public const string Exp = "exp";
    public const string Floor = "floor";
    public const string IndexOf = "indexof";
    public const string LOG = "log";
    public const string PI = "pi";
    public const string Pow = "pow";
    public const string Round = "round";
    public const string Set = "set";
    public const string Sin = "sin";
    public const string Size = "size";
    public const string Sqrt = "sqrt";
    public const string Substr = "substr";
    public const string Tolower = "tolower";
    public const string Toupper = "toupper";

    public static string EndArgStr = EndArg.ToString();

    public static string[] OperActions = { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=" };

    public static string[] MathActions =
    {
        "&&", "||", "==", "!=", "<=", ">=", "++", "--",
        "%", "*", "/", "+", "-", "^", "<", ">", "="
    };

    // Actions: always decreasing by the number of characters.
    public static string[] Actions = (OperActions.Union(MathActions)).ToArray();

    public static char[] NextArgArray = NextArg.ToString().ToCharArray();
    public static char[] EndArgArray = EndArg.ToString().ToCharArray();
    public static char[] EndArrayArray = EndArray.ToString().ToCharArray();
    public static char[] EndLineArray = EndLine.ToString().ToCharArray();
    public static char[] QuoteArray = Quote.ToString().ToCharArray();

    public static char[] CompareArray = "<>=)".ToCharArray();
    public static char[] IfArgArray = "&|)".ToCharArray();
    public static char[] EndParseArray = " ;)}\n".ToCharArray();
    public static char[] NextOrEndArray = { NextArg, EndArg, /*START_GROUP,*/ EndGroup, EndStatement, Space };

    public static char[] TokenSeparation = ("<>=+-*/%&|^\t " + "\r\n" +
                                             Not + StartArg + EndArg + StartGroup + EndGroup + NextArg + EndStatement).ToCharArray();

    // Functions that allow a space separator after them, on top of parentheses. The
    // function arguments may have spaces as well, e.g. copy a.txt b.txt
    public static List<string> FunctWithSpace = new List<string>
    {
        Function
    };

    // Functions that allow a space separator after them, on top of parentheses but
    // only once, i.e. function arguments are not allowed to have spaces
    // between them e.g. return a*b;
    public static List<string> FunctWithSpaceOnce = new List<string>
    {
        Return
    };

    // The Control Flow Functions. It doesn't make sense to merge them or
    // use in calculation of a result.
    public static List<string> ControlFlow = new List<string>
    {
        Break, Continue, Function, If, While, Return, Throw, Try
    };

    public static List<string> ElseList = new List<string>();
    public static List<string> ElseIfList = new List<string>();
    public static List<string> CatchList = new List<string>();

    public static int DefaultFileLines = 20;

    public static string AllFiles = "*.*";

    public static int MaxErrorChars = 20;
}
