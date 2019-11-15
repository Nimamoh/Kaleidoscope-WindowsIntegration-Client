using System;
using System.IO.Ports;
using System.Threading.Tasks;
using kio_windows_integration.Services;
using static kio_windows_integration.Helpers.ErrorMangementHelper;

namespace kio_windows_integration.Models
{
    public static class KeyboardConnectHelperExtensions
    {
        public static Task OpenAsync(this SerialPort serialPort)
        {
            return Task.Run(serialPort.Open);
        }

        public static Task CloseAsync(this SerialPort serialPort)
        {
            return Task.Run(serialPort.Close);
        }
    }

    /// <summary>
    /// Represent the connection to the keyboard
    /// </summary>
    public static class KeyboardConnectHelper
    {
        // @formatter:off
        public class KeyboardConnectException : Exception
        {
            public KeyboardConnectException(string msg, Exception inner): base(msg, inner) { } 
        } 
        // @formatter:on

        /// <summary>
        /// Try to connect to keyboard through the port name
        /// Throws <exception cref="KeyboardConnectException" /> if failed to connect
        /// </summary>
        /// <param name="port"></param>
        /// <param name="portName"></param>
        public static async Task TryConnectKeyboard(SerialPort port, string portName)
        {
            if (port.IsOpen)
                port.Close();


            port.PortName = portName;
            ConfigureDefault(port);

            try
            {
                await port.OpenAsync().ConfigureAwait(false);
                await WindowsIntegrationFocusApi.VersionAsync(port).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await Silently(port.CloseAsync, Task.CompletedTask).ConfigureAwait(false);
                throw new KeyboardConnectException(e.Message, e);
            }
        }

        private static void ConfigureDefault(SerialPort serialPort)
        {
            serialPort.BaudRate = 9600;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.WriteTimeout = 500;
            serialPort.ReadTimeout = 500;
        }
    }
}