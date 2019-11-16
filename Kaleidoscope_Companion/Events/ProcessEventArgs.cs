using System;

namespace kaleidoscope_companion.Events
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