using System;

namespace kaleidoscope_companion.Helpers
{
    public interface IErrorHandler
    {
        void Handle(Exception e);
    }
}