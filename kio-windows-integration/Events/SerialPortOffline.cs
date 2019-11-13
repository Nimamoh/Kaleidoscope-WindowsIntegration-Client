namespace kio_windows_integration.Events
{
    /// <summary>
    /// Sent when serial port disconnected. Usually sent by the poller checking serial port status
    /// </summary>
    public class SerialPortOffline: IAppEvent { }
}