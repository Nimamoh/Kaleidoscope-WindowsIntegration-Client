using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using kio_windows_integration.Events;
using kio_windows_integration.Helpers;
using kio_windows_integration.Models;
using kio_windows_integration.Services;
using kio_windows_integration.ViewModels;
using log4net.Config;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

[assembly: XmlConfigurator(Watch = true)]

namespace kio_windows_integration
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

        public void Handle(ProcessOnFocus message)
        {
            var logErrorHandler = _container.GetInstance<LogErrorHandler>();
            var pid = message.Pid;
            HandleProcessOnFocusAsync(pid).SafeFireAndForget(logErrorHandler);
        }
        
        private async Task HandleProcessOnFocusAsync(int pid)
        {
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
            var logErrorHandler = _container.GetInstance<LogErrorHandler>();
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var winApi = _container.GetInstance<WinApi>();
            
            eventAggregator.Subscribe(this);

            DisplayRootViewFor<ShellViewModel>();

            #region App wide configuration

            winApi.ProcessOnForeground += OnProcessOnForeground;
            PollSerialPortAsync().SafeFireAndForget(logErrorHandler);

            #endregion
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var logErrorHandler = _container.GetInstance<LogErrorHandler>();
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var winApi = _container.GetInstance<WinApi>();
            var layerSwitcher = _container.GetInstance<LayerSwitcher>();
            eventAggregator.Unsubscribe(this);

            winApi.ProcessOnForeground -= OnProcessOnForeground;
            layerSwitcher.UnsetLastLayersAsync().SafeFireAndForget(logErrorHandler);

            base.OnExit(sender, e);
            this.stopPoller = true;
        }

        protected void OnProcessOnForeground(object sender, ProcessEventArgs args)
        {
            var eventAggregator = _container.GetInstance<IEventAggregator>();
            var pid = args.Pid;
            eventAggregator.PublishOnUIThread(new ProcessOnFocus(pid));
        }

        #region Serial port poller

        private bool stopPoller = false;

        private async Task PollSerialPortAsync()
        {
            await Task.Run(PollSerialPort);
        }

        private void PollSerialPort()
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
                Thread.Sleep(300);
                var isCurrentlyOpened = IsSerialPortOpen();

                if (wasPreviouslyOpened == isCurrentlyOpened)
                    continue;

                if (isCurrentlyOpened)
                    eventAggregator.PublishOnUIThread(new SerialPortOnline());
                else
                    eventAggregator.PublishOnUIThread(new SerialPortOffline());
                wasPreviouslyOpened = isCurrentlyOpened;
            }

            stopPoller = false;
        }

        #endregion
    }
}