using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using kio_windows_integration.Events;
using kio_windows_integration.Models;
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

        public async void Connect()
        {
            var port = Ports[SelectedPort];
            try
            {
                KeyboardConnectState = ConnectState.Pending;
                var sp = await KeyboardConnectHelper.ConfigureAndCheck(keyboardSerialPort, port.Name, eventAggregator);
            }
            catch (KeyboardConnectHelper.KeyboardConnectException e)
            {
                Log.Error("Failed to connect to serial port", e);
                KeyboardConnectState = ConnectState.Disconnected;
            }
        }

        public async void Disconnect()
        {
            try
            {
                KeyboardConnectState = ConnectState.Pending;
                await Task.Run(keyboardSerialPort.Close);
            }
            catch (Exception e)
            {
                Log.Warn("Failed to disconnect keyboard", e);
                KeyboardConnectState = ConnectState.Disconnected;
            }
        }

        #endregion

        #region Message processing

        public void Handle(SuccessOnKeyboardConnect message)
        {
            KeyboardConnectState = ConnectState.Connected;
        }

        public void Handle(FailureOnKeyboardConnect message)
        {
            KeyboardConnectState = ConnectState.Disconnected;
        }

        public void Handle(SerialPortOffline message)
        {
            KeyboardConnectState = ConnectState.Disconnected;
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
            KeyboardConnectState = connected ? ConnectState.Connected : ConnectState.Disconnected;
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