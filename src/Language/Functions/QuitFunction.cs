using System.Collections.Generic;

namespace SplitAndMerge
{
    class QuitFunction : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            var code = Utils.GetSafeInt(args, 0, 0);
            return QuitScript(script, code);
        }

        public static Variable QuitScript(ParsingScript script, int exitCode = 0)
        {
            var interpreter = script.InterpreterInstance;
            interpreter.ExitCode = exitCode;
            interpreter.IsRunning = false;

            if (script.StackLevel != null)
            {
                interpreter.PopLocalVariables(script.StackLevel.Id);
                script.StackLevel = null;
            }
            script.CurrentModule = "";
            script.SetDone();

            return Variable.EmptyInstance;// new Variable(Variable.VarType.QUIT);
        }

        public override string Description()
        {
            return "Quits scripting engine without terminating the process. Stops Debugger if attached.";
        }
    }
}
