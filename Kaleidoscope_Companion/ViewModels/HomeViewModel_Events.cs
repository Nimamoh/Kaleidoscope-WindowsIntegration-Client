using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
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
            this.eventAggregator.SubscribeOnPublishedThread(this);
            this.keyboardSerialPort = keyboardSerialPort;
        }

        #region Actions
        
        public void Connect()
        {
            ConnectAsync().SafeFireAndForget(e =>
            {
                Log.Error("An unexpected error occured during connection to keyboard", e);
                Task.Run(() => eventAggregator.PublishOnUIThreadAsync(new FailureOnKeyboardConnect(e.Message, e)));
            });
        }

        private async Task ConnectAsync()
        {
            var port = Ports[SelectedPort];

            await eventAggregator.PublishOnUIThreadAsync(new PendingOnKeyboardConnect(port.Name));
            try
            {
                await TryConnectKeyboard(keyboardSerialPort, port.Name);
                await eventAggregator.PublishOnUIThreadAsync(new SuccessOnKeyboardConnect(port.Name));
            }
            catch (KeyboardConnectException e)
            {
                await eventAggregator.PublishOnUIThreadAsync(new FailureOnKeyboardConnect(e.Message, e));
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

        public Task HandleAsync(PendingOnKeyboardConnect message, CancellationToken cancellationToken)
        {
            KeyboardConnectState = Pending;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SuccessOnKeyboardConnect message, CancellationToken cancellationToken)
        {
            KeyboardConnectState = Connected;
            return Task.CompletedTask;
        }

        public Task HandleAsync(FailureOnKeyboardConnect message, CancellationToken cancellationToken)
        {
            KeyboardConnectState = Disconnected;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SerialPortOffline message, CancellationToken cancellationToken)
        {
            KeyboardConnectState = Disconnected;
            return Task.CompletedTask;
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

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            eventAggregator.SubscribeOnPublishedThread(this);
            await base.OnActivateAsync(cancellationToken);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            await base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion
    }
}