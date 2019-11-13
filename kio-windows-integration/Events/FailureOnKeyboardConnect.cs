﻿using System;

namespace kio_windows_integration.Events
{
    /// <summary>
    /// Sent when a connection attempt on the keybaord has failed
    /// </summary>
    public class FailureOnKeyboardConnect
    {
        public FailureOnKeyboardConnect(string msg, Exception inner)
        {

            Message = msg;
            Inner = inner;
        }

        public Exception Inner { get; }

        public string Message { get; }
    }
}