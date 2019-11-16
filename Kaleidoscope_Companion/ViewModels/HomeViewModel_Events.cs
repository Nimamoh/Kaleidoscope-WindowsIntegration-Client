using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Models;
using kaleidoscope_companion.Helpers;
using static kaleidoscope_companion.Models.KeyboardConnectHelper;
using static kaleidoscope_companion.ViewModels.HomeViewModel.ConnectState;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kaleidoscope_companion.ViewModels
{
    public partial class HomeViewModel : Screen
        , IHandle<PendingOnKeyboardConnect>
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
            ConnectAsync().SafeFireAndForget(e =>
            {
                Log.Error("An unexpected error occured during connection to keyboard", e);
                eventAggregator.PublishOnUIThread(new FailureOnKeyboardConnect(e.Message, e));
            });
        }

        private async Task ConnectAsync()
        {
            var port = Ports[SelectedPort];

            eventAggregator.PublishOnUIThread(new PendingOnKeyboardConnect(port.Name));
            try
            {
                await TryConnectKeyboard(keyboardSerialPort, port.Name);
                eventAggregator.PublishOnUIThread(new SuccessOnKeyboardConnect(port.Name));
            }
            catch (KeyboardConnectHelper.KeyboardConnectException e)
            {
                eventAggregator.PublishOnUIThread(new FailureOnKeyboardConnect(e.Message, e));
            }
        }

        public void Disconnect()
        {
            KeyboardConnectState = Pending;
            keyboardSerialPort.CloseAsync()
                .SafeFireAndForget(exception =>
                {
                    Log.Warn("Failed to disconnect keyboard", exception);
                    KeyboardConnectState = Disconnected;
                });
        }

        #endregion

        #region Message processing

        public void Handle(PendingOnKeyboardConnect message)
        {
            KeyboardConnectState = Pending;
        }

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