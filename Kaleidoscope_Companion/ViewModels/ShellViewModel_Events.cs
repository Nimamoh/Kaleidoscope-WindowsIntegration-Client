using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Helpers;

namespace kaleidoscope_companion.ViewModels
{
    public partial class ShellViewModel : IHandle<SuccessOnKeyboardConnect>
        , IHandle<SerialPortOffline>
    {
        private readonly IEventAggregator eventAggregator;

        #region Lifecycle

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            eventAggregator.SubscribeOnPublishedThread(this);
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }

        protected override void OnViewLoaded(object view)
        {
            CanNavToHome = true;
            CanNavToSerialMon = keyboardSerialPort?.IsOpen ?? false;
            Task.Run(NavToHome);
        }

        #endregion


        #region Actions

        public void UnfoldMenu()
        {
            MenuCollapsed = false;
        }

        public void FoldMenu()
        {
            MenuCollapsed = true;
        }

        #endregion

        #region Message processing

        public async Task HandleAsync(SuccessOnKeyboardConnect message, CancellationToken cancellationToken)
        {
            var portName = message.PortName;
            ConnectedPort = portName;

            CanNavToSerialMon = true;
            CanNavToConf = true;
            
            await NavToConf(); // automatically navigate to configuration when connected
        }

        public async Task HandleAsync(SerialPortOffline message, CancellationToken cancellationToken)
        {
            ConnectedPort = NotConnectedStatusBarMessage;

            await NavToHome();
            CanNavToSerialMon = false;
            CanNavToConf = false;
        }

        #endregion
    }
}