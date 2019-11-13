using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kio_windows_integration
{
    /// <summary>
    /// Represents the Focus Protocol, a line based proto.
    /// <see cref="https://github.com/keyboardio/Kaleidoscope-Focus"/>
    /// </summary>
    public class FocusClient
    {
        const string ReqResponseStopLine = ".";

        /// <inheritdoc cref="Request(SerialPort,string,string[])"/>
        public static async Task<string> RequestAsync(SerialPort sp, string command, params string[] args)
        {
            return await Task.Run(() => Request(sp, command, args));
        }

        /// <summary>
        /// Sends a focus request, then reads its response.
        /// A focus request is consists of a list of space separated words, first word being the command, following ones being arguments
        /// A response can be multi line, it is terminated by a line consisting of <see cref="REQ_RESPONSE_END_TOKEN"/> only.
        /// </summary>
        /// <param name="sp">The <see cref="System.IO.Ports.SerialPort"/> object to execute the request on</param>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
        /// <exception cref="T:System.TimeoutException">The operation did not complete before the time-out period ended.
        /// <returns>A string representing the response of the request</returns>
        /// 
        public static string Request(SerialPort sp, string command, params string[] args)
        {
            string wholeCommand = string.Join(" ", args.Prepend(command));
            sp.WriteLine(wholeCommand);

            StringBuilder responseBuilder = new StringBuilder();

            bool firstLoop = true;
            do
            {
                string lineBuf = sp.ReadLine().Replace("\r", "");
                if (lineBuf == ReqResponseStopLine)
                    break;

                if (!firstLoop)
                    responseBuilder.Append(Environment.NewLine);
                responseBuilder.Append(lineBuf);

                firstLoop = false;
            } while (true);

            return responseBuilder.ToString();
        }
    }
}