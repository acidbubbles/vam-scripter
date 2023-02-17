using System;

namespace ScripterLang
{
    public class TryCatchExpression : Expression
    {
        private readonly CodeBlockExpression _tryBlock;
        private readonly CodeBlockExpression _catchBlock;
        private readonly CodeBlockExpression _finallyBlock;
        private readonly VariableReference _catchVariable;

        public TryCatchExpression(CodeBlockExpression tryBlock, CodeBlockExpression catchBlock, CodeBlockExpression finallyBlock, VariableReference catchVariable)
        {
            _tryBlock = tryBlock;
            _catchBlock = catchBlock;
            _finallyBlock = finallyBlock;
            _catchVariable = catchVariable;
        }

        public override void Bind()
        {
            _tryBlock.Bind();
            if (_catchVariable != null)
                _catchVariable.bound = true;
            _catchBlock?.Bind();
            _finallyBlock?.Bind();
        }

        public override Value Evaluate()
        {
            try
            {
                _tryBlock.Evaluate();
            }
            catch (Exception e)
            {
                _catchVariable?.Initialize(new ExceptionReference(e));
                _catchBlock?.Evaluate();
            }
            finally
            {
                _catchVariable?.Clear();
                _finallyBlock?.Evaluate();
            }

            return Value.Void;
        }
    }
}
