using System;

namespace kaleidoscope_companion.Exceptions
{
    public class WindowsIntegrationFocusApiException : Exception
    {
        public WindowsIntegrationFocusApiException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}