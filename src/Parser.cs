using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
	public class Parser
	{
		public static bool Verbose { get; set; }

		public static Variable LoadAndCalculate(string data, ref int from, char[] to)
		{
			// First step: process passed expression by splitting it into a list of cells.
			List<Variable> listToMerge = Calculate(data, ref from, to);

			if (listToMerge.Count == 0) {
				throw new ArgumentException("Couldn't parse [" +
					   data.Substring (from) + "]");
			}

			// If there is just one resulting cell there is no need
			// to perform the second step to merge tokens.
			if (listToMerge.Count == 1) {
				return listToMerge[0];
			}

			Variable baseCell = listToMerge[0];
			int index = 1;

			// Second step: merge list of cells to get the result of an expression.
			Variable result = Merge(baseCell, ref index, listToMerge);
			return result;
		}

		private static List<Variable> Calculate(string data, ref int from, char[] to)
		{
			List<Variable> listToMerge = new List<Variable>(16);

			if (from >= data.Length || to.Contains(data[from]))
			{
				listToMerge.Add(Variable.EmptyInstance);
				return listToMerge;
			}

			StringBuilder item = new StringBuilder();
			int negated = 0;
			bool inQuotes = false;

			do
			{ // Main processing cycle of the first part.
				string negateSymbol = Utils.IsNotSign(data.Substring(from));
				if (negateSymbol != null)
				{
					negated++;
					from += negateSymbol.Length;
					continue;
				}

        char ch = data[from++];
				inQuotes = ch == Constants.QUOTE && (from == 0 || data[from] != '\\') ? !inQuotes : inQuotes;
				string action = null;

				bool keepCollecting = inQuotes || StillCollecting(item.ToString(), to, data, from, ref action);
				if (keepCollecting)
				{ // The char still belongs to the previous operand.
					item.Append(ch);

					if (from < data.Length && (inQuotes || !to.Contains(data[from])))
					{
						continue;
					}
				}

				string parsingItem = item.ToString();

        CheckConsistency (parsingItem, listToMerge, data, from);

				Utils.MoveForwardIf(data, ref from, Constants.SPACE);

				if (action != null && action.Length > 1)
				{
					from += (action.Length - 1);
				}

				// We are done getting the next token. The getValue() call below may
				// recursively call loadAndCalculate(). This will happen if extracted
				// item is a function or if the next item is starting with a START_ARG '('.
				ParserFunction func = new ParserFunction(data, ref from, parsingItem, ch, ref action);
        Variable current = func.GetValue(data, ref from);

        if (negated > 0 && current.Type == Variable.VarType.NUMBER)
				{
					// If there has been a NOT sign, this is a boolean.
					// Use XOR (true if exactly one of the arguments is true).
					bool boolRes = !((negated % 2 == 0) ^ Convert.ToBoolean(current.Value));
					current = new Variable(Convert.ToDouble(boolRes));
					negated = 0;
				}

				if (action == null)
				{
					action = UpdateAction(data, ref from, to);
				}


				char next = from < data.Length ? data[from] : Constants.EMPTY;
				bool done = listToMerge.Count == 0 &&
					       ((action == Constants.END_ARG_STR &&
                   current.Type != Variable.VarType.NUMBER) ||
				          next == Constants.END_STATEMENT );
				if (done)
				{ // If there is no numerical result, we are not in a math expression.
					listToMerge.Add(current);
					return listToMerge;
				}

				Variable cell = new Variable(current);
				cell.Action = action;
				listToMerge.Add(cell);
				item.Clear();
      } while (from < data.Length && (inQuotes || !to.Contains(data[from])));

			// This happens when called recursively inside of the math expression:
			Utils.MoveForwardIf(data, ref from, Constants.END_ARG);

			return listToMerge;
		}

    private static void CheckConsistency(string item, List<Variable> listToMerge,
                                         string data, int from)
    {
      if (Constants.CONTROL_FLOW.Contains(item) && listToMerge.Count > 0) {
        // This can happen when the end of statement ";" is forgotten.



        throw new ArgumentException ("Token [" +
          item + "] can't be part of an expression. Check \";\". Stopped at [" +
            data.Substring (from - 1, Constants.MAX_ERROR_CHARS) + " ...]");
      }
    }

    private static bool StillCollecting(string item, char[] to, string data, int from,
			                        ref string action)
		{
			char ch   = from > 0 ? data[from - 1] : Constants.EMPTY;
      char next = from < data.Length ? data[from] : Constants.EMPTY;
      char prev = from > 1 ? data[from - 2] : Constants.EMPTY;

			if (to.Contains(ch) || ch == Constants.START_ARG ||
				                   ch == Constants.START_GROUP ||
				                 next == Constants.EMPTY)
			{
				return false;
			}

			// Case of a negative number, or starting with the closing bracket:
      if (item.Length == 0 &&
         ((ch == '-' && next != '-') || ch == Constants.END_ARG))
			{
				return true;
			}

      // Case of a scientific notation 1.2e+5 or 1.2e-5 or 1e5:
      if (Char.ToUpper(prev) == 'E' &&
         (ch == '-' || ch == '+' || Char.IsDigit(ch)) &&
         item.Length > 1 && Char.IsDigit(item[item.Length - 2]))
      {
        return true;
      }

      // Otherwise if it's an action (+, -, *, etc.) or a space
      // we're done collecting current token.
			if ((action = Utils.ValidAction(data, from - 1)) != null ||
			  	(item.Length > 0 && ch == Constants.SPACE))
			{
				return false;
			}

			return true;
		}

		private static string UpdateAction(string data, ref int from, char[] to)
		{
			// We search a valid action till we get to the End of Argument ')'
			// or pass the end of string.
			if (from >= data.Length || data[from] == Constants.END_ARG ||
				to.Contains(data[from]))
			{
				return Constants.END_ARG.ToString();
			}

			// Start searching from the previous character.
			int index = from;// - 1;

			string action = Utils.ValidAction(data, index);

			//while (action == null && index < data.Length)
			while (action == null && index < data.Length &&
				   data [index] == Constants.END_ARG)
 			{// Look for the next character in string until a valid action is found.
				action = Utils.ValidAction (data, ++index);

				//if (index >= data.Length || data [index] != Constants.END_ARG) {
				//	break;
				//}
			}

			// We need to advance forward not only the action length but also all
			// the characters we skipped before getting the action.
			int advance = action == null ? 0 : action.Length + Math.Max(0, index - from);
			from += advance;
			return action == null ? Constants.END_ARG.ToString() : action;
		}

		// From outside this function is called with mergeOneOnly = false.
		// It also calls itself recursively with mergeOneOnly = true, meaning
		// that it will return after only one merge.
		private static Variable Merge(Variable current, ref int index, List<Variable> listToMerge,
			                          bool mergeOneOnly = false)
		{
			if (Verbose)
			{
				Utils.PrintList(listToMerge, index - 1);
			}

			while (index < listToMerge.Count)
			{
				Variable next = listToMerge[index++];

				while (!CanMergeCells(current, next))
				{ // If we cannot merge cells yet, go to the next cell and merge
					// next cells first. E.g. if we have 1+2*3, we first merge next
					// cells, i.e. 2*3, getting 6, and then we can merge 1+6.
					Merge(next, ref index, listToMerge, true /* mergeOneOnly */);
				}

				MergeCells(current, next);
				if (mergeOneOnly)
				{
					break;
				}
			}

			if (Verbose)
			{
				Console.WriteLine("Calculated: {0} {1}",
					              current.Value, current.String);
			}
			return current;
		}

		private static void MergeCells(Variable leftCell, Variable rightCell)
		{
      if (leftCell.Type == Variable.VarType.BREAK ||
          leftCell.Type == Variable.VarType.CONTINUE) {
      // Done!
        return;
      }
      if (leftCell.Type == Variable.VarType.NUMBER ||
         rightCell.Type == Variable.VarType.NUMBER) {
				MergeNumbers(leftCell, rightCell);
			}
			else
			{
				MergeStrings(leftCell, rightCell);
			}

			leftCell.Action = rightCell.Action;
		}

		private static void MergeNumbers(Variable leftCell, Variable rightCell)
		{
      if (leftCell.Action != "+" &&
          rightCell.Type != Variable.VarType.NUMBER)
			{
				throw new ArgumentException("Can't merge a number " +
          leftCell.Value + " with [" + rightCell.AsString() + "]");
			}
			switch (leftCell.Action)
			{
			case "^": leftCell.Value = Math.Pow(leftCell.Value, rightCell.Value);
				break;
			case "%": leftCell.Value %= rightCell.Value;
				break;
			case "*": leftCell.Value *= rightCell.Value;
				break;
			case "/":
				if (rightCell.Value == 0)
				{
					throw new ArgumentException("Division by zero");
				}
				leftCell.Value /= rightCell.Value;
				break;
			case "+":
        if (rightCell.Type != Variable.VarType.NUMBER) {
			      leftCell.String = leftCell.AsString() + rightCell.String;
				} else {
				    leftCell.Value += rightCell.Value;
				}
				break;
			case "-":  leftCell.Value -= rightCell.Value;
				break;
			case "<":  leftCell.Value = Convert.ToDouble(leftCell.Value < rightCell.Value);
				break;
			case ">":  leftCell.Value = Convert.ToDouble(leftCell.Value > rightCell.Value);
				break;
			case "<=": leftCell.Value = Convert.ToDouble(leftCell.Value <= rightCell.Value);
				break;
			case ">=": leftCell.Value = Convert.ToDouble(leftCell.Value >= rightCell.Value);
				break;
			case "==": leftCell.Value = Convert.ToDouble(leftCell.Value == rightCell.Value);
				break;
			case "!=": leftCell.Value = Convert.ToDouble(leftCell.Value != rightCell.Value);
				break;
			case "&&": leftCell.Value = Convert.ToDouble(
				Convert.ToBoolean(leftCell.Value) && Convert.ToBoolean(rightCell.Value));
				break;
			case "||": leftCell.Value = Convert.ToDouble(
				Convert.ToBoolean(leftCell.Value) || Convert.ToBoolean(rightCell.Value));
				break;
			}
		}

		private static void MergeStrings(Variable leftCell, Variable rightCell)
		{

			switch (leftCell.Action)
			{
			case "+":  leftCell.String += rightCell.AsString();
				break;
			case "<":  leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) < 0);
				break;
			case ">":  leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) > 0);
				break;
			case "<=": leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) <= 0);
				break;
			case ">=": leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) >= 0);
				break;
			case "==": leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) == 0);
				break;
			case "!=": leftCell.Value = Convert.ToDouble(
				       string.Compare(leftCell.String, rightCell.String) != 0);
				break;
			default:

        throw new ArgumentException("Can't perform action [" +
					leftCell.Action + "] on strings");

			}
		}

		private static bool CanMergeCells(Variable leftCell, Variable rightCell)
		{
			return GetPriority(leftCell.Action) >= GetPriority(rightCell.Action);
		}

		private static int GetPriority(string action)
		{
			switch (action)
			{
				case "++":
				case "--": return 10;
				case "^" : return 9;
				case "%" :
				case "*" :
				case "/" : return 8;
				case "+" :
				case "-" : return 7;
				case "<" :
				case ">" :
				case ">=":
				case "<=": return 6;
				case "==":
				case "!=": return 5;
				case "&&": return 4;
				case "||": return 3;
				case "+=":
				case "=" : return 2;
			}
			return 0;
		}
	}
}
