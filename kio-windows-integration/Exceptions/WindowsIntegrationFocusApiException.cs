using System;

namespace kio_windows_integration.Exceptions
{
    public class WindowsIntegrationFocusApiException : Exception
    {
        public WindowsIntegrationFocusApiException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}