using System;
using Caliburn.Micro;
using kio_windows_integration.Events;
using kio_windows_integration.Models;
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
        /// Handle case where serial port goes offline while trying to request it.
        /// Handle error by sending a <see cref="SerialPortOffline"/> accross the event aggregator
        /// <param name="fallbackValue">Fallback value used to recover the handled error</param>
        /// </summary>
        public T NotifyWholeAppOnKeyboardCommunicationLoss<T>(Func<T> expression, T fallbackValue)
        {
            try
            {
                return expression.Invoke();
            }
            catch (KeyboardConnectHelper.KeyboardConnectException e)
            {
                Log.Error("Failed while communicating with keyboard", e);
                eventAggregator.PublishOnUIThread(new SerialPortOffline());
            }

            return fallbackValue;
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
    }
}