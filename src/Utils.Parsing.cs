using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class Utils
{
    public static Variable GetItem(string data, ref int from)
    {
        MoveForwardIf(data, ref from, Constants.NextArg, Constants.Space);

        if (data.Length <= from)
        {
            throw new ArgumentException("Incomplete function definition");
        }

        var value = new Variable();

        if (data[from] == Constants.Quote)
        {
            // We are extracting a string between quotes.
            from++; // Skip first quote.
            if (from < data.Length && data[from] == Constants.Quote)
            {
                // the argument is ""
                value.String = "";
            }
            else
            {
                value.String = GetToken(data, ref from, Constants.QuoteArray);
            }

            from++; // skip next separation char
        }
        else if (data[from] == Constants.StartGroup)
        {
            // We are extracting a list between curly braces.
            from++; // Skip first brace.
            var isList = true;
            value.Tuple = GetArgs(data, ref from,
                Constants.StartGroup, Constants.EndGroup, out isList);

            return value;
        }
        else
        {
            // A variable, a function, or a number.
            var var = Parser.LoadAndCalculate(data, ref from, Constants.NextOrEndArray);
            value.Copy(var);
        }

        MoveForwardIf(data, ref from, Constants.EndArg, Constants.Space);
        return value;
    }

    public static string GetToken(string data, ref int from, char[] to)

    {
        var curr = from < data.Length ? data[from] : Constants.Empty;
        var prev = from > 0 ? data[from - 1] : Constants.Empty;

        if (!to.Contains(Constants.Space))
        {
            // Skip a leading space unless we are inside of quotes
            while (curr == Constants.Space && prev != Constants.Quote)
            {
                from++;
                curr = from < data.Length ? data[from] : Constants.Empty;
                prev = from > 0 ? data[from - 1] : Constants.Empty;
            }
        }

        MoveForwardIf(data, ref from, Constants.Quote);

        var end = data.IndexOfAny(to, from);
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

        if (data[end - 1] == Constants.Quote)
        {
            end--;
        }

        var var = data.Substring(from, end - from);
        // \"yes\" --> "yes"
        var = var.Replace("\\\"", "\"");
        //from = end + 1;
        from = end;

        MoveForwardIf(data, ref from, Constants.Quote, Constants.Space);

        return var;
    }

    public static string GetNextToken(string data, ref int from)
    {
        if (from >= data.Length)
        {
            return "";
        }

        var end = data.IndexOfAny(Constants.TokenSeparation, from);

        if (end < from)
        {
            return "";
        }

        var var = data.Substring(from, end - from);
        from = end;
        return var;
    }

    public static string GetStringOrVarValue(string data, ref int from)
    {
        MoveForwardIf(data, ref from, Constants.Space);

        // If this token starts with a quote then it is a string constant.
        // Otherwide we treat it as a variable, but if the variable doesn't exist then it
        // will be still treated as a string constant.
        var stringConstant = data.Substring(from).StartsWith(Constants.Quote.ToString());

        var token = GetToken(data, ref from, Constants.NextOrEndArray);
        // Check if this is a variable definition:
        stringConstant = stringConstant || !ParserFunction.FunctionExists(token);
        if (!stringConstant)
        {
            var sourceValue = ParserFunction.GetFunction(token).GetValue(data, ref from);
            token = sourceValue.String;
        }

        return token;
    }

    public static int GoToNextStatement(string data, ref int from)
    {
        var endGroupRead = 0;
        while (from < data.Length)
        {
            var currentChar = data[from];
            switch (currentChar)
            {
                case Constants.EndGroup:
                    endGroupRead++;
                    from++;
                    return endGroupRead;
                case Constants.StartGroup:
                case Constants.Quote:
                case Constants.Space:
                case Constants.EndStatement:
                case Constants.EndArg:
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

        if (!IsCompareSign(data[from + 1]))
        {
            if (data[from] == '<' || data[from] == '>')
            {
                return data.Substring(from++, 1);
            }

            throw new ArgumentException("Couldn't extract comparison token from " + data.Substring(from));
        }

        var result = data.Substring(from, 2);
        from++;
        from++;
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

        var argumentList = 0;
        for (var i = from; i < data.Length; i++)
        {
            var ch = data[i];
            switch (ch)
            {
                case Constants.NextArg:
                    return true;
                case Constants.StartArg:
                    argumentList++;
                    break;
                case Constants.EndStatement:
                case Constants.EndGroup:
                case Constants.EndArg:
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
        var args = GetArgs(data, ref from,
            Constants.StartArg, Constants.EndArg, out isList);

        var result = new List<string>();
        for (var i = 0; i < args.Count; i++)
        {
            result.Add(args[i].AsString());
        }

        return result;
    }

    public static List<Variable> GetArgs(string data, ref int from,
        char start, char end, out bool isList)
    {
        var args = new List<Variable>();

        isList = from < data.Length && data[from] == Constants.StartGroup;

        if (from >= data.Length || data[from] == Constants.EndStatement)
        {
            return args;
        }

        var lastChar = from;
        GetBodyBetween(data, ref lastChar, start, end);

        while (from < lastChar)
        {
            var item = GetItem(data, ref from);
            if (item.Equals(Variable.EmptyInstance))
            {
                break;
            }

            args.Add(item);
        }


        MoveForwardIf(data, ref from, end, Constants.Space);

        return args;
    }

    public static string[] GetFunctionSignature(string data, ref int from)
    {
        MoveForwardIf(data, ref from, Constants.StartArg, Constants.Space);

        var endArgs = data.IndexOf(Constants.EndArg, from);
        if (endArgs < 0)
        {
            throw new ArgumentException("Couldn't extract function signature");
        }

        var argStr = data.Substring(from, endArgs - from);
        var args = argStr.Split(Constants.NextArgArray);

        from = endArgs + 1;

        return args;
    }

    public static int ExtractArrayElement(ref string varName)
    {
        var argStart = varName.IndexOf(Constants.StartArray);
        if (argStart <= 0)
        {
            return -1;
        }

        var argEnd = varName.IndexOf(Constants.EndArray, argStart + 1);
        if (argEnd <= argStart + 1)
        {
            return -1;
        }

        var getIndexFrom = argStart;
        MoveForwardIf(varName, ref getIndexFrom,
            Constants.StartArg, Constants.StartArray);

        var existing = Parser.LoadAndCalculate(varName, ref getIndexFrom,
            Constants.EndArrayArray);

        if (existing.Type == Variable.VarType.Number && existing.Value >= 0)
        {
            varName = varName.Substring(0, argStart);
            return (int)existing.Value;
        }

        return -1;
    }

    public static bool EndsWithFunction(string buffer, List<string> functions)
    {
        foreach (var key in functions)
        {
            if (buffer.EndsWith(key))
            {
                var prev = key.Length >= buffer.Length ? Constants.EndStatement : buffer[buffer.Length - key.Length - 1];
                if (Constants.TokenSeparation.Contains(prev))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool SpaceNotNeeded(char next)
    {
        return (next == Constants.Space || next == Constants.StartArg ||
                next == Constants.StartGroup || next == Constants.StartArray ||
                next == Constants.Empty);
    }

    public static bool KeepSpace(StringBuilder sb, char next)
    {
        if (SpaceNotNeeded(next))
        {
            return false;
        }

        return EndsWithFunction(sb.ToString(), Constants.FunctWithSpace);
    }

    public static bool KeepSpaceOnce(StringBuilder sb, char next)
    {
        if (SpaceNotNeeded(next))
        {
            return false;
        }

        return EndsWithFunction(sb.ToString(), Constants.FunctWithSpaceOnce);
    }

    public static string ConvertToScript(string source)
    {
        var sb = new StringBuilder(source.Length);

        var inQuotes = false;
        var spaceOk = false;
        var inComments = false;
        var previous = Constants.Empty;

        var parentheses = 0;
        var groups = 0;

        for (var i = 0; i < source.Length; i++)
        {
            var ch = source[i];
            var next = i + 1 < source.Length ? source[i + 1] : Constants.Empty;

            if (inComments && ch != '\n')
            {
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
                    if (inQuotes)
                    {
                        sb.Append(ch);
                    }
                    else
                    {
                        var keepSpace = KeepSpace(sb, next);
                        spaceOk = keepSpace ||
                                  (previous != Constants.Empty && previous != Constants.NextArg && spaceOk);
                        var spaceOKonce = KeepSpaceOnce(sb, next);
                        if (spaceOk || spaceOKonce)
                        {
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
                    spaceOk = false;
                    continue;
                case Constants.EndArg:
                    if (!inQuotes)
                    {
                        parentheses--;
                        spaceOk = false;
                    }

                    break;
                case Constants.StartArg:
                    if (!inQuotes)
                    {
                        parentheses++;
                    }

                    break;
                case Constants.EndGroup:
                    if (!inQuotes)
                    {
                        groups--;
                        spaceOk = false;
                    }

                    break;
                case Constants.StartGroup:
                    if (!inQuotes)
                    {
                        groups++;
                    }

                    break;
                case Constants.EndStatement:
                    if (!inQuotes)
                    {
                        spaceOk = false;
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
            throw new ArgumentException("Uneven parentheses " + Constants.StartArg + Constants.EndArg);
        }

        if (groups != 0)
        {
            throw new ArgumentException("Uneven groups " + Constants.StartGroup + Constants.EndGroup);
        }

        return sb.ToString();
    }

    public static string GetBodyBetween(string data, ref int from, char open, char close)
    {
        // We are supposed to be one char after the beginning of the string, i.e.
        // we must not have the opening char as the first one.
        var sb = new StringBuilder(data.Length);
        var braces = 0;

        for (; from < data.Length; from++)
        {
            var ch = data[from];

            if (string.IsNullOrEmpty(ch.ToString()) && sb.Length == 0)
            {
                continue;
            }
            else if (ch == open)
            {
                braces++;
            }
            else if (ch == close)
            {
                braces--;
            }

            sb.Append(ch);
            if (braces == -1)
            {
                if (ch == close)
                {
                    sb.Remove(sb.Length - 1, 1);
                }

                break;
            }
        }

        return sb.ToString();
    }


    public static string IsNotSign(string data)
    {
        return data.StartsWith(Constants.Not) ? Constants.Not : null;
    }

    public static string ValidAction(string data, int from)
    {
        if (from < 0 || from >= data.Length)
        {
            return null;
        }

        var action = StartsWith(data.Substring(from), Constants.Actions);
        return action;
    }

    public static string StartsWith(string data, string[] items)
    {
        foreach (var item in items)
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
        foreach (var ch in arr)
        {
            if (MoveForwardIf(data, ref from, ch))
            {
                return;
            }
        }
    }

    public static bool MoveForwardIf(string data, ref int from, char expected,
        char expected2 = Constants.Empty)
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
        var lines = GetItem(data, ref from);
        if (lines.Tuple == null)
        {
            throw new ArgumentException("Expected a list argument");
        }

        var sb = new StringBuilder(80 * lines.Tuple.Count);
        foreach (var line in lines.Tuple)
        {
            sb.AppendLine(line.String);
        }

        return sb.ToString();
    }
}
