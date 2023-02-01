using System;
using System.Collections.Generic;

public class ParserFunction
{
    public ParserFunction()
    {
        _mImpl = this;
    }

    // A "virtual" Constructor
    internal ParserFunction(string data, ref int from, string item, char ch, ref string action)
    {
        if (item.Length == 0 && (ch == Constants.StartArg || from >= data.Length))
        {
            // There is no function, just an expression in parentheses
            _mImpl = _sIDFunction;
            return;
        }

        _mImpl = GetArrayFunction(item, ref from, action);
        if (_mImpl != null)
        {
            return;
        }

        _mImpl = GetFunctionOrAction(item, ref action);

        if (_mImpl == _sStrOrNumFunction && string.IsNullOrEmpty(item))
        {
            var problem = (!string.IsNullOrEmpty(action) ? action : ch.ToString());
            var restData = ch.ToString() +
                           data.Substring(from, Math.Min(data.Length - from - 1, Constants.MaxErrorChars));
            throw new ArgumentException("Couldn't parse [" + problem + "] in " + restData + "...");
        }
    }

    public static ParserFunction GetArrayFunction(string name, ref int from, string action)
    {
        if (!string.IsNullOrEmpty(action))
        {
            return null;
        }

        var arrayStart = name.IndexOf(Constants.StartArray);
        if (arrayStart <= 0)
        {
            return null;
        }

        var origLength = name.Length;
        var arrayIndex = Utils.ExtractArrayElement(ref name);
        if (arrayIndex < 0)
        {
            return null;
        }

        var pf = GetFunction(name);
        if (pf == null)
        {
            return null;
        }

        from -= (origLength - arrayStart - 1);
        return pf;
    }

    public static ParserFunction GetFunctionOrAction(string item, ref string action)
    {
        var actionFunction = GetAction(action);

        // If passed action exists and is registered we are done.
        if (actionFunction != null)
        {
            var theAction = actionFunction.NewInstance() as ActionFunction;
            theAction.Name = item;
            theAction.Action = action;

            action = null;
            return theAction;
        }

        // Otherwise look for local and global functions.
        var pf = GetFunction(item);

        if (pf != null)
        {
            return pf;
        }

        // Function not found, will try to parse this as a string in quotes or a number.
        _sStrOrNumFunction.Item = item;
        return _sStrOrNumFunction;
    }

    public static ParserFunction GetFunction(string item)
    {
        ParserFunction impl;
        // First search among local variables.

        if (_sLocals.Count > 0)
        {
            var local = _sLocals.Peek().Variables;
            if (local.TryGetValue(item, out impl))
            {
                // Local function exists (a local variable)
                return impl;
            }
        }

        if (_sFunctions.TryGetValue(item, out impl))
        {
            // Global function exists and is registered (e.g. pi, exp, or a variable)
            return impl.NewInstance();
        }

        return null;
    }

    public static ActionFunction GetAction(string action)
    {
        if (string.IsNullOrEmpty(action))
        {
            return null;
        }

        ActionFunction impl;
        if (_sActions.TryGetValue(action, out impl))
        {
            // Action exists and is registered (e.g. =, +=, --, etc.)
            return impl;
        }

        return null;
    }

    public static bool FunctionExists(string item)
    {
        var exists = false;
        // First check if the local function stack has this variable defined.
        if (_sLocals.Count > 0)
        {
            var local = _sLocals.Peek().Variables;
            exists = local.ContainsKey(item);
        }

        // If it is not defined locally, then check globally:
        return exists || _sFunctions.ContainsKey(item);
    }

    public static void AddGlobalOrLocalVariable(string name, ParserFunction function)
    {
        function.Name = name;
        if (_sLocals.Count > 0)
        {
            AddLocalVariable(function);
        }
        else
        {
            AddGlobal(name, function);
        }
    }

    public static void AddGlobal(string name, ParserFunction function)
    {
        _sFunctions[name] = function;
        function.Name = name;
    }

    public static void AddAction(string name, ActionFunction action)
    {
        _sActions[name] = action;
    }

    public static void AddLocalVariables(StackLevel locals)
    {
        _sLocals.Push(locals);
    }

    public static void AddStackLevel(string name)
    {
        _sLocals.Push(new StackLevel(name));
    }

    public static void AddLocalVariable(ParserFunction local)
    {
        StackLevel locals = null;
        if (_sLocals.Count == 0)
        {
            locals = new StackLevel();
            _sLocals.Push(locals);
        }
        else
        {
            locals = _sLocals.Peek();
        }

        locals.Variables[local.Name] = local;
    }

    public static void PopLocalVariables()
    {
        _sLocals.Pop();
    }

    public static int GetCurrentStackLevel()
    {
        return _sLocals.Count;
    }

    public static void InvalidateStacksAfterLevel(int level)
    {
        while (_sLocals.Count > level)
        {
            _sLocals.Pop();
        }
    }

    public static void PopLocalVariable(string name)
    {
        if (_sLocals.Count == 0)
        {
            return;
        }

        var locals = _sLocals.Peek().Variables;
        locals.Remove(name);
    }

    public Variable GetValue(string data, ref int from)
    {
        return _mImpl.Evaluate(data, ref from);
    }

    protected virtual Variable Evaluate(string data, ref int from)
    {
        // The real implementation will be in the derived classes.
        return new Variable();
    }

    public virtual ParserFunction NewInstance()
    {
        return this;
    }

    protected string MName;

    public string Name
    {
        get { return MName; }
        set { MName = value; }
    }

    private readonly ParserFunction _mImpl;

    // Global functions:
    private static readonly Dictionary<string, ParserFunction> _sFunctions = new Dictionary<string, ParserFunction>();

    // Global actions - function:
    private static readonly Dictionary<string, ActionFunction> _sActions = new Dictionary<string, ActionFunction>();

    public class StackLevel
    {
        public StackLevel(string name = null)
        {
            Name = name;
            Variables = new Dictionary<string, ParserFunction>();
        }

        public string Name { get; set; }
        public Dictionary<string, ParserFunction> Variables { get; set; }
    }

    // Local variables:
    // Stack of the functions being executed:
    private static readonly Stack<StackLevel> _sLocals = new Stack<StackLevel>();

    public static Stack<StackLevel> ExecutionStack
    {
        get { return _sLocals; }
    }

    private static readonly StringOrNumberFunction _sStrOrNumFunction =
        new StringOrNumberFunction();

    private static readonly IdentityFunction _sIDFunction =
        new IdentityFunction();
}

public abstract class ActionFunction : ParserFunction
{
    protected string MAction;

    public string Action
    {
        set { MAction = value; }
    }
}
