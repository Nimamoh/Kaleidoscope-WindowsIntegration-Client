namespace kio_windows_integration.Events
{
    /// <summary>
    /// Sent when serial port associated to the keyboard has just opened.
    /// Sent by the poller to check serial port status
    /// </summary>
    public class SerialPortOnline: IAppEvent { }
}