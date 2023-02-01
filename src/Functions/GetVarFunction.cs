using System;

public class GetVarFunction : ParserFunction
{
    internal GetVarFunction(Variable value)
    {
        _mValue = value;
    }

    protected override Variable Evaluate(string data, ref int from)
    {
        // First check if this element is part of an array:
        if (from - 1 < data.Length && data[from - 1] == Constants.StartArray)
        {
            // There is an index given - it may be for an element of the tuple.
            if (_mValue.Tuple == null || _mValue.Tuple.Count == 0)
            {
                throw new ArgumentException("No tuple exists for the index");
            }

            var index = Parser.LoadAndCalculate(data, ref from,
                Constants.EndArrayArray);

            //Variable index = Utils.GetItem(data, ref from);
            Utils.CheckInteger(index);

            if (index.Value < 0 || index.Value >= _mValue.Tuple.Count)
            {
                throw new ArgumentException($"Incorrect index [{index.Value}] for tuple of size {_mValue.Tuple.Count}");
            }

            Utils.MoveForwardIf(data, ref from, Constants.EndArray);
            return _mValue.Tuple[(int)index.Value];
        }

        return _mValue;
    }

    private readonly Variable _mValue;
}