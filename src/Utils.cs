using System;
using System.Collections.Generic;

public partial class Utils
{
    public static void CheckArgs(int args, int expected, string msg)
    {
        if (args < expected)
        {
            throw new ArgumentException("Expecting " + expected +
                                        " arguments but got " + args + " in " + msg);
        }
    }

    public static void CheckPosInt(Variable variable)
    {
        CheckInteger(variable);
        if (variable.Value <= 0)
        {
            throw new ArgumentException("Expecting a positive integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckNonNegativeInt(Variable variable)
    {
        CheckInteger(variable);
        if (variable.Value < 0)
        {
            throw new ArgumentException("Expecting a non  negative integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckInteger(Variable variable)
    {
        CheckNumber(variable);
        if (variable.Value % 1 != 0)
        {
            throw new ArgumentException("Expecting an integer instead of [" +
                                        variable.Value + "]");
        }
    }

    public static void CheckNumber(Variable variable)
    {
        if (variable.Type != Variable.VarType.Number)
        {
            throw new ArgumentException("Expecting a number instead of [" +
                                        variable.String + "]");
        }
    }

    public static void PrintList(List<Variable> list, int from)
    {
        Console.Write("Merging list:");
        for (var i = from; i < list.Count; i++)
        {
            Console.Write(" ({0}, '{1}')", list[i].Value, list[i].Action);
        }

        Console.WriteLine();
    }
}
