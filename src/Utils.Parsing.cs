using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitAndMerge
{
	public partial class Utils
	{
		public static Variable GetItem(string data, ref int from)
		{
			MoveForwardIf(data, ref from, Constants.NEXT_ARG, Constants.SPACE);

			if (data.Length <= from)
			{
				throw new ArgumentException("Incomplete function definition");
			}

			Variable value = new Variable();

			if (data[from] == Constants.QUOTE)
			{
				// We are extracting a string between quotes.
				from++; // Skip first quote.
				if (from < data.Length && data[from] == Constants.QUOTE)
				{ // the argument is ""
					value.String = "";
				}
				else
				{
					value.String = Utils.GetToken(data, ref from, Constants.QUOTE_ARRAY);
				}
				from++; // skip next separation char
			}
			else if (data[from] == Constants.START_GROUP)
			{
				// We are extracting a list between curly braces.
				from++; // Skip first brace.
				bool isList = true;
				value.Tuple = GetArgs (data, ref from,
					Constants.START_GROUP, Constants.END_GROUP, out isList);

				return value;
			}
			else
			{
				// A variable, a function, or a number.
				Variable var = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_OR_END_ARRAY);
				value.Copy(var);
			}

			MoveForwardIf(data, ref from, Constants.END_ARG, Constants.SPACE);
			return value;
		}

		public static string GetToken(string data, ref int from, char[] to)

		{
			char curr = from < data.Length ? data[from] : Constants.EMPTY;
			char prev = from > 0 ? data[from - 1] : Constants.EMPTY;

			if (!to.Contains (Constants.SPACE)) {
				// Skip a leading space unless we are inside of quotes
				while (curr == Constants.SPACE && prev != Constants.QUOTE) {
					from++;
					curr = from < data.Length ? data [from] : Constants.EMPTY;
					prev = from > 0 ? data [from - 1] : Constants.EMPTY;
				}
			}

			MoveForwardIf (data, ref from, Constants.QUOTE);

			int end = data.IndexOfAny(to, from);
			if (from >= end)
			{
				from++;
				return string.Empty;
			}

			// Skip found characters that have a backslash before.
			while ((end > 0 && data[end - 1] == '\\') && end + 1 < data.Length)
			{
				end = data.IndexOfAny(to, end + 1);
			}

			if (end < from)
			{
				throw new ArgumentException("Couldn't extract token from " + data.Substring(from));
			}

			if (data[end - 1] == Constants.QUOTE)
			{
				end--;
			}

			string var = data.Substring(from, end - from);
			// \"yes\" --> "yes"
			var = var.Replace("\\\"", "\"");
			//from = end + 1;
			from = end;

			MoveForwardIf (data, ref from, Constants.QUOTE, Constants.SPACE);

			return var;
		}

		public static string GetNextToken(string data, ref int from)
		{
			if (from >= data.Length)
			{
				return "";
			}

			int end = data.IndexOfAny(Constants.TOKEN_SEPARATION, from);

			if (end < from)
			{
				return "";
			}

			string var = data.Substring(from, end - from);
			from = end;
			return var;
		}

		public static string GetStringOrVarValue(string data, ref int from)
		{
			MoveForwardIf(data, ref from, Constants.SPACE);

			// If this token starts with a quote then it is a string constant.
			// Otherwide we treat it as a variable, but if the variable doesn't exist then it
			// will be still treated as a string constant.
			bool stringConstant = data.Substring(from).StartsWith(Constants.QUOTE.ToString());

			string token = Utils.GetToken(data, ref from, Constants.NEXT_OR_END_ARRAY);
			// Check if this is a variable definition:
			stringConstant = stringConstant || !ParserFunction.FunctionExists(token);
			if (!stringConstant) {
				Variable sourceValue = ParserFunction.GetFunction(token).GetValue (data, ref from);
				token = sourceValue.String;
			}

			return token;
		}

		public static int GoToNextStatement(string data, ref int from)
		{
			int endGroupRead = 0;
			while (from < data.Length)
			{
				char currentChar = data[from];
				switch (currentChar)
				{
				case Constants.END_GROUP: endGroupRead++;
					from++;
					return endGroupRead;
				case Constants.START_GROUP:
				case Constants.QUOTE:
				case Constants.SPACE:
				case Constants.END_STATEMENT:
				case Constants.END_ARG:
					from++;
					break;
				default: return endGroupRead;
				}
			}
			return endGroupRead;
		}

		public static string GetComparison(string data, ref int from)
		{
			if (data.Length <= from + 1)
			{
				throw new ArgumentException("End of line before extracting comparison token");
			}

			if (!Utils.IsCompareSign(data[from + 1]))
			{
				if (data[from] == '<' || data[from] == '>')
				{
					return data.Substring(from++, 1);
				}
				throw new ArgumentException("Couldn't extract comparison token from " + data.Substring(from));
			}

			string result = data.Substring(from, 2);
			from++; from++;
			return result;
		}

		public static bool IsCompareSign(char ch)
		{
			return ch == '<' || ch == '>' || ch == '=';
		}

		public static bool IsAndOrSign(char ch)
		{
			return ch == '&' || ch == '|';
		}

		// Checks whether there is an argument separator (e.g.  ',') before the end of the
		// function call. E.g. returns true for "a,b)" and "a(b,c),d)" and false for "b),c".
		public static bool SeparatorExists(string data, int from)
		{
			if (from >= data.Length)
			{
				return false;
			}

			int argumentList = 0;
			for (int i = from; i < data.Length; i++)
			{
				char ch = data[i];
				switch (ch)
				{
				case Constants.NEXT_ARG:
					return true;
				case Constants.START_ARG:
					argumentList++;
					break;
				case Constants.END_STATEMENT:
				case Constants.END_GROUP:
				case Constants.END_ARG:
					if (--argumentList < 0)
					{
						return false;
					}
					break;
				}
			}

			return false;
		}

		public static List<string> GetFunctionArgs(string data, ref int from)
		{
			bool isList;
			List<Variable> args = Utils.GetArgs(data, ref from,
				Constants.START_ARG, Constants.END_ARG, out isList);

			List<string> result = new List<string>();
			for (int i = 0; i < args.Count; i++)
			{
        result.Add(args[i].AsString());
			}
			return result;
		}

		public static List<Variable> GetArgs(string data, ref int from,
			char start, char end, out bool isList)
		{
			List<Variable> args = new List<Variable>();

			isList = from < data.Length && data[from] == Constants.START_GROUP;

			if (from >= data.Length || data [from] == Constants.END_STATEMENT) {
				return args;
			}

			int lastChar = from;
			Utils.GetBodyBetween(data, ref lastChar, start, end);

			while (from < lastChar)
			{
        Variable item = Utils.GetItem(data, ref from);
				if (item.Equals(Variable.EmptyInstance)) {
          break;
				}

				args.Add(item);
			}


			MoveForwardIf(data, ref from, end, Constants.SPACE);

			return args;
		}

		public static string[] GetFunctionSignature(string data, ref int from)
		{
			MoveForwardIf(data, ref from, Constants.START_ARG, Constants.SPACE);

			int endArgs = data.IndexOf(Constants.END_ARG, from);
			if (endArgs < 0)
			{
				throw new ArgumentException("Couldn't extract function signature");
			}

			string argStr = data.Substring (from, endArgs - from);
			string[] args = argStr.Split (Constants.NEXT_ARG_ARRAY);

			from = endArgs + 1;

			return args;
		}

		public static int ExtractArrayElement(ref string varName)
		{
			int argStart = varName.IndexOf(Constants.START_ARRAY);
			if (argStart <= 0)
			{
				return -1;
			}

			int argEnd = varName.IndexOf(Constants.END_ARRAY, argStart + 1);
			if (argEnd <= argStart + 1)
			{
				return -1;
			}

			int getIndexFrom = argStart;
      Utils.MoveForwardIf(varName, ref getIndexFrom,
            Constants.START_ARG, Constants.START_ARRAY);

      Variable existing = Parser.LoadAndCalculate(varName, ref getIndexFrom,
                                 Constants.END_ARRAY_ARRAY);

      if (existing.Type == Variable.VarType.NUMBER && existing.Value >= 0)
			{
				varName = varName.Substring(0, argStart);
				return (int)existing.Value;
			}

			return -1;
		}

		public static bool EndsWithFunction(string buffer, List<string> functions)
		{
			foreach (string key in functions) {
				if (buffer.EndsWith (key))
				{
					char prev = key.Length >= buffer.Length ?
						Constants.END_STATEMENT :
						buffer [buffer.Length - key.Length - 1];
					if (Constants.TOKEN_SEPARATION.Contains (prev)) {
						return true;
					}
				}
			}

			return false;
		}

		public static bool SpaceNotNeeded(char next)
		{
			return (next == Constants.SPACE || next == Constants.START_ARG ||
        next == Constants.START_GROUP || next == Constants.START_ARRAY ||
        next == Constants.EMPTY);
		}

		public static bool KeepSpace(StringBuilder sb, char next)
		{
			if (SpaceNotNeeded(next)) {
				return false;
			}

			return EndsWithFunction(sb.ToString(), Constants.FUNCT_WITH_SPACE);
		}
		public static bool KeepSpaceOnce(StringBuilder sb, char next)
		{
			if (SpaceNotNeeded(next)) {
				return false;
			}

			return EndsWithFunction(sb.ToString(), Constants.FUNCT_WITH_SPACE_ONCE);
		}

		public static string ConvertToScript(string source)
		{
			StringBuilder sb = new StringBuilder(source.Length);

			bool inQuotes    = false;
			bool spaceOK     = false;
			bool inComments  = false;
			char previous    = Constants.EMPTY;

			int parentheses = 0;
			int groups = 0;

			for (int i = 0; i < source.Length; i++)
			{
				char ch = source[i];
				char next = i + 1 < source.Length ? source[i + 1] : Constants.EMPTY;

				if (inComments && ch != '\n') {
					continue;
				}

				switch (ch)
				{
				case '/':
					if (inComments || next == '/')
					{
						inComments = true;
						continue;
					}
					break;
				case '“':
				case '”':
				case '"':
					ch = '"';
					if (!inComments)
					{
						if (previous != '\\') inQuotes = !inQuotes;
					}
					break;
				case ' ':
					if (inQuotes) {
						sb.Append (ch);
					}
					else {
						bool keepSpace = KeepSpace(sb, next);
						spaceOK = keepSpace || 
							 (previous != Constants.EMPTY && previous != Constants.NEXT_ARG && spaceOK);
						bool spaceOKonce = KeepSpaceOnce(sb, next);
						if (spaceOK || spaceOKonce) {
							sb.Append(ch);
						}
					}
					continue;
				case '\t':
        case '\r':
					if (inQuotes) sb.Append(ch);
          continue;
				case '\n':
					inComments = false;
					spaceOK    = false;
					continue;
				case Constants.END_ARG:
					if (!inQuotes) {
						parentheses--;
						spaceOK = false;
					}
					break;
				case Constants.START_ARG:
					if (!inQuotes) {
						parentheses++;
					}
					break;
				case Constants.END_GROUP:
					if (!inQuotes) {
						groups--;
						spaceOK = false;
					}
					break;
				case Constants.START_GROUP:
					if (!inQuotes) {
						groups++;
					}
					break;
				case Constants.END_STATEMENT:
					if (!inQuotes) {
						spaceOK = false;
					}
					break;
				default: break;
				}
				if (!inComments)
				{
					sb.Append(ch);
				}
				previous = ch;
			}

			if (parentheses != 0)
			{
				throw new ArgumentException("Uneven parentheses " + Constants.START_ARG + Constants.END_ARG);
			}
			if (groups != 0)
			{
				throw new ArgumentException("Uneven groups " + Constants.START_GROUP + Constants.END_GROUP);
			}

      return sb.ToString();
		}

		public static string GetBodyBetween(string data, ref int from, char open, char close)
		{
			// We are supposed to be one char after the beginning of the string, i.e.
			// we must not have the opening char as the first one.
			StringBuilder sb = new StringBuilder(data.Length);
			int braces = 0;

			for (; from < data.Length; from++)
			{
				char ch = data[from];

				if (string.IsNullOrWhiteSpace(ch.ToString()) && sb.Length == 0) {
					continue;
				} else if (ch == open) {
					braces++;
				} else if (ch == close) {
					braces--;
				}

				sb.Append(ch);
				if (braces == -1)
				{
					if (ch == close) {
						sb.Remove(sb.Length - 1, 1);
					}
					break;
				}
			}

			return sb.ToString();
		}


		public static string IsNotSign(string data)
		{
			return data.StartsWith(Constants.NOT) ? Constants.NOT : null;
		}

		public static string ValidAction(string data, int from)
		{
			if (from < 0 || from >= data.Length)
			{
				return null;
			}
			string action = Utils.StartsWith(data.Substring(from), Constants.ACTIONS);
			return action;
		}

		public static string StartsWith(string data, string[] items)
		{
			foreach (string item in items)
			{
				if (data.StartsWith(item))
				{
					return item;
				}
			}
			return null;
		}

		public static void MoveForwardIf(string data, ref int from, char[] arr)
		{
			foreach (char ch in arr) {
				if (MoveForwardIf (data, ref from, ch)) {
					return;
				}
			}
		}

		public static bool MoveForwardIf(string data, ref int from, char expected,
			                             char expected2 = Constants.EMPTY)
		{
			if (from < data.Length && 
				(data[from] == expected || data[from] == expected2))
			{
				from++;
				return true;
			}
			return false;
		}

		public static bool MoveBackIf(string data, ref int from, char notExpected)
		{
			if (from < data.Length && from > 0 && data[from] == notExpected)
			{
				from--;
				return true;
			}
			return false;
		}

		public static string GetLinesFromList(string data, ref int from)
		{
			Variable lines = Utils.GetItem(data, ref from);
			if (lines.Tuple == null) 
			{
				throw new ArgumentException("Expected a list argument");
			}

			StringBuilder sb = new StringBuilder(80 * lines.Tuple.Count);
			foreach (Variable line in lines.Tuple)
			{
				sb.AppendLine(line.String);
			}

			return sb.ToString();
		}

	}
}
