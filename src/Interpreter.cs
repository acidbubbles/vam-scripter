using System;
using System.Text;

public class OutputAvailableEventArgs : EventArgs
{
    public string Output { get; set; }
}

public class Interpreter
{
    private static Interpreter _instance;

    private Interpreter()
    {
        Init();
    }

    public static Interpreter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Interpreter();
            }

            return _instance;
        }
    }

    private int _maxLoops = 10000;

    public void Init()
    {
        ParserFunction.AddGlobal(Constants.If, new IfStatement(this));
        ParserFunction.AddGlobal(Constants.While, new WhileStatement(this));
        ParserFunction.AddGlobal(Constants.Break, new BreakStatement());
        ParserFunction.AddGlobal(Constants.Continue, new ContinueStatement());
        ParserFunction.AddGlobal(Constants.Return, new ReturnStatement());
        ParserFunction.AddGlobal(Constants.Function, new FunctionCreator(this));
        ParserFunction.AddGlobal(Constants.Throw, new ThrowFunction());
        ParserFunction.AddGlobal(Constants.Try, new TryBlock(this));

        ParserFunction.AddGlobal(Constants.Abs, new AbsFunction());
        ParserFunction.AddGlobal(Constants.Acos, new AcosFunction());
        ParserFunction.AddGlobal(Constants.Asin, new AsinFunction());
        ParserFunction.AddGlobal(Constants.Ceil, new CeilFunction());
        ParserFunction.AddGlobal(Constants.Cos, new CosFunction());
        ParserFunction.AddGlobal(Constants.Exp, new ExpFunction());
        ParserFunction.AddGlobal(Constants.Floor, new FloorFunction());
        ParserFunction.AddGlobal(Constants.IndexOf, new IndexOfFunction());
        ParserFunction.AddGlobal(Constants.LOG, new LogFunction());
        ParserFunction.AddGlobal(Constants.PI, new PiFunction());
        ParserFunction.AddGlobal(Constants.Pow, new PowFunction());
        ParserFunction.AddGlobal(Constants.Round, new RoundFunction());
        ParserFunction.AddGlobal(Constants.Set, new SetVarFunction());
        ParserFunction.AddGlobal(Constants.Sin, new SinFunction());
        ParserFunction.AddGlobal(Constants.Size, new SizeFunction());
        ParserFunction.AddGlobal(Constants.Sqrt, new SqrtFunction());
        ParserFunction.AddGlobal(Constants.Substr, new SubstrFunction());
        ParserFunction.AddGlobal(Constants.Tolower, new ToLowerFunction());
        ParserFunction.AddGlobal(Constants.Toupper, new ToUpperFunction());

        ParserFunction.AddAction(Constants.Assignment, new AssignFunction());
        ParserFunction.AddAction(Constants.Increment, new IncrementDecrementFunction());
        ParserFunction.AddAction(Constants.Decrement, new IncrementDecrementFunction());

        for (var i = 0; i < Constants.OperActions.Length; i++)
        {
            ParserFunction.AddAction(Constants.OperActions[i], new OperatorAssignFunction());
        }

        Constants.ElseList.Add(Constants.Else);
        Constants.ElseIfList.Add(Constants.ElseIf);
        Constants.CatchList.Add(Constants.Catch);
    }

    public Variable Process(string script)
    {
        var data = Utils.ConvertToScript(script);
        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        var currentChar = 0;
        Variable result = null;

        while (currentChar < data.Length)
        {
            result = Parser.LoadAndCalculate(data, ref currentChar, Constants.EndParseArray);
            Utils.GoToNextStatement(data, ref currentChar);
        }

        return result;
    }

    internal Variable ProcessWhile(string data, ref int from)
    {
        var startWhileCondition = from;

        // A check against an infinite loop.
        var cycles = 0;
        var stillValid = true;

        while (stillValid)
        {
            from = startWhileCondition;

            //int startSkipOnBreakChar = from;
            var condResult = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
            stillValid = Convert.ToBoolean(condResult.Value);

            if (!stillValid)
            {
                break;
            }

            // Check for an infinite loop if we are comparing same values:
            if (_maxLoops > 0 && ++cycles >= _maxLoops)
            {
                throw new ArgumentException($"Looks like an infinite loop after {cycles} cycles.");
            }

            var result = ProcessBlock(data, ref from);
            if (result.Type == Variable.VarType.Break)
            {
                from = startWhileCondition;
                break;
            }
        }

        // The while condition is not true anymore: must skip the whole while
        // block before continuing with next statements.
        SkipBlock(data, ref from);
        return Variable.Undefined();
    }

    internal Variable ProcessIf(string data, ref int from)
    {
        var startIfCondition = from;

        var result = Parser.LoadAndCalculate(data, ref from, Constants.EndArgArray);
        var isTrue = Convert.ToBoolean(result.Value);

        if (isTrue)
        {
            result = ProcessBlock(data, ref from);

            if (result.Type == Variable.VarType.Break ||
                result.Type == Variable.VarType.Continue)
            {
                // Got here from the middle of the if-block. Skip it.
                from = startIfCondition;
                SkipBlock(data, ref from);
            }

            SkipRestBlocks(data, ref from);

            return result;
        }

        // We are in Else. Skip everything in the If statement.
        SkipBlock(data, ref from);

        var endOfToken = from;
        var nextToken = Utils.GetNextToken(data, ref endOfToken);

        if (Constants.ElseIfList.Contains(nextToken))
        {
            from = endOfToken + 1;
            result = ProcessIf(data, ref from);
        }
        else if (Constants.ElseList.Contains(nextToken))
        {
            from = endOfToken + 1;
            result = ProcessBlock(data, ref from);
        }

        return Variable.EmptyInstance;
    }

    internal Variable ProcessTry(string data, ref int from)
    {
        var startTryCondition = from - 1;
        var currentStackLevel = ParserFunction.GetCurrentStackLevel();
        Exception exception = null;

        Variable result = null;

        try
        {
            result = ProcessBlock(data, ref from);
        }
        catch (ArgumentException exc)
        {
            exception = exc;
        }

        if (exception != null ||
            result.Type == Variable.VarType.Break ||
            result.Type == Variable.VarType.Continue)
        {
            // Got here from the middle of the try-block either because
            // an exception was thrown or because of a Break/Continue. Skip it.
            from = startTryCondition;
            SkipBlock(data, ref from);
        }

        var catchToken = Utils.GetNextToken(data, ref from);
        from++; // skip opening parenthesis
        // The next token after the try block must be a catch.
        if (!Constants.CatchList.Contains(catchToken))
        {
            throw new ArgumentException($"Expecting a 'catch()' but got [{catchToken}]");
        }

        var exceptionName = Utils.GetNextToken(data, ref from);
        from++; // skip closing parenthesis

        if (exception != null)
        {
            var excStack = CreateExceptionStack(currentStackLevel);
            ParserFunction.InvalidateStacksAfterLevel(currentStackLevel);

            var excFunc = new GetVarFunction(Variable.CreateString(exception.Message + excStack));
            ParserFunction.AddGlobalOrLocalVariable(exceptionName, excFunc);

            result = ProcessBlock(data, ref from);
            ParserFunction.PopLocalVariable(exceptionName);
        }
        else
        {
            SkipBlock(data, ref from);
        }

        SkipRestBlocks(data, ref from);
        return result;
    }

    private static string CreateExceptionStack(int lowestStackLevel)
    {
        var result = "";
        var stack = ParserFunction.ExecutionStack;
        var level = stack.Count;
        foreach (var stackLevel in stack)
        {
            if (level-- < lowestStackLevel)
            {
                break;
            }

            if (string.IsNullOrEmpty(stackLevel.Name))
            {
                continue;
            }

            result += $"\r\n  {stackLevel.Name}()";
        }

        if (!string.IsNullOrEmpty(result))
        {
            result = $" at{result}";
        }

        return result;
    }

    private Variable ProcessBlock(string data, ref int from)
    {
        var blockStart = from;
        Variable result = null;

        while (from < data.Length)
        {
            var endGroupRead = Utils.GoToNextStatement(data, ref from);
            if (endGroupRead > 0)
            {
                return result ?? Variable.Undefined();
            }

            if (from >= data.Length)
            {
                throw new ArgumentException($"Couldn't process block [{data.Substring(blockStart)}]");
            }

            result = Parser.LoadAndCalculate(data, ref from, Constants.EndParseArray);


            if (result.Type == Variable.VarType.Break ||
                result.Type == Variable.VarType.Continue)
            {
                return result;
            }
        }

        return result;
    }

    private void SkipBlock(string data, ref int from)
    {
        var blockStart = from;
        var startCount = 0;
        var endCount = 0;
        while (startCount == 0 || startCount > endCount)
        {
            if (from >= data.Length)
            {
                throw new ArgumentException($"Couldn't skip block [{data.Substring(blockStart)}]");
            }

            var currentChar = data[from++];
            switch (currentChar)
            {
                case Constants.StartGroup:
                    startCount++;
                    break;
                case Constants.EndGroup:
                    endCount++;
                    break;
            }
        }

        if (startCount != endCount)
        {
            throw new ArgumentException("Mismatched parentheses");
        }
    }

    private void SkipRestBlocks(string data, ref int from)
    {
        while (from < data.Length)
        {
            var endOfToken = from;
            var nextToken = Utils.GetNextToken(data, ref endOfToken);
            if (!Constants.ElseIfList.Contains(nextToken) &&
                !Constants.ElseList.Contains(nextToken))
            {
                return;
            }

            from = endOfToken;
            SkipBlock(data, ref from);
        }
    }
}
