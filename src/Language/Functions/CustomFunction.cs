using System;
using System.Collections.Generic;

namespace SplitAndMerge
{
    public class CustomFunction : ParserFunction
    {
        internal CustomFunction(string funcName,
            string body, string[] args, ParsingScript script)
        {
            InterpreterInstance = script.InterpreterInstance;
            Name = funcName;
            m_body = body;
            m_args = RealArgs = args;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                int ind = arg.IndexOf('=');
                if (ind > 0)
                {
                    RealArgs[i] = arg.Substring(0, ind).Trim();
                    m_args[i] = RealArgs[i].ToLower();
                    string defValue = ind >= arg.Length - 1 ? "" : arg.Substring(ind + 1).Trim();

                    Variable defVariable = Utils.GetVariableFromString(defValue, script);
                    defVariable.CurrentAssign = m_args[i];
                    defVariable.Index = i;

                    m_defArgMap[i] = m_defaultArgs.Count;
                    m_defaultArgs.Add(defVariable);
                }
                else
                {
                    m_args[i] = RealArgs[i].ToLower();
                }

                ArgMap[m_args[i]] = i;
            }
        }

        public void RegisterArguments(List<Variable> args,
            List<KeyValuePair<string, Variable>> args2 = null)
        {
            if (args == null)
            {
                args = new List<Variable>();
            }
            int missingArgs = m_args.Length - args.Count;

            bool namedParameters = false;
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                int argIndex = -1;
                if (ArgMap.TryGetValue(arg.CurrentAssign, out argIndex))
                {
                    namedParameters = true;
                    if (i != argIndex)
                    {
                        args[i] = argIndex < args.Count ? args[argIndex] : args[i];
                        while (argIndex > args.Count - 1)
                        {
                            args.Add(Variable.EmptyInstance);
                        }
                        args[argIndex] = arg;
                    }
                }
                else if (namedParameters)
                {
                    throw new ArgumentException("All arguments in function [" + m_name +
                                                "] must be in arg=value form.");
                }
            }

            if (missingArgs > 0 && missingArgs <= m_defaultArgs.Count)
            {
                if (!namedParameters)
                {
                    for (int i = m_defaultArgs.Count - missingArgs; i < m_defaultArgs.Count; i++)
                    {
                        args.Add(m_defaultArgs[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (args[i].Type == Variable.VarType.NONE ||
                            (!string.IsNullOrWhiteSpace(args[i].CurrentAssign) &&
                             args[i].CurrentAssign != m_args[i]))
                        {
                            int defIndex = -1;
                            if (!m_defArgMap.TryGetValue(i, out defIndex))
                            {
                                throw new ArgumentException("No argument [" + m_args[i] +
                                                            "] given for function [" + m_name + "].");
                            }
                            args[i] = m_defaultArgs[defIndex];
                        }
                    }
                }
            }
            for (int i = args.Count; i < m_args.Length; i++)
            {
                int defIndex = -1;
                if (!m_defArgMap.TryGetValue(i, out defIndex))
                {
                    throw new ArgumentException("No argument [" + m_args[i] +
                                                "] given for function [" + m_name + "].");
                }
                args.Add(m_defaultArgs[defIndex]);
            }
            m_stackLevel = new StackLevel(m_name);

            if (args2 != null)
            {
                foreach (var entry in args2)
                {
                    var arg = new GetVarFunction(entry.Value);
                    arg.Name = entry.Key;
                    m_stackLevel.Variables[entry.Key] = arg;
                }
            }

            int maxSize = Math.Min(args.Count, m_args.Length);
            for (int i = 0; i < maxSize; i++)
            {
                var arg = new GetVarFunction(args[i]);
                arg.Name = m_args[i];
                m_stackLevel.Variables[m_args[i]] = arg;
            }

            for (int i = m_args.Length; i < args.Count; i++)
            {
                var arg = new GetVarFunction(args[i]);
                m_stackLevel.Variables[args[i].ParamName.ToLower()] = arg;
            }

            if (NamespaceData  != null)
            {
                var vars = NamespaceData.Variables;
                string prefix = NamespaceData.Name + ".";
                foreach (KeyValuePair<string, ParserFunction> elem in vars)
                {
                    string key = elem.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ?
                        elem.Key.Substring(prefix.Length) : elem.Key;
                    m_stackLevel.Variables[key] = elem.Value;
                }
            }

            InterpreterInstance.AddLocalVariables(m_stackLevel);
        }

        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();

            Utils.ExtractParameterNames(args, m_name, script);

            script.MoveBackIf(Constants.START_GROUP);

            if (args.Count + m_defaultArgs.Count < m_args.Length)
            {
                throw new ArgumentException("Function [" + m_name + "] arguments mismatch: " +
                                            m_args.Length + " declared, " + args.Count + " supplied");
            }

            Variable result = Run(args, script);
            return result;
        }


        public Variable Run(List<Variable> args = null, ParsingScript script = null)
        {
            // 1. Add passed arguments as local variables to the Parser.
            RegisterArguments(args, null);

            // 2. Execute the body of the function.
            Variable result = null;
            ParsingScript tempScript = Utils.GetTempScript(InterpreterInstance,
                m_body, m_stackLevel, m_name, script,
                m_parentScript, m_parentOffset);

            while (tempScript.Pointer < m_body.Length - 1 &&
                   (result == null || !result.IsReturn))
            {
                result = tempScript.Execute();
                tempScript.GoToNextStatement();
            }

            InterpreterInstance.PopLocalVariables(m_stackLevel.Id);

            if (result == null)
            {
                result = Variable.EmptyInstance;
            }
            else
            {
                result.IsReturn = false;
            }

            return result;
        }

        public static Variable Run(Interpreter interpreter, string functionName,
            Variable arg1 = null, Variable arg2 = null, Variable arg3 = null, ParsingScript script = null)
        {
            CustomFunction customFunction = interpreter.GetFunction(functionName) as CustomFunction;

            if (customFunction == null)
            {
                return null;
            }

            List<Variable> args = new List<Variable>();
            if (arg1 != null)
            {
                args.Add(arg1);
            }
            if (arg2 != null)
            {
                args.Add(arg2);
            }
            if (arg3 != null)
            {
                args.Add(arg3);
            }

            Variable result = customFunction.Run(args, script);
            return result;
        }

        public static Variable Run(Interpreter interpreter, string functionName,
            List<Variable> args, ParsingScript script = null)
        {
            CustomFunction customFunction = interpreter.GetFunction(functionName) as CustomFunction;

            if (customFunction == null)
            {
                return null;
            }

            Variable result = customFunction.Run(args, script);
            return result;
        }

        public override ParserFunction NewInstance()
        {
            var newInstance = (CustomFunction)this.MemberwiseClone();
            newInstance.m_stackLevel = null;
            return newInstance;
        }

        public ParsingScript ParentScript { set { m_parentScript = value; } }
        public int ParentOffset { set { m_parentOffset = value; } }
        public string Body { get { return m_body; } }

        public int ArgumentCount { get { return m_args.Length; } }
        public string Argument(int nIndex) { return m_args[nIndex]; }

        public StackLevel NamespaceData { get; set; }

        public int DefaultArgsCount
        {
            get
            {
                return m_defaultArgs.Count;
            }
        }

        public string Header
        {
            get
            {
                return Constants.FUNCTION + " " + Constants.GetRealName(Name) + " " +
                       Constants.START_ARG + string.Join(", ", m_args) +
                       Constants.END_ARG + " " + Constants.START_GROUP;
            }
        }

        protected string m_body;
        protected string[] m_args;
        protected ParsingScript m_parentScript = null;
        protected int m_parentOffset = 0;
        protected StackLevel m_stackLevel;

        List<Variable> m_defaultArgs = new List<Variable>();
        Dictionary<int, int> m_defArgMap = new Dictionary<int, int>();

        public Dictionary<string, int> ArgMap { get; private set; } = new Dictionary<string, int>();
        public string[] RealArgs { get; private set; }
    }
}
