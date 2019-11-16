namespace kaleidoscope_companion.Events
{
    /// <summary>
    /// Sent when trying to connect to the keyboard
    /// XXX : When you send this event, please make sure to always eventually send
    /// <see cref="SuccessOnKeyboardConnect"/> or <see cref="FailureOnKeyboardConnect"/>
    /// to notify subscribers to the end of the pending state
    /// </summary>
    public class PendingOnKeyboardConnect: IAppEvent
    {

        public PendingOnKeyboardConnect(string portName)
        {
            PortName = portName;
        }

        /// <summary>
        /// The port name trying to connect to. Can be null
        /// </summary>
        public string PortName { get; set; }
    }
}