using System;
using System.IO.Ports;
using System.Threading.Tasks;
using kaleidoscope_companion.Exceptions;
using log4net;

namespace kaleidoscope_companion.Services
{
    /// <summary>
    /// Client side of the keyboard Windows Integration API
    /// </summary>
    public static partial class WindowsIntegrationFocusApi
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WindowsIntegrationFocusApi));


        /// <inheritdoc cref="Version(SerialPort)"/>
        public static async Task<string> VersionAsync(SerialPort sp)
        {
            return await Task.Run(() => Version(sp));
        }

        /// <summary>
        /// Retrieve the Windows Integration API version installed on the keyboard
        /// </summary>
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// <returns>A semversion</returns>
        private static string Version(SerialPort sp)
        {
            return WrapEx(() => FocusClient.Request(sp, "wi.version"));
        }

        
        /// <inheritdoc cref="TopActiveLayer(SerialPort)"/>
        public static async Task<int> TopActiveLayerAsync(SerialPort sp)
        {
            return await Task.Run(() => TopActiveLayer(sp));
        }
        
        /// <summary>
        /// Retrieve the Top Active layer
        /// </summary>
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// <returns>The top active layer (0 based numbered)</returns>
        private static int TopActiveLayer(SerialPort sp)
        {
            return WrapEx(delegate
            {
                var sTopActiveLayer = FocusClient.Request(sp, "wi.layer");
                return int.Parse(sTopActiveLayer);
            });
        }

        /// <inheritdoc cref="TotalLayerCount(int)"/>
        public static async Task<int> TotalLayerCountAsync(SerialPort sp)
        {
            return await Task.Run(() => TotalLayerCount(sp));
        }

        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// <returns>The total number layer count</returns>
        private static int TotalLayerCount(SerialPort sp)
        {
            return WrapEx(delegate
            {
                var sLayerCount = FocusClient.Request(sp, "wi.layers");
                return int.Parse(sLayerCount);
            });
        }

        /// <inheritdoc cref="IsLayerActive(SerialPort,int)"/>
        public static async Task<bool> IsLayerActiveAsync(SerialPort sp, int layerN)
        {
            return await Task.Run(() => IsLayerActive(sp, layerN));
        }
        /// <summary>
        /// Tell if the n layer is active
        /// </summary>
        /// <param name="layerN"></param>
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// <returns>true if active, false otherwise</returns>
        private static bool IsLayerActive(SerialPort sp, int layerN)
        {
            return WrapEx(delegate
            {
                var layerArg = Convert.ToString(layerN);
                var answer = FocusClient.Request(sp, "wi.layer", layerArg);
                return bool.Parse(answer);
            });
        }


        /// <inheritdoc cref="ActivateLayer(SerialPort,int)"/>
        public static async Task ActivateLayerAsync(SerialPort sp, int layerN)
        {
             await Task.Run(() => ActivateLayer(sp, layerN));
        }
        /// <summary>
        /// Enables layer n
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// </summary>
        private static void ActivateLayer(SerialPort sp, int layerN)
        {
            Log.Info("Activate layer " + layerN);
            SetLayer(sp, layerN, true);
        }

        
        /// <inheritdoc cref="DeactivateLayer(SerialPort,int)"/>
        public static async Task DeactivateLayerAsync(SerialPort sp, int layerN)
        {
             await Task.Run(() => DeactivateLayer(sp, layerN));
        }
        /// <summary>
        /// Disable layer n
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// </summary>
        private static void DeactivateLayer(SerialPort sp, int layerN)
        {
            Log.Info("Deactivate layer " + layerN);
            SetLayer(sp, layerN, false);
        }
        
        /// <summary>
        /// <exception cref="WindowsIntegrationFocusApiException"></exception>
        /// </summary>
        private static void SetLayer(SerialPort sp, int layerN, bool active)
        {
            WrapEx(delegate
            {
                var layerArg = Convert.ToString(layerN);
                var activeArg = (active) ? "1" : "0";
                FocusClient.Request(sp, "wi.layer", layerArg, activeArg);
            });
        }
    }
}