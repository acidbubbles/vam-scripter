namespace SplitAndMerge
{
    public partial class Interpreter
    {
        public void InitStandalone()
        {
            RegisterFunction(Constants.GOTO, new GotoGosubFunction(true));
            RegisterFunction(Constants.GOSUB, new GotoGosubFunction(false));

            AddAction(Constants.LABEL_OPERATOR, new LabelFunction());
        }
   }
}
