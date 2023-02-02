using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SplitAndMerge
{
    public partial class Utils
    {
        public static Func<string, string> GetFileContentsDelegate { get; set; }

        public static char[] SPACESEP = new char[] { ' ' };

        public static void CheckArgs(int args, int expected, string msg, bool exactMatch = false)
        {
            if (args < expected || (exactMatch && args != expected))
            {
                throw new ArgumentException("Expecting " + expected +
                    " arguments but got " + args + " in " + msg);
            }
        }
        public static void CheckPosInt(Variable variable, ParsingScript script = null)
        {
            CheckInteger(variable, script);
            if (variable.Value <= 0)
            {
                ThrowErrorMsg("Expected a positive integer instead of [" +
                              variable.Value + "].", script, script == null ? "" : script.Current.ToString());
            }
        }

        public static void CheckNonNegativeInt(Variable variable, ParsingScript script = null)
        {
            CheckInteger(variable, script);
            if (variable.Value < 0)
            {
                ThrowErrorMsg("Expected a non-negative integer instead of [" +
                              variable.Value + "].", script, script == null ? "" : script.Current.ToString());
            }
        }
        public static void CheckInteger(Variable variable, ParsingScript script = null)
        {
            CheckNumber(variable, script);
            if (variable.Value % 1 != 0.0)
            {
                ThrowErrorMsg("Expected an integer instead of [" +
                              variable.Value + "].", script, script == null ? "" : script.Current.ToString());
            }
        }
        public static void CheckNumber(Variable variable, ParsingScript script = null)
        {
            if (variable.Type != Variable.VarType.NUMBER)
            {
                ThrowErrorMsg("Expected a number instead of [" +
                              variable.AsString() + "].", script, script == null ? "" : script.Current.ToString());
            }
        }
        public static void CheckArray(Variable variable, string name)
        {
            if (variable.Tuple == null)
            {
                string realName = Constants.GetRealName(name);
                throw new ArgumentException("An array expected for variable [" +
                                               realName + "]");
            }
        }
        public static void CheckNotEmpty(ParsingScript script, string varName, string name)
        {
            if (!script.StillValid() || string.IsNullOrWhiteSpace(varName))
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Incomplete arguments for [" + realName + "].", script, name);
            }
        }

        public static void CheckNotNull(object obj, string name, ParsingScript script, int index = -1)
        {
            if (obj == null)
            {
                string indexStr = index >= 0 ? " in position " + (index + 1) : "";
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Invalid argument " + indexStr +
                                            " in function [" + realName + "].", script, name);
            }
        }
        public static void CheckNotNull(string name, ParserFunction func, ParsingScript script)
        {
            if (func == null)
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Variable or function [" + realName + "] doesn't exist.", script, name);
            }
        }
        public static void CheckNotNull(object obj, string name, ParsingScript script)
        {
            if (obj == null)
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Object [" + realName + "] doesn't exist.", script, name);
            }
        }

        public static void CheckNotEnd(ParsingScript script, string name)
        {
            if (!script.StillValid())
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Incomplete arguments for [" + realName + "]", script, script.Prev.ToString());
            }
        }
        public static void CheckNotEnd(ParsingScript script)
        {
            if (!script.StillValid())
            {
                ThrowErrorMsg("Incomplete function definition.", script, script.Prev.ToString());
            }
        }

        public static void CheckNotEmpty(string varName, string name)
        {
            if (string.IsNullOrEmpty(varName))
            {
                string realName = Constants.GetRealName(name);
                ThrowErrorMsg("Incomplete arguments for [" + realName + "]", null, name);
            }
        }

        public static void CheckForValidName(string name, ParsingScript script)
        {
            if (string.IsNullOrWhiteSpace(name) || (!Char.IsLetter(name[0]) && name[0] != '_'))
            {
                ThrowErrorMsg("Illegal variable name: [" + name + "]",
                              script, name);
            }

            string illegals = "\"'?!";
            int first = name.IndexOfAny(illegals.ToCharArray());
            if (first >= 0)
            {
                var ind = name.IndexOf('[');
                if (ind < 0 || ind > first)
                {
                    for (int i = 0; i < illegals.Length; i++)
                    {
                        char ch = illegals[i];
                        if (name.Contains(ch))
                        {
                            ThrowErrorMsg("Variable [" + name + "] contains illegal character [" + ch + "]",
                                          script, name);
                        }
                    }
                }
            }
        }

        public static string TrimString1(string text, int maxSize = 15, bool addQoutes = false, bool addDots = true)
        {
            if (text.Length <= maxSize)
            {
                return text;
            }
            var parts = text.Split(SPACESEP, 2);
            var friendlyName = TrimString(parts.First(), maxSize, addQoutes, addDots);
            return friendlyName;
        }

        public static string TrimString(string text, int maxSize = 15, bool addQoutes = false, bool addDots = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            text = text.Split('\n')[0].Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            if (text.Length > maxSize)
            {
                text = text.Substring(0, maxSize - 3) + (addDots ? "..." : "");
            }
            if (addQoutes)
            {
                text = "\"" + text + "\"";
            }

            return text;
        }

        public static void ThrowErrorMsg(string msg, ParsingScript script, string token)
        {
            string code = script == null || string.IsNullOrWhiteSpace(script.OriginalScript) ? "" : script.OriginalScript;
            int lineNumber = script == null ? 0 : script.OriginalLineNumber;
            int minLines = script == null || script.OriginalLine.ToLower().Contains(token.ToLower()) ? 1 : 2;

            ThrowErrorMsg(msg, code, lineNumber, minLines);
        }

        static void ThrowErrorMsg(string msg, string script, int lineNumber, int minLines = 1)
        {
            string[] lines = script.Split('\n');
            lineNumber = lines.Length <= lineNumber ? -1 : lineNumber;
            System.Diagnostics.Debug.WriteLine(msg);
            if (lineNumber < 0)
            {
                throw new ParsingException(msg);
            }

            var currentLineNumber = lineNumber;
            var line = lines[lineNumber].Trim();
            var collectMore = line.Length < 3 || minLines > 1;
            var lineContents = line;

            while (collectMore && currentLineNumber > 0)
            {
                line = lines[--currentLineNumber].Trim();
                collectMore = line.Length < 2 || (minLines > lineNumber - currentLineNumber + 1);
                lineContents = line + "  " + lineContents;
            }

            if (lines.Length > 1)
            {
                string lineStr = currentLineNumber == lineNumber ? "Line " + (lineNumber + 1) :
                                 "Lines " + (currentLineNumber + 1) + "-" + (lineNumber + 1);
                msg += " " + lineStr + ": " + lineContents;
            }

            StringBuilder stack = new StringBuilder();
            stack.AppendLine("" + currentLineNumber);
            stack.AppendLine(line);

            throw new ParsingException(msg, stack.ToString());
        }

        static void ThrowErrorMsg(string msg, string code, int level, int lineStart, int lineEnd, string filename)
        {
            var lineNumber = level > 0 ? lineStart : lineEnd;
            ThrowErrorMsg(msg, code, lineNumber);
        }

        public static bool CheckLegalName(string name, ParsingScript script = null, bool throwError = true)
        {
            if (string.IsNullOrWhiteSpace(name) || Constants.CheckReserved(name))
            {
                if (!throwError)
                {
                    return false;
                }
                Utils.ThrowErrorMsg(name + " is a reserved name.", script, name);
            }
            if (Char.IsDigit(name[0]) || name[0] == '-')
            {
                if (!throwError)
                {
                    return false;
                }
                Utils.ThrowErrorMsg(name + " has illegal first character " + name[0], null, name);
            }

            return true;
        }

        public static ParsingScript GetTempScript(Interpreter interpreter, string str, ParserFunction.StackLevel stackLevel,
            string name = "",
            ParsingScript script = null, ParsingScript parentScript = null,
            int parentOffset = 0)
        {
            ParsingScript tempScript = new ParsingScript(interpreter, str);
            tempScript.ScriptOffset = parentOffset;
            if (parentScript != null)
            {
                tempScript.Char2Line = parentScript.Char2Line;
                tempScript.OriginalScript = parentScript.OriginalScript;
            }
            tempScript.ParentScript = script;
            tempScript.Context = script == null ? null : script.Context;
            tempScript.InTryBlock = script == null ? false : script.InTryBlock;
            tempScript.StackLevel = stackLevel;

            return tempScript;
        }

        public static bool ExtractParameterNames(List<Variable> args, string functionName, ParsingScript script)
        {
            CustomFunction custFunc = script.InterpreterInstance.GetFunction(functionName) as CustomFunction;
            if (custFunc == null)
            {
                return false;
            }

            var realArgs = custFunc.RealArgs;
            for (int i = 0; i < args.Count && i < realArgs.Length; i++)
            {
                string name = args[i].CurrentAssign;
                args[i].ParamName = string.IsNullOrWhiteSpace(name) ? realArgs[i] : name;
            }
            return true;
        }

        public static int GetSafeInt(List<Variable> args, int index, int defaultValue = 0)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            Variable numberVar = args[index];
            if (numberVar.Type != Variable.VarType.NUMBER)
            {
                if (string.IsNullOrWhiteSpace(numberVar.String))
                {
                    return defaultValue;
                }
                int num;
                if (!Int32.TryParse(numberVar.String, NumberStyles.Number,
                                     CultureInfo.InvariantCulture, out num))
                {
                    throw new ArgumentException("Expected an integer instead of [" + numberVar.AsString() + "]");
                }
                return num;
            }
            return numberVar.AsInt();
        }
        public static double GetSafeDouble(List<Variable> args, int index, double defaultValue = 0.0)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }

            Variable numberVar = args[index];
            if (numberVar.Type != Variable.VarType.NUMBER)
            {
                double num;
                if (!CanConvertToDouble(numberVar.String, out num))
                {
                    throw new ArgumentException("Expected a double instead of [" + numberVar.AsString() + "]");
                }
                return num;
            }
            return numberVar.AsDouble();
        }

        public static string GetSafeString(List<Variable> args, int index, string defaultValue = "")
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            return args[index].AsString();
        }
        public static Variable GetSafeVariable(List<Variable> args, int index, Variable defaultValue = null)
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }
            return args[index];
        }

        public static string GetSafeToken(List<Variable> args, int index, string defaultValue = "")
        {
            if (args.Count <= index)
            {
                return defaultValue;
            }

            Variable var = args[index];
            string token = var.ParsingToken;

            return token;
        }

        public static Variable GetVariable(string varName, ParsingScript script = null, bool testNull = true)
        {
            if (script == null)
            {
                script = new ParsingScript(script.InterpreterInstance, "");
            }
            varName = varName.ToLower();

            ParserFunction func = script.InterpreterInstance.GetVariable(varName, script);
            if (!testNull && func == null)
            {
                return null;
            }
            Utils.CheckNotNull(varName, func, script);
            Variable varValue = func.GetValue(script);
            Utils.CheckNotNull(varValue, varName, script);
            return varValue;
        }

        public static double ConvertToDouble(object obj, ParsingScript script = null)
        {
            string str = obj.ToString().ToLower();
            double num = 0;

            if (!CanConvertToDouble(str, out num) &&
                script != null)
            {
                ProcessErrorMsg(str, script);
            }
            return num;
        }

        public static bool CanConvertToDouble(string str, out double num)
        {
            if (str == "true")
            {
                num = 1.0;
                return true;
            }
            if (str == "false")
            {
                num = 0.0;
                return true;
            }
            return Double.TryParse(str, NumberStyles.Number |
                                        NumberStyles.AllowExponent |
                                        NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out num);
        }

        public static void ProcessErrorMsg(string str, ParsingScript script)
        {
            char ch = script.TryPrev();
            string entity = ch == '(' ? "function" :
                            ch == '[' ? "array" :
                            ch == '{' ? "operand" :
                                        "variable";
            string token = Constants.GetRealName(str);

            string msg = "Couldn't find " + entity + " [" + token + "].";

            ThrowErrorMsg(msg, script, str);
        }

        public static bool ConvertToBool(object obj)
        {
            string str = obj.ToString();
            double dRes = 0;
            if (CanConvertToDouble(str, out dRes))
            {
                return dRes != 0;
            }
            bool res = false;

            Boolean.TryParse(str, out res);
            return res;
        }
        public static int ConvertToInt(object obj, ParsingScript script = null)
        {
            double num = ConvertToDouble(obj, script);
            return (int)num;
        }

        public static void Extract(string data, ref string str1, ref string str2,
                                   ref string str3, ref string str4, ref string str5)
        {
            string[] vals = data.Split(new char[] { ',', ':' });
            str1 = vals[0];
            if (vals.Length > 1)
            {
                str2 = vals[1];
                if (vals.Length > 2)
                {
                    str3 = vals[2];
                    if (vals.Length > 3)
                    {
                        str4 = vals[3];
                        if (vals.Length > 4)
                        {
                            str5 = vals[4];
                        }
                    }
                }
            }
        }
        public static int GetNumberOfDigits(string data, int itemNumber = -1)
        {
            if (itemNumber >= 0)
            {
                string[] vals = data.Split(new char[] { ',', ':' });
                if (vals.Length <= itemNumber)
                {
                    return 0;
                }
                int min = 0;
                for (int i = 0; i < vals.Length; i++)
                {
                    min = Math.Max(min, GetNumberOfDigits(vals[i]));
                }
                return min;
            }

            int index = data.IndexOf(".");
            if (index < 0 || index >= data.Length - 1)
            {
                return 0;
            }
            return data.Length - index - 1;
        }
        public static void Extract(string data, ref double val1, ref double val2,
                                                ref double val3, ref double val4)
        {
            string[] vals = data.Split(new char[] { ',', ':' });
            val1 = ConvertToDouble(vals[0].Trim());

            if (vals.Length > 1)
            {
                val2 = ConvertToDouble(vals[1].Trim());
                if (vals.Length > 2)
                {
                    val3 = ConvertToDouble(vals[2].Trim());
                }
                if (vals.Length > 3)
                {
                    val4 = ConvertToDouble(vals[3].Trim());
                }
            }
            else
            {
                val3 = val2 = val1;
            }
        }

        public static string RemovePrefix(string text)
        {
            string candidate = text.Trim().ToLower();
            if (candidate.Length > 2 && candidate.StartsWith("l'",
                          StringComparison.OrdinalIgnoreCase))
            {
                return candidate.Substring(2).Trim();
            }

            int firstSpace = candidate.IndexOf(' ');
            if (firstSpace <= 0)
            {
                return candidate;
            }

            string prefix = candidate.Substring(0, firstSpace);
            if (prefix.Length == 3 && candidate.Length > 4 &&
               (prefix == "der" || prefix == "die" || prefix == "das" ||
                prefix == "los" || prefix == "las" || prefix == "les"))
            {
                return candidate.Substring(firstSpace + 1);
            }
            if (prefix.Length == 2 && candidate.Length > 3 &&
               (prefix == "el" || prefix == "la" || prefix == "le" ||
                prefix == "il" || prefix == "lo"))
            {
                return candidate.Substring(firstSpace + 1);
            }
            return candidate;
        }

        public static void ExtendArrayIfNeeded<T>(List<T> array, int count, T defaultValue)
        {
            if (array.Count <= count)
            {
                array.Capacity = count + 1;
                while (array.Count <= count)
                {
                    array.Add(defaultValue);
                }
            }
        }

        static public byte[] TruncateArray(byte[] array, int bytesReceived = 0)
        {
            int i = array.Length - 1;
            while (i >= 0 && array[i] == 0 && i >= bytesReceived)
            {
                --i;
            }
            byte[] newArray = new byte[i + 1];
            Array.Copy(array, newArray, i + 1);
            return newArray;
        }
    }
}
