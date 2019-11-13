using System.IO.Ports;
using Caliburn.Micro;

namespace kio_windows_integration.ViewModels
{
    public partial class ShellViewModel : Conductor<object>
    {
        private readonly SerialPort keyboardSerialPort;

        public ShellViewModel(
            SerialPort keyboardSerialPort,
            SerialMonViewModel serialMonViewModel,
            ConfigureViewModel configureViewModel,
            DebugViewModel debugViewModel,
            HomeViewModel homeViewModel,
            IEventAggregator eventAggregator)
        {
            this.keyboardSerialPort = keyboardSerialPort;

            this.serialMonViewModel = serialMonViewModel;
            this.homeViewModel = homeViewModel;
            this.debugViewModel = debugViewModel;
            this.configureViewModel = configureViewModel;

            this.eventAggregator = eventAggregator;

            eventAggregator.Subscribe(this);
        }

        public string Title { get; } = "Kaleidoscope Companion";

        #region Status bar

        private string connectedPort = NotConnectedStatusBarMessage;

        public string ConnectedPort
        {
            get => connectedPort;
            set
            {
                connectedPort = value;
                NotifyOfPropertyChange(nameof(ConnectedPort));
            }
        }

        #endregion

        #region Menu

        private bool menuCollapsed;

        public bool MenuCollapsed
        {
            get => menuCollapsed;
            set
            {
                menuCollapsed = value;
                NotifyOfPropertyChange(() => MenuCollapsed);
            }
        }

        #endregion

        #region Guards

        private bool canNavToHome = true;

        public bool CanNavToHome
        {
            get => canNavToHome;
            set
            {
                canNavToHome = value;
                NotifyOfPropertyChange(nameof(CanNavToHome));
            }
        }

        private bool canNavToConf = false;

        public bool CanNavToConf
        {
            get => canNavToConf;
            set
            {
                canNavToConf = value;
                NotifyOfPropertyChange(nameof(CanNavToConf));
            }
        }

        private bool canNavToSerialMon = false;

        public bool CanNavToSerialMon
        {
            get => canNavToSerialMon;
            set
            {
                canNavToSerialMon = value;
                NotifyOfPropertyChange(nameof(CanNavToSerialMon));
            }
        }

        #endregion
    }
}