using System;

namespace SplitAndMerge
{
	class PiFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			return new Variable(Math.PI);
		}
	}

	class ExpFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable result = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			result.Value = Math.Exp(result.Value);
			return result;
		}
	}

	class PowFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg1 = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_ARG_ARRAY);
			from++; // eat separation
			Variable arg2 = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);

			arg1.Value = Math.Pow(arg1.Value, arg2.Value);
			return arg1;
		}
	}

	class SinFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Sin(arg.Value);
			return arg;
		}
	}

	class CosFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Cos(arg.Value);
			return arg;
		}
	}

	class AsinFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Asin(arg.Value);
			return arg;
		}
	}

	class AcosFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Acos(arg.Value);
			return arg;
		}
	}

	class SqrtFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Sqrt(arg.Value);
			return arg;
		}
	}

	class AbsFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Abs(arg.Value);
			return arg;
		}
	}

	class CeilFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Ceiling(arg.Value);
			return arg;
		}
	}

	class FloorFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Floor(arg.Value);
			return arg;
		}
	}

	class RoundFunction : ParserFunction
	{
		protected override Variable Evaluate(string data, ref int from)
		{
			Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
			arg.Value = Math.Round(arg.Value);
			return arg;
		}
	}

  class LogFunction : ParserFunction
  {
    protected override Variable Evaluate(string data, ref int from)
    {
      Variable arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
      arg.Value = Math.Log(arg.Value);
      return arg;
    }
  }

}
