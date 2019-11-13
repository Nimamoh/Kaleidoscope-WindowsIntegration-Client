using System;
using System.Data;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Caliburn.Micro;
using kio_windows_integration.Events;
using kio_windows_integration.Services;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kio_windows_integration.Models
{
    /// <summary>
    /// Represent the connection to the keyboard
    /// </summary>
    public static class KeyboardConnectHelper
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(KeyboardConnectHelper));

        // @formatter:off
        public class KeyboardConnectException : Exception
        {
            public KeyboardConnectException(string msg, Exception inner): base(msg, inner) { } 
        } 
        // @formatter:on

        /// <summary>
        /// Connects the keyboard with default params. Returns the arg serial port configured.
        /// Then check if API support is OKAY on keyboard side
        /// Also sends event that the keyboard has successfully been connected to through Event aggregator
        /// </summary>
        /// <returns></returns>
        public static async Task<SerialPort> ConfigureAndCheck(SerialPort port, String portName, IEventAggregator aggregator)
        {
            if (port.IsOpen)
               port.Close();

            port.PortName = portName;
            port.BaudRate = 9600;
            port.DtrEnable = true;
            port.RtsEnable = true;
            port.WriteTimeout = 500;
            port.ReadTimeout = 500;

            try
            {
                port.Open();
                var version =await WindowsIntegrationFocusApi.VersionAsync(port);
                aggregator.PublishOnUIThread(new SuccessOnKeyboardConnect(portName));
                Log.Info("Successfully connected to " + portName+". Api version " + version);
            }
            catch (Exception e)
            {
                port.Close();
                aggregator.PublishOnUIThread(new FailureOnKeyboardConnect(e.Message, e));
                throw new KeyboardConnectException(e.Message, e);
            }

            return port;
        }
    }
}