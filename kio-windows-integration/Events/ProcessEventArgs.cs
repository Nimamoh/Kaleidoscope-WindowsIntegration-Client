using System;

namespace kio_windows_integration.Events
{
    public class ProcessEventArgs : EventArgs
    {
        public ProcessEventArgs(int pid)
        {
            Pid = pid;
        }

        public int Pid { get; }
    }
}