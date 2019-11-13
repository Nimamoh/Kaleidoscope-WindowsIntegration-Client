using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using kio_windows_integration.ValueConverters;
using static kio_windows_integration.ViewModels.HomeViewModel;

namespace kio_windows_integration.ViewModels
{
    #region View data types

    public class PortItem
    {
        public PortItem(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    #endregion

    #region Converters

    public class ConnectingIsVisibleConverter :
        BaseValueConverter<ConnectState, Visibility>
    {
        public override Visibility Convert(ConnectState value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ConnectState.Disconnected:
                case ConnectState.Pending:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public override ConnectState ConvertBack(Visibility value, object parameter, CultureInfo culture)
        {
            return ConnectState.Disconnected;
        }
    }

    public class ConnectingIsCollapsedConverter :
        ConnectingIsVisibleConverter
    {
        private Visibility invert(Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Hidden:
                case Visibility.Collapsed:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public override Visibility Convert(ConnectState value, object parameter, CultureInfo culture)
        {
            var initial = base.Convert(value, parameter, culture);
            return invert(initial);
        }
    }

    #endregion


    public partial class HomeViewModel
    {
        public enum ConnectState
        {
            Disconnected,
            Pending,
            Connected
        }

        private readonly SerialPort keyboardSerialPort;

        private ConnectState keyboardConnectState = ConnectState.Disconnected;

        public ConnectState KeyboardConnectState
        {
            get => keyboardConnectState;
            set
            {
                keyboardConnectState = value;
                NotifyOfPropertyChange(nameof(KeyboardConnectState));
                CanConnect = (value == ConnectState.Disconnected);
            }
        }

        private bool canConnect;

        public bool CanConnect
        {
            get => canConnect;
            set { canConnect = value; NotifyOfPropertyChange(nameof(CanConnect)); }
        }

        private ObservableCollection<PortItem> ports;

        public ObservableCollection<PortItem> Ports
        {
            get => ports;
            set
            {
                ports = value;
                NotifyOfPropertyChange(nameof(Ports));
            }
        }

        private int selectedPort;

        public int SelectedPort
        {
            get => selectedPort;
            set
            {
                selectedPort = value;
                NotifyOfPropertyChange(nameof(SelectedPort));
            }
        }
    }
}