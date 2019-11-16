using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Action = System.Action;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kio_windows_integration.Helpers
{
    /// <summary>
    /// Some common error management 
    /// </summary>
    public class ErrorMangementHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ErrorMangementHelper));

        private IEventAggregator eventAggregator;

        public ErrorMangementHelper(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Eats any exception.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fallbackValue">The returned value in case of an exception happening</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Silently<T>(Func<T> expression, T fallbackValue)
        {
            try
            {
                return expression.Invoke();
            }
            catch (Exception)
            {
                return fallbackValue;
            }
        }

        public static async Task Silently(Func<Task> asyncStatement)
        {
            try
            {
                await asyncStatement.Invoke();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void Silently(Action statement)
        {
            try
            {
                statement();
            }
            catch (Exception)
            {
                // 
            }
        }
    }
}