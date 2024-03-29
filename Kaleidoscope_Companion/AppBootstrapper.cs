﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Helpers;
using kaleidoscope_companion.Models;
using kaleidoscope_companion.Services;
using kaleidoscope_companion.ViewModels;
using log4net.Config;
using static kaleidoscope_companion.Helpers.ErrorMangementHelper;
using static kaleidoscope_companion.Models.KeyboardConnectHelper;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace kaleidoscope_companion
{
    public partial class AppBootstrapper : BootstrapperBase
        , IHandle<ProcessOnFocus>
    {
        #region App wide singletons

        private static readonly ILog Log = LogManager.GetLogger(typeof(AppBootstrapper));

        #endregion

        public AppBootstrapper()
        {
            Initialize();
        }

        #region Message handling

        public async Task HandleAsync(ProcessOnFocus message, CancellationToken cancellationToken)
        {
            var pid = message.Pid;
            
            var layerSwitcher = _container.GetInstance<LayerSwitcher>();
            var mappings = _container.GetInstance<ISet<ApplicationLayerMapping>>();
            var processName = Process.GetProcessById(pid).ProcessName;
            
            var relatedMappings =
                mappings.Where(mapping => processName == mapping.ProcessName);

            await layerSwitcher.UnsetLastLayersAsync();
            var layers = relatedMappings.Select(mapping => mapping.Layer).ToArray();
            await layerSwitcher.SetLayersAsync(layers);
        }

        #endregion

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Log.Debug("Application is starting...");

            var logErrorHandler = _container.GetInstance<LogErrorHandler>();
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var winApi = _container.GetInstance<WinApi>();

            // eventAggregator.Subscribe(this);
            eventAggregator.SubscribeOnPublishedThread(this);

            DisplayRootViewFor<ShellViewModel>();

            #region App wide configuration

            winApi.ProcessOnForeground += OnProcessOnForeground;
            TryAutoConnect().SafeFireAndForget(logErrorHandler);
            PollSerialPortAsync().SafeFireAndForget(logErrorHandler);

            #endregion

            Log.Debug("Application started.");
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Log.Debug("Application is exiting...");

            var logErrorHandler = _container.GetInstance<LogErrorHandler>();
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var winApi = _container.GetInstance<WinApi>();
            var layerSwitcher = _container.GetInstance<LayerSwitcher>();
            eventAggregator.Unsubscribe(this);

            winApi.ProcessOnForeground -= OnProcessOnForeground;
            layerSwitcher.UnsetLastLayersAsync().SafeFireAndForget(logErrorHandler);

            base.OnExit(sender, e);
            this.stopPoller = true;

            Log.Debug("Application exited...");
        }

        private void OnProcessOnForeground(object sender, ProcessEventArgs args)
        {
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var pid = args.Pid;
            eventAggregator.PublishOnUIThreadAsync(new ProcessOnFocus(pid));
        }

        #region Serial port routines

        private bool stopPoller;

        private async Task TryAutoConnect()
        {
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var sp = keyboardSerialPort;
            var ports = SerialPort.GetPortNames();

            await eventAggregator.PublishOnUIThreadAsync(new PendingOnKeyboardConnect(null));
            foreach (var port in ports)
            {
                await Silently(() =>  TryConnectKeyboard(sp, port));

                if (!keyboardSerialPort.IsOpen) continue;
                await eventAggregator.PublishOnUIThreadAsync(new SuccessOnKeyboardConnect(port));
                break;
            }

            if (!keyboardSerialPort.IsOpen)
                await eventAggregator.PublishOnUIThreadAsync(new FailureOnKeyboardConnect("Cannot autoconnect"));
        }

        private async Task PollSerialPortAsync()
        {
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var sp = _container.GetInstance<SerialPort>();

            bool IsSerialPortOpen()
            {
                return sp?.IsOpen ?? false;
            }

            var wasPreviouslyOpened = IsSerialPortOpen();
            while (!stopPoller)
            {
                await Task.Delay(3000);
                var isCurrentlyOpened = IsSerialPortOpen();

                if (wasPreviouslyOpened == isCurrentlyOpened)
                    continue;

                if (isCurrentlyOpened)
                    await eventAggregator.PublishOnUIThreadAsync(new SerialPortOnline());
                else
                    await eventAggregator.PublishOnUIThreadAsync(new SerialPortOffline());
                wasPreviouslyOpened = isCurrentlyOpened;
            }

            stopPoller = false;
        }
        #endregion
    }
}