using System;
using kio_windows_integration.Annotations;

namespace kio_windows_integration.Events
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

        [CanBeNull]
        public Exception Inner { get; }

        public string Message { get; }
    }
}