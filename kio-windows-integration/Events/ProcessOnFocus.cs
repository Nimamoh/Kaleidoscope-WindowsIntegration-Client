namespace kio_windows_integration.Events
{
    /// <summary>
    /// Sent when a new window has been sent to foreground
    /// Hold the PID of the process of the window 
    /// </summary>
    public class ProcessOnFocus: IAppEvent
    {
        public int Pid { get; }

        public ProcessOnFocus(int pid)
        {
            Pid = pid;
        }
    }
}