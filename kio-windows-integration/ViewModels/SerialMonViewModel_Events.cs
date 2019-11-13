using System;
using Caliburn.Micro;
using kio_windows_integration.Events;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kio_windows_integration.ViewModels
{
    public partial class SerialMonViewModel: IHandle<SuccessOnKeyboardConnect>
        , IHandle<SerialPortOffline>
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(SerialMonViewModel));

        
        #region Message processing
        
        public void Handle(SuccessOnKeyboardConnect message)
        {
            CanSendToKeyboard = true;
        }

        public void Handle(SerialPortOffline message)
        {
            CanSendToKeyboard = false;
        }

        #endregion

        #region UI events

        public async void SendToKeyboard(string msg)
        {
            if (msg == null)
                return;
            try
            {
                Console += Environment.NewLine + "< " + msg + Environment.NewLine;
                UserInput = "";
                string response = await FocusClient.RequestAsync(keyboardSerialPort, msg);
                Console += "> " + response.Replace("\n", "\n> ");
            }
            catch (Exception e)
            {
                Log.Error("Failed to send command to keyboard: " + msg, e);
            }
        }

        #endregion

        #region Lifecycle

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

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            CanSendToKeyboard = this.keyboardSerialPort?.IsOpen ?? false;
        }

        #endregion
    }
}