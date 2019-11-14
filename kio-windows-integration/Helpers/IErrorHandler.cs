using System;

namespace kio_windows_integration.Helpers
{
    public interface IErrorHandler
    {
        void Handle(Exception e);
    }
}