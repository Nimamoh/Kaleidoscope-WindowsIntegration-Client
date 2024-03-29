﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Helpers;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kaleidoscope_companion.ViewModels
{
    public partial class SerialMonViewModel : IHandle<SuccessOnKeyboardConnect>
        , IHandle<SerialPortOffline>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SerialMonViewModel));


        #region Message processing

        public Task HandleAsync(SuccessOnKeyboardConnect message, CancellationToken cancellationToken)
        {
            CanSendToKeyboard = true;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SerialPortOffline message, CancellationToken cancellationToken)
        {
            CanSendToKeyboard = false;
            return Task.CompletedTask;
        }

        #endregion

        #region UI events

        public void SendToKeyboard(string msg)
        {
            SendToKeyboardAsync(msg)
                .SafeFireAndForget(e => Log.Error("Failed to send command to keyboard: " + msg, e));
        }

        private async Task SendToKeyboardAsync(string msg)
        {
            if (msg == null)
                return;
            Console += Environment.NewLine + "< " + msg + Environment.NewLine;
            UserInput = "";
            string response = await FocusClient.RequestAsync(keyboardSerialPort, msg);
            Console += "> " + response.Replace("\n", "\n> ");
        }

        #endregion

        #region Lifecycle

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

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            CanSendToKeyboard = this.keyboardSerialPort?.IsOpen ?? false;
        }

        #endregion
    }
}