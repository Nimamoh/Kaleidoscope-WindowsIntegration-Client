using System;

namespace kaleidoscope_companion.Events
{
    /// <summary>
    /// Sent when a connection attempt on the keybaord has failed
    /// </summary>
    public class FailureOnKeyboardConnect
    {
        public FailureOnKeyboardConnect(string msg, Exception inner = null)
        {

            Message = msg;
            Inner = inner;
        }

        public Exception Inner { get; }

        public string Message { get; }
    }
}