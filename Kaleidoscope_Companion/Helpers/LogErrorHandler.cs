using System;
using log4net;

namespace kaleidoscope_companion.Helpers
{
    public class LogErrorHandler: IErrorHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LogErrorHandler));

        public void Handle(Exception e)
        {
            Log.Error("An unexpected error occured: ", e);
        }
    }
}