using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SplitAndMerge
{
    public class Statics
    {
        public static string StringVar = "";
        public static double DoubleVar = 0.0;
        public static bool   BoolVar   = false;
        public static int    IntVar    = 0;

        static Dictionary<string, Func<string, string>> m_compiledCode =
           new Dictionary<string, Func<string, string>>();

        public static Variable InvokeCall(Type type, string methodName, string paramName,
                                          string paramValue, object master = null)
        {
            string key = type + "_" + methodName + "_" + paramName;
            Func<string, string> func = null;

            // Cache compiled function:
            if (!m_compiledCode.TryGetValue(key, out func))
            {
                MethodInfo methodInfo = type.GetMethod(methodName, new Type[] { typeof(string) });
                ParameterExpression param = Expression.Parameter(typeof(string), paramName);

                MethodCallExpression methodCall = master == null ? Expression.Call(methodInfo, param) :
                                                             Expression.Call(Expression.Constant(master), methodInfo, param);
                Expression<Func<string, string>> lambda =
                    Expression.Lambda<Func<string, string>>(methodCall, new ParameterExpression[] { param });
                func = lambda.Compile();
                m_compiledCode[key] = func;
            }

            string result = func(paramValue);
            return new Variable(result);
        }

        public static Object GetVariableValue(string name, ParsingScript script)
        {
            var field = typeof(Statics).GetField(name);
            Utils.CheckNotNull(field, name, script);
            Object result = field.GetValue(null);
            return result;
        }

        public static bool SetVariableValue(string name, Object value, ParsingScript script)
        {
            Type type   = typeof(Statics);
            var props   = type.GetProperties();
            var members = type.GetMembers();
            var methods = type.GetMethods();
            var fields  = type.GetFields();
            var field   = type.GetField(name);
            Utils.CheckNotNull(field, name, script);
            field.SetValue(null, Convert.ChangeType(value, field.FieldType));
            return true;
        }

        public static string ProcessClick(string arg)
        {
            var now = DateTime.Now.ToString("T");
            return "Clicks: " + arg + "\n" + now;
        }
    }
}
