using System;

namespace kio_windows_integration.Exceptions
{
    public class WindowsIntegrationFocusApiException : Exception
    {
        public WindowsIntegrationFocusApiException() : base()
        {
        }

        public WindowsIntegrationFocusApiException(string msg) : base(msg)
        {
        }

        public WindowsIntegrationFocusApiException(string msg, Exception inner) : base(msg, inner)
        {
        }
    }
}