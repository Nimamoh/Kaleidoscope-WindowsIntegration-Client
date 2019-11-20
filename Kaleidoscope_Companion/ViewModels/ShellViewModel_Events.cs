using Caliburn.Micro;
using kaleidoscope_companion.Events;

namespace kaleidoscope_companion.ViewModels
{
    public partial class ShellViewModel : IHandle<SuccessOnKeyboardConnect>
        , IHandle<SerialPortOffline>
    {
        private readonly IEventAggregator eventAggregator;

        #region Lifecycle

        protected override void OnActivate()
        {
            eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            eventAggregator.Unsubscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            CanNavToHome = true;
            CanNavToSerialMon = keyboardSerialPort?.IsOpen ?? false;
            NavToHome();
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

        public void Handle(SuccessOnKeyboardConnect message)
        {
            var portName = message.PortName;
            ConnectedPort = portName;

            CanNavToSerialMon = true;
            CanNavToConf = true;
            
            NavToConf(); // automatically navigate to configuration when connected
        }

        public void Handle(SerialPortOffline message)
        {
            ConnectedPort = NotConnectedStatusBarMessage;

            NavToHome();
            CanNavToSerialMon = false;
            CanNavToConf = false;
        }

        #endregion
    }
}