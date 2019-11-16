using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Models;

namespace kaleidoscope_companion.ViewModels
{
    public partial class SerialMonViewModel : Screen
    {
        private readonly SerialPort keyboardSerialPort;
        private readonly IEventAggregator eventAggregator;

        public SerialMonViewModel(SerialPort keyboardSerialPort, IEventAggregator eventAggregator)
        {
            this.keyboardSerialPort = keyboardSerialPort;
            this.eventAggregator = eventAggregator;
            CanSendToKeyboard = keyboardSerialPort?.IsOpen ?? false;
        }

        #region UI and events

        private string userInput = "Type your text there";

        public string UserInput
        {
            get => userInput;
            set
            {
                userInput = value;
                NotifyOfPropertyChange(nameof(UserInput));
            }
        }


        private string console = "";

        public string Console
        {
            get { return console; }
            set
            {
                console = value;
                NotifyOfPropertyChange(nameof(Console));
            }
        }

        private bool canSendToKeyboard;

        public bool CanSendToKeyboard
        {
            get => canSendToKeyboard;
            set
            {
                canSendToKeyboard = value;
                NotifyOfPropertyChange(nameof(CanSendToKeyboard));
            }
        }

        #endregion
    }
}