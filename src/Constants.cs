using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
	public class Constants
	{
    public const char START_ARG     = '(';
    public const char END_ARG       = ')';
    public const char START_ARRAY   = '[';
    public const char END_ARRAY     = ']';
		public const char END_LINE      = '\n';
		public const char NEXT_ARG      = ',';
		public const char QUOTE         = '"';
		public const char SPACE         = ' ';
		public const char START_GROUP   = '{';
		public const char END_GROUP     = '}';
		public const char END_STATEMENT = ';';
		public const char EMPTY         = '\0';

		public const string ASSIGNMENT  = "=";
		public const string AND         = "&&";
		public const string OR          = "||";
		public const string NOT         = "!";
		public const string INCREMENT   = "++";
		public const string DECREMENT   = "--";
		public const string EQUAL       = "==";
		public const string NOT_EQUAL   = "!=";
		public const string LESS        = "<";
		public const string LESS_EQ     = "<=";
		public const string GREATER     = ">";
		public const string GREATER_EQ  = ">=";
		public const string ADD_ASSIGN  = "+=";
		public const string SUBT_ASSIGN = "-=";
		public const string MULT_ASSIGN = "*=";
		public const string DIV_ASSIGN  = "/=";

		public const string IF          = "if";
		public const string ELSE        = "else";
		public const string ELSE_IF     = "elif";
		public const string WHILE       = "while";
		public const string BREAK       = "break";
		public const string CONTINUE    = "continue";
		public const string FUNCTION    = "function";
		public const string RETURN      = "return";
		public const string INCLUDE     = "include";
		public const string TRY         = "try";
		public const string CATCH       = "catch";
		public const string THROW       = "throw";
		public const string COMMENT     = "//";

		public const string ABS         = "abs";
		public const string ACOS        = "acos";
		public const string APPEND      = "append";
		public const string APPENDLINE  = "appendline";
		public const string APPENDLINES = "appendlines";
		public const string ASIN        = "asin";
		public const string CD          = "cd";
		public const string CD__        = "cd..";
		public const string COPY        = "copy";
		public const string CEIL        = "ceil";
		public const string CONNECTSRV  = "connectsrv";
		public const string COS         = "cos";
		public const string DIR         = "dir";
		public const string DELETE      = "del";
		public const string ENV         = "env";
		public const string EXISTS      = "exists";
		public const string EXP         = "exp";
		public const string FINDFILES   = "findfiles";
		public const string FINDSTR     = "findstr";
		public const string FLOOR       = "floor";
		public const string INDEX_OF    = "indexof";
    public const string KILL        = "kill";
    public const string LOG         = "log";
		public const string MKDIR       = "mkdir";
		public const string MORE        = "more";
		public const string MOVE        = "move";
		public const string PI          = "pi";
		public const string POW         = "pow";
		public const string PSINFO      = "psinfo";
		public const string PSTIME      = "pstime";
		public const string PWD         = "pwd";
    public const string READ        = "read";
    public const string READFILE    = "readfile";
    public const string READNUMBER  = "readnum";
		public const string ROUND       = "round";
		public const string RUN         = "run";
    public const string SETENV      = "setenv";
		public const string SET         = "set";
		public const string SIN         = "sin";
		public const string SIZE        = "size";
		public const string SQRT        = "sqrt";
		public const string STARTSRV    = "startsrv";
		public const string SUBSTR      = "substr";
		public const string TAIL        = "tail";
		public const string TOLOWER     = "tolower";
		public const string TOUPPER     = "toupper";
    public const string WRITE       = "write";
    public const string WRITELINE   = "writeline";
		public const string WRITELINES  = "writelines";
    public const string WRITENL     = "writenl";

		public static string END_ARG_STR    = END_ARG.ToString();

		public static string[] OPER_ACTIONS = { "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^="};
		public static string[] MATH_ACTIONS = { "&&", "||", "==", "!=", "<=", ">=", "++", "--",
			                                    "%", "*", "/", "+", "-", "^", "<", ">", "="};

		// Actions: always decreasing by the number of characters.
		public static string[] ACTIONS      = (OPER_ACTIONS.Union(MATH_ACTIONS)).ToArray();

		public static char[] NEXT_ARG_ARRAY     = NEXT_ARG.ToString().ToCharArray();
    public static char[] END_ARG_ARRAY      = END_ARG.ToString().ToCharArray();
    public static char[] END_ARRAY_ARRAY    = END_ARRAY.ToString().ToCharArray();
		public static char[] END_LINE_ARRAY     = END_LINE.ToString().ToCharArray();
		public static char[] QUOTE_ARRAY        = QUOTE.ToString().ToCharArray();

		public static char[] COMPARE_ARRAY      = "<>=)".ToCharArray();
		public static char[] IF_ARG_ARRAY       = "&|)".ToCharArray();
		public static char[] END_PARSE_ARRAY    = " ;)}\n".ToCharArray();
		public static char[] NEXT_OR_END_ARRAY  = { NEXT_ARG, END_ARG, /*START_GROUP,*/ END_GROUP, END_STATEMENT, SPACE };

		public static char[] TOKEN_SEPARATION = ("<>=+-*/%&|^\t " + Environment.NewLine +
			NOT + START_ARG + END_ARG + START_GROUP + END_GROUP + NEXT_ARG + END_STATEMENT).ToCharArray();

		// Functions that allow a space separator after them, on top of parentheses. The
		// function arguments may have spaces as well, e.g. copy a.txt b.txt
		public static List<string> FUNCT_WITH_SPACE = new List<string> {
			APPENDLINE, CD, CONNECTSRV, COPY, DELETE, DIR, EXISTS, FINDFILES, FINDSTR,
      FUNCTION, MKDIR, MORE, MOVE, READFILE, RUN, STARTSRV, TAIL, WRITE, WRITELINE, WRITENL
		};

    // Functions that allow a space separator after them, on top of parentheses but
		// only once, i.e. function arguments are not allowed to have spaces
		// between them e.g. return a*b;
		public static List<string> FUNCT_WITH_SPACE_ONCE = new List<string> {
			RETURN
		};

    // The Control Flow Functions. It doesn't make sense to merge them or
    // use in calculation of a result.
    public static List<string> CONTROL_FLOW = new List<string> {
      BREAK, CONTINUE, FUNCTION, IF, INCLUDE, WHILE, RETURN, THROW, TRY
    };

    public static List<string> ELSE_LIST    = new List<string>();
		public static List<string> ELSE_IF_LIST = new List<string>();
		public static List<string> CATCH_LIST   = new List<string>();

		public static int DEFAULT_FILE_LINES    = 20;

		public static string ALL_FILES = "*.*";

    public static int MAX_ERROR_CHARS = 20;
	}
}
