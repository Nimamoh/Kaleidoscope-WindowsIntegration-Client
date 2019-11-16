using System;
using System.Threading.Tasks;

namespace kaleidoscope_companion.Helpers
{
    public static class AsyncHelpers_Extensions
    {
        /// <summary>
        /// Let you fire and forget tasks with an error handler
        /// to prevent exception leaking.
        /// see: https://johnthiriet.com/removing-async-void/#
        /// </summary>
        public static async void SafeFireAndForget(this Task task, IErrorHandler handler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                handler.Handle(e);
            }
        }

        /// <see cref="SafeFireAndForget(System.Threading.Tasks.Task,IErrorHandler)"/>
        public static async void SafeFireAndForget(this Task task, Action<Exception> handler)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                handler.Invoke(e);
            }
        }
    }
    
    public class AsyncHelper {}
}