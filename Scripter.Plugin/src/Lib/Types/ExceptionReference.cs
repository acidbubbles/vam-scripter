using System;

namespace ScripterLang
{
    public class ExceptionReference : ObjectReference
    {
        private readonly Exception _exception;

        public ExceptionReference(Exception exception)
        {
            _exception = exception;
        }

        public override Value GetProperty(string name)
        {
            switch (name)
            {
                case "message":
                    return Value.CreateString(_exception.Message);
                case "stack":
                    return Value.CreateString(_exception.StackTrace);
                default:
                    return Value.Undefined;
            }
        }
    }
}
