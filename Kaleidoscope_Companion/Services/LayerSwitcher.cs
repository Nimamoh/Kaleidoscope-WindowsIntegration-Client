using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using kaleidoscope_companion.Models;
using log4net;

namespace kaleidoscope_companion.Services
{
    /// <summary>
    /// Let apply keyboard layers in different fashion
    /// </summary>
    public class LayerSwitcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LayerSwitcher));

        private readonly SerialPort port;
        private readonly Stack<int> nextLayers = new Stack<int>();

        public LayerSwitcher(SerialPort port)
        {
            this.port = port;
        }

        /// <summary>
        /// Activate the set of layers passed in parameters.
        /// This set of layer remains active until another call to this method is done or
        /// when <see cref="UnsetLastLayersAsync"/> is called
        /// </summary>
        public async Task SetLayersAsync(params int[] layerNumbers)
        {
            var layers = layerNumbers?.Where(i => i >= 0) ?? new int[] { };
            var enumerable = layers as int[] ?? layers.ToArray();
            if (!enumerable.Any())
                return;

            await UnsetLastLayersAsync();

            foreach (var layer in enumerable)
            {
                try
                {
                    await WindowsIntegrationFocusApi.ActivateLayerAsync(port, layer);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to switch to layer {layer}", e);
                    return; // Exit
                }
                finally
                {
                    nextLayers.Push(-layer); // Schedule layer deactivation on next call
                }
            }
        }

        /// <summary>
        /// Deactivate previously activated set of layer through <see cref="SetLayersAsync"/>
        /// </summary>
        public async Task UnsetLastLayersAsync()
        {
            while (nextLayers.Count > 0)
                await ActivateOrDeactivateALayerAsync(nextLayers.Pop());
        }

        private async Task ActivateOrDeactivateALayerAsync(int layerN)
        {
            var abs = Math.Abs(layerN);
            try
            {
                if (layerN > 0)
                    await WindowsIntegrationFocusApi.ActivateLayerAsync(port, abs);
                else
                    await WindowsIntegrationFocusApi.DeactivateLayerAsync(port, abs);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to activate / deactivate layer {abs}", e);
            }
        }
    }
}