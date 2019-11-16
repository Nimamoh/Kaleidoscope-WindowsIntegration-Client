namespace kaleidoscope_companion.Events
{
    /// <summary>
    /// sent when keyboard has successfully been connected to through serial protocol
    /// holds the port name
    /// </summary>
    public class SuccessOnKeyboardConnect: IAppEvent
    {
        public string PortName { get; private set; }
        public SuccessOnKeyboardConnect(string portName)
        {
            PortName = portName;
        }
    }
}