using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using kio_windows_integration.Events;
using kio_windows_integration.Helpers;
using static kio_windows_integration.Models.KeyboardConnectHelper;
using static kio_windows_integration.ViewModels.HomeViewModel.ConnectState;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kio_windows_integration.ViewModels
{
    public partial class HomeViewModel : Screen
        , IHandle<SuccessOnKeyboardConnect>
        , IHandle<FailureOnKeyboardConnect>
        , IHandle<SerialPortOffline>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeViewModel));

        private readonly IEventAggregator eventAggregator;

        public HomeViewModel(SerialPort keyboardSerialPort, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
            this.keyboardSerialPort = keyboardSerialPort;
        }

        #region Actions

        public void Connect()
        {
            var port = Ports[SelectedPort];
            KeyboardConnectState = Pending;

            ConfigureCheckAndNotify(keyboardSerialPort, port.Name, eventAggregator)
                .SafeFireAndForget(exception =>
                {
                    Log.Error(exception is KeyboardConnectException
                            ? "Failed to connect to serial port"
                            : "An unexpected exception occured", exception);
                });
        }

        public void Disconnect()
        {
            KeyboardConnectState = Pending;
            Task.Run(keyboardSerialPort.Close)
                .SafeFireAndForget(exception =>
                {
                    Log.Warn("Failed to disconnect keyboard", exception);
                    KeyboardConnectState = Disconnected;
                });
        }

        #endregion

        #region Message processing

        public void Handle(SuccessOnKeyboardConnect message)
        {
            KeyboardConnectState = Connected;
        }

        public void Handle(FailureOnKeyboardConnect message)
        {
            KeyboardConnectState = Disconnected;
        }

        public void Handle(SerialPortOffline message)
        {
            KeyboardConnectState = Disconnected;
        }

        #endregion

        #region Lifecycles

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            Ports = new ObservableCollection<PortItem>(
                SerialPort.GetPortNames().Select(name => new PortItem(name)));
            SelectedPort = 0;

            var connected = keyboardSerialPort?.IsOpen ?? false;
            KeyboardConnectState = connected ? Connected : Disconnected;
        }

        protected override void OnActivate()
        {
            eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        #endregion
    }
}