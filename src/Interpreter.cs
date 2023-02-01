using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
	public class OutputAvailableEventArgs : EventArgs
	{
		public string Output { get; set; }
	}

	public class Interpreter
	{

		private static Interpreter instance;

		private Interpreter()
		{
			Init();
		}

		public static Interpreter Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new Interpreter();
				}
				return instance;
			}
		}

		private int MAX_LOOPS;

		private StringBuilder m_output = new StringBuilder();
		public string Output {
			get {
				string output = m_output.ToString().Trim();
				m_output.Clear();
				return output;
			}
		}

		public event EventHandler<OutputAvailableEventArgs> GetOutput;

		public void AppendOutput(string text, bool newLine = true)
		{
			EventHandler<OutputAvailableEventArgs> handler = GetOutput;
			if (handler != null)
			{
				OutputAvailableEventArgs args = new OutputAvailableEventArgs();
				args.Output = text + (newLine ? Environment.NewLine : string.Empty);
				handler(this, args);
			}
		}

		public void Init()
		{
			ParserFunction.AddGlobal(Constants.IF,          new IfStatement(this));
			ParserFunction.AddGlobal(Constants.WHILE,       new WhileStatement(this));
			ParserFunction.AddGlobal(Constants.BREAK,       new BreakStatement());
			ParserFunction.AddGlobal(Constants.CONTINUE,    new ContinueStatement());
			ParserFunction.AddGlobal(Constants.RETURN,      new ReturnStatement());
			ParserFunction.AddGlobal(Constants.FUNCTION,    new FunctionCreator(this));
			ParserFunction.AddGlobal(Constants.INCLUDE,     new IncludeFile());
			ParserFunction.AddGlobal(Constants.THROW,       new ThrowFunction());
			ParserFunction.AddGlobal(Constants.TRY,         new TryBlock(this));

			ParserFunction.AddGlobal(Constants.ABS,         new AbsFunction());
			ParserFunction.AddGlobal(Constants.ACOS,        new AcosFunction());
			ParserFunction.AddGlobal(Constants.APPEND,      new AppendFunction());
			ParserFunction.AddGlobal(Constants.APPENDLINE,  new AppendLineFunction());
			ParserFunction.AddGlobal(Constants.APPENDLINES, new AppendLinesFunction());
			ParserFunction.AddGlobal(Constants.ASIN,        new AsinFunction());
			ParserFunction.AddGlobal(Constants.CD,          new CdFunction());
			ParserFunction.AddGlobal(Constants.CD__,        new Cd__Function());
			ParserFunction.AddGlobal(Constants.CEIL,        new CeilFunction());
			ParserFunction.AddGlobal(Constants.CONNECTSRV,  new ClientSocket(this));
			ParserFunction.AddGlobal(Constants.COPY,        new CopyFunction());
			ParserFunction.AddGlobal(Constants.COS,         new CosFunction());
			ParserFunction.AddGlobal(Constants.DELETE,      new DeleteFunction());
			ParserFunction.AddGlobal(Constants.DIR,         new DirFunction(this));
			ParserFunction.AddGlobal(Constants.ENV,         new GetEnvFunction());
			ParserFunction.AddGlobal(Constants.EXISTS,      new ExistsFunction());
			ParserFunction.AddGlobal(Constants.EXP,         new ExpFunction());
			ParserFunction.AddGlobal(Constants.FINDFILES,   new FindfilesFunction(this));
			ParserFunction.AddGlobal(Constants.FINDSTR,     new FindstrFunction(this));
			ParserFunction.AddGlobal(Constants.FLOOR,       new FloorFunction());
			ParserFunction.AddGlobal(Constants.INDEX_OF,    new IndexOfFunction());
      ParserFunction.AddGlobal(Constants.KILL,        new KillFunction(this));
      ParserFunction.AddGlobal(Constants.LOG,         new LogFunction());
			ParserFunction.AddGlobal(Constants.MKDIR,       new MkdirFunction());
			ParserFunction.AddGlobal(Constants.MORE,        new MoreFunction());
			ParserFunction.AddGlobal(Constants.MOVE,        new MoveFunction());
			ParserFunction.AddGlobal(Constants.PI,          new PiFunction());
			ParserFunction.AddGlobal(Constants.POW,         new PowFunction());
			ParserFunction.AddGlobal(Constants.PSINFO,      new PsInfoFunction(this));
			ParserFunction.AddGlobal(Constants.PSTIME,      new ProcessorTimeFunction());
			ParserFunction.AddGlobal(Constants.PWD,         new PwdFunction(this));
      ParserFunction.AddGlobal(Constants.READ,        new ReadConsole());
      ParserFunction.AddGlobal(Constants.READFILE,    new ReadFileFunction(this));
      ParserFunction.AddGlobal(Constants.READNUMBER,  new ReadConsole(true));
			ParserFunction.AddGlobal(Constants.ROUND,       new RoundFunction());
			ParserFunction.AddGlobal(Constants.RUN,         new RunFunction(this));
			ParserFunction.AddGlobal(Constants.SET,         new SetVarFunction());
			ParserFunction.AddGlobal(Constants.SETENV,      new SetEnvFunction());
			ParserFunction.AddGlobal(Constants.SIN,         new SinFunction());
			ParserFunction.AddGlobal(Constants.SIZE,        new SizeFunction());
			ParserFunction.AddGlobal(Constants.SQRT,        new SqrtFunction());
			ParserFunction.AddGlobal(Constants.STARTSRV,    new ServerSocket(this));
			ParserFunction.AddGlobal(Constants.SUBSTR,      new SubstrFunction());
			ParserFunction.AddGlobal(Constants.TAIL,        new TailFunction());
			ParserFunction.AddGlobal(Constants.TOLOWER,     new ToLowerFunction());
			ParserFunction.AddGlobal(Constants.TOUPPER,     new ToUpperFunction());
      ParserFunction.AddGlobal(Constants.WRITE,       new PrintFunction(this, false));
			ParserFunction.AddGlobal(Constants.WRITELINE,   new WriteLineFunction());
			ParserFunction.AddGlobal(Constants.WRITELINES,  new WriteLinesFunction());
      ParserFunction.AddGlobal(Constants.WRITENL,     new PrintFunction(this, true));

			ParserFunction.AddAction(Constants.ASSIGNMENT,  new AssignFunction());
			ParserFunction.AddAction(Constants.INCREMENT,   new IncrementDecrementFunction());
			ParserFunction.AddAction(Constants.DECREMENT,   new IncrementDecrementFunction());

			for (int i = 0; i < Constants.OPER_ACTIONS.Length; i++) {
				ParserFunction.AddAction(Constants.OPER_ACTIONS[i], new OperatorAssignFunction());
			}

			Constants.ELSE_LIST.Add(Constants.ELSE);
			Constants.ELSE_IF_LIST.Add(Constants.ELSE_IF);
			Constants.CATCH_LIST.Add(Constants.CATCH);

			ReadConfig();
		}

		public void ReadConfig()
		{
			MAX_LOOPS         = ReadConfig("maxLoops", 256000);

			if (ConfigurationManager.GetSection ("Languages") == null) {
				return;
			}
			var languagesSection = ConfigurationManager.GetSection("Languages") as NameValueCollection;
			if (languagesSection.Count == 0)
			{
				return;
			}

			string languages = languagesSection["languages"];
			string[] supportedLanguages = languages.Split(",".ToCharArray());

			foreach(string language in supportedLanguages)
			{
				var languageSection    = ConfigurationManager.GetSection(language) as NameValueCollection;

				AddTranslation(languageSection, Constants.IF);
				AddTranslation(languageSection, Constants.WHILE);
				AddTranslation(languageSection, Constants.BREAK);
				AddTranslation(languageSection, Constants.CONTINUE);
				AddTranslation(languageSection, Constants.RETURN);
				AddTranslation(languageSection, Constants.FUNCTION);
				AddTranslation(languageSection, Constants.INCLUDE);
				AddTranslation(languageSection, Constants.THROW);
				AddTranslation(languageSection, Constants.TRY);

				AddTranslation(languageSection, Constants.APPEND);
				AddTranslation(languageSection, Constants.APPENDLINE);
				AddTranslation(languageSection, Constants.APPENDLINES);
				AddTranslation(languageSection, Constants.CD);
				AddTranslation(languageSection, Constants.CD__);
				AddTranslation(languageSection, Constants.CEIL);
				AddTranslation(languageSection, Constants.COPY);
				AddTranslation(languageSection, Constants.DELETE);
				AddTranslation(languageSection, Constants.DIR);
				AddTranslation(languageSection, Constants.ENV);
				AddTranslation(languageSection, Constants.EXISTS);
				AddTranslation(languageSection, Constants.FINDFILES);
				AddTranslation(languageSection, Constants.FINDSTR);
				AddTranslation(languageSection, Constants.FLOOR);
				AddTranslation(languageSection, Constants.INDEX_OF);
				AddTranslation(languageSection, Constants.KILL);
				AddTranslation(languageSection, Constants.MKDIR);
				AddTranslation(languageSection, Constants.MORE);
				AddTranslation(languageSection, Constants.MOVE);
 				AddTranslation(languageSection, Constants.PSINFO);
				AddTranslation(languageSection, Constants.PWD);
        AddTranslation(languageSection, Constants.READ);
        AddTranslation(languageSection, Constants.READFILE);
        AddTranslation(languageSection, Constants.READNUMBER);
				AddTranslation(languageSection, Constants.ROUND);
				AddTranslation(languageSection, Constants.RUN);
				AddTranslation(languageSection, Constants.SET);
				AddTranslation(languageSection, Constants.SETENV);
				AddTranslation(languageSection, Constants.SIZE);
				AddTranslation(languageSection, Constants.SUBSTR);
				AddTranslation(languageSection, Constants.TAIL);
				AddTranslation(languageSection, Constants.TOLOWER);
				AddTranslation(languageSection, Constants.TOUPPER);
        AddTranslation(languageSection, Constants.WRITE);
				AddTranslation(languageSection, Constants.WRITELINE);
				AddTranslation(languageSection, Constants.WRITELINES);
        AddTranslation(languageSection, Constants.WRITENL);

				// Special dealing for else, elif since they are not separate
				// functions but are part of the if statement block.
				// Same for and, or, not.
				AddSubstatementTranslation(languageSection, Constants.ELSE,    Constants.ELSE_LIST);
				AddSubstatementTranslation(languageSection, Constants.ELSE_IF, Constants.ELSE_IF_LIST);
				AddSubstatementTranslation(languageSection, Constants.CATCH,   Constants.CATCH_LIST);
			}
		}

		public int ReadConfig(string configName, int defaultValue = 0)
		{
			string config = ConfigurationManager.AppSettings[configName];
			int value = defaultValue;
			if (string.IsNullOrWhiteSpace(config) || !Int32.TryParse(config, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public void AddTranslation(NameValueCollection languageDictionary, string originalName)
		{
			string translation = languageDictionary[originalName];
			if (string.IsNullOrWhiteSpace(translation))
			{ // The translation is not provided for this function.
				return;
			}

			if (translation.IndexOfAny((" \t\r\n").ToCharArray()) >= 0)
			{
				throw new ArgumentException("Translation of [" + translation + "] contains white spaces");
			}

			ParserFunction originalFunction = ParserFunction.GetFunction(originalName);
			ParserFunction.AddGlobal(translation, originalFunction);

			// If the list of functions after which there can be a space (becides a parenthesis)
			// contains the original function, also add the translation to the list.
			if (Constants.FUNCT_WITH_SPACE.Contains(originalName)) {
				Constants.FUNCT_WITH_SPACE.Add(translation);
			}
		}

		public void AddSubstatementTranslation(NameValueCollection languageDictionary,
			string originalName, List<string> keywordsArray)
		{
			string translation = languageDictionary[originalName];
			if (string.IsNullOrWhiteSpace(translation))
			{ // The translation is not provided for this sub statement.
				return;
			}

			if (translation.IndexOfAny((" \t\r\n").ToCharArray()) >= 0)
			{
				throw new ArgumentException("Translation of [" + translation + "] contains white spaces");
			}

			keywordsArray.Add(translation);
		}

		public Variable Process(string script)
		{
			string data = Utils.ConvertToScript(script);
      if (string.IsNullOrWhiteSpace (data)) {
				return null;
			}

			int currentChar = 0;
			Variable result = null;

      while (currentChar < data.Length)
			{
        result = Parser.LoadAndCalculate(data, ref currentChar, Constants.END_PARSE_ARRAY);
        Utils.GoToNextStatement(data, ref currentChar);
			}

			return result;
		}

		internal Variable ProcessWhile(string data, ref int from)
		{
			int startWhileCondition = from;

			// A check against an infinite loop.
			int cycles = 0;
			bool stillValid = true;

			while (stillValid)
			{
				from = startWhileCondition;

				//int startSkipOnBreakChar = from;
				Variable condResult = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
				stillValid = Convert.ToBoolean(condResult.Value);

				if (!stillValid)
				{
					break;
				}

				// Check for an infinite loop if we are comparing same values:
				if (MAX_LOOPS > 0 && ++cycles >= MAX_LOOPS)
				{
					throw new ArgumentException("Looks like an infinite loop after " +
						cycles + " cycles.");
				}

				Variable result = ProcessBlock(data, ref from);
        if (result.Type == Variable.VarType.BREAK)
				{
          from = startWhileCondition;
					break;
				}
			}

			// The while condition is not true anymore: must skip the whole while
			// block before continuing with next statements.
			SkipBlock(data, ref from);
			return new Variable();
		}

		internal Variable ProcessIf(string data, ref int from)
		{
			int startIfCondition = from;

			Variable result = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			bool isTrue = Convert.ToBoolean(result.Value);

			if (isTrue)
			{
				result = ProcessBlock(data, ref from);

        if (result.Type == Variable.VarType.BREAK ||
            result.Type == Variable.VarType.CONTINUE)
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

			int endOfToken = from;
			string nextToken = Utils.GetNextToken(data, ref endOfToken);

			if (Constants.ELSE_IF_LIST.Contains(nextToken))
			{
				from = endOfToken + 1;
				result = ProcessIf(data, ref from);
			}
			else if (Constants.ELSE_LIST.Contains(nextToken))
			{
				from = endOfToken + 1;
				result = ProcessBlock(data, ref from);
			}

      return Variable.EmptyInstance;
		}

		internal Variable ProcessTry(string data, ref int from)
		{
			int startTryCondition = from - 1;
			int currentStackLevel = ParserFunction.GetCurrentStackLevel();
			Exception exception   = null;

			Variable result = null;

			try {
				result = ProcessBlock(data, ref from);
			}
			catch(ArgumentException exc) {
				exception = exc;
			}

if (exception != null ||
    result.Type == Variable.VarType.BREAK ||
    result.Type == Variable.VarType.CONTINUE)
{
	// Got here from the middle of the try-block either because
	// an exception was thrown or because of a Break/Continue. Skip it.
	from = startTryCondition;
	SkipBlock(data, ref from);
}

			string catchToken = Utils.GetNextToken(data, ref from);
			from++; // skip opening parenthesis
			// The next token after the try block must be a catch.
			if (!Constants.CATCH_LIST.Contains(catchToken))
			{
				throw new ArgumentException("Expecting a 'catch()' but got [" +
					catchToken + "]");
			}

			string exceptionName = Utils.GetNextToken(data, ref from);
			from++; // skip closing parenthesis

			if (exception != null) {
				string excStack = CreateExceptionStack(currentStackLevel);
				ParserFunction.InvalidateStacksAfterLevel(currentStackLevel);

				GetVarFunction excFunc = new GetVarFunction(new Variable(exception.Message + excStack));
				ParserFunction.AddGlobalOrLocalVariable(exceptionName, excFunc);

				result = ProcessBlock(data, ref from);
				ParserFunction.PopLocalVariable(exceptionName);
			} else {
				SkipBlock (data, ref from);
			}
				
			SkipRestBlocks(data, ref from);
			return result;
		}

		private static string CreateExceptionStack(int lowestStackLevel) {
			string result = "";
			Stack<ParserFunction.StackLevel> stack = ParserFunction.ExecutionStack;
			int level = stack.Count;
			foreach (ParserFunction.StackLevel stackLevel in stack) {
				if (level-- < lowestStackLevel) {
					break;
				}
				if (string.IsNullOrWhiteSpace(stackLevel.Name)) {
					continue;
				}
				result += Environment.NewLine + "  " + stackLevel.Name + "()";
			}

			if (!string.IsNullOrWhiteSpace (result)) {
				result = " at" + result;
			}

			return result;
		}

		private Variable ProcessBlock(string data, ref int from)
		{
			int blockStart = from;
			Variable result = null;

			while(from < data.Length)
			{
				int endGroupRead = Utils.GoToNextStatement(data, ref from);
				if (endGroupRead > 0)
				{
					return result != null ? result : new Variable();
				}

				if (from >= data.Length)
				{
					throw new ArgumentException("Couldn't process block [" +
						data.Substring(blockStart) + "]");
				}

				result = Parser.LoadAndCalculate(data, ref from, Constants.END_PARSE_ARRAY);


        if (result.Type == Variable.VarType.BREAK ||
            result.Type == Variable.VarType.CONTINUE) {
				 return result;
				}
			}
			return result;
		}

		private void SkipBlock(string data, ref int from)
		{
			int blockStart = from;
			int startCount = 0;
			int endCount = 0;
			while (startCount == 0 || startCount > endCount)
			{
				if (from >= data.Length)
				{
					throw new ArgumentException("Couldn't skip block [" + data.Substring(blockStart) + "]");
				}
				char currentChar = data[from++];
				switch (currentChar)
				{
				case Constants.START_GROUP: startCount++; break;
				case Constants.END_GROUP:   endCount++; break;
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
				int endOfToken = from;
				string nextToken = Utils.GetNextToken(data, ref endOfToken);
				if (!Constants.ELSE_IF_LIST.Contains(nextToken) &&
					!Constants.ELSE_LIST.Contains(nextToken))
				{
					return;
				}
				from = endOfToken;
				SkipBlock(data, ref from);
			}
		}
	}
}
