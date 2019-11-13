﻿using Caliburn.Micro;
using kio_windows_integration.Events;

namespace kio_windows_integration.ViewModels
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

        public void OpenMenu()
        {
            MenuCollapsed = false;
        }

        public void CloseMenu()
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