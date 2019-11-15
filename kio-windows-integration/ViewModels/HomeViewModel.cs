using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Windows;
using kio_windows_integration.ValueConverters;
using static System.Windows.Visibility;
using static kio_windows_integration.ViewModels.HomeViewModel;
using static kio_windows_integration.ViewModels.HomeViewModel.ConnectState;

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

    abstract class WithNoConvertBack<T, TR> : BaseValueConverter<T, TR>
    {
        public override T ConvertBack(TR value, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Should never be used");
        }
    }
    class ConnectedIsCollapsedConverter :
        WithNoConvertBack<ConnectState, Visibility>
    {
        public override Visibility Convert(ConnectState value, object parameter, CultureInfo culture)
        {
            return value == Connected ? Collapsed : Visible;
        }
    }

    class ConnectedIsVisibleConverter :
        WithNoConvertBack<ConnectState, Visibility>
    {
        public override Visibility Convert(ConnectState value, object parameter, CultureInfo culture)
        {
            return value == Connected ? Visible : Collapsed;
        }
    }

    class PendingIsVisibleConverter :
        WithNoConvertBack<ConnectState, Visibility>
    {
        public override Visibility Convert(ConnectState value, object parameter, CultureInfo culture)
        {
            return value == Pending ? Visible : Collapsed;
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

        private ConnectState keyboardConnectState = Disconnected;

        public ConnectState KeyboardConnectState
        {
            get => keyboardConnectState;
            set
            {
                keyboardConnectState = value;
                NotifyOfPropertyChange(nameof(KeyboardConnectState));
                CanConnect = (value == Disconnected);
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