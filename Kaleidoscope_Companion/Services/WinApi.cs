using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Models;
using Microsoft.WindowsAPICodePack.Shell;

namespace kaleidoscope_companion.Services
{
    /// <summary>
    /// Wrapper of diverse windows platform APIs
    /// </summary>
    public partial class WinApi
    {
        #region Constants

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        #endregion

        #region Bindings

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32")]
        private static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        /// <summary>
        /// Retrieve the PID of the process that created the window
        /// <see cref="GetWindowThreadProcessId"/>
        /// </summary>
        /// <param name="hwnd">a window handle</param>
        /// <returns>the pid, -1 if it failed</returns>
        public static Int32 GetWindowProcessID(IntPtr hwnd)
        {
            Int32 pid = -1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime);

        private WinEventDelegate _winEventDelegate;
        private IntPtr _winEventHook;

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            int pid = GetWindowProcessID(hwnd);
            if (pid > 0)
                OnProcessOnForeground(pid);
        }

        #endregion

        #region Events

        public delegate void ProcessOnForegroundHandler(object sender, ProcessEventArgs e);

        /// <summary>
        /// Event sent when a process just had its window placed on foreground
        /// </summary>
        public event ProcessOnForegroundHandler ProcessOnForeground;

        protected virtual void OnProcessOnForeground(int pid)
        {
            if (ProcessOnForeground != null)
                ProcessOnForeground(this, new ProcessEventArgs(pid));
        }

        #endregion

        public WinApi()
        {
            _winEventDelegate = WinEventProc;
            _winEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventDelegate, 0, 0,
                WINEVENT_OUTOFCONTEXT);
        }

        ~WinApi()
        {
            UnhookWinEvent(_winEventHook);
        }

        /// <inheritdoc cref="QueryInstalledPrograms"/>
        public static async Task<IEnumerable<ApplicationMetaInf>> QueryInstalledProgramsAsync()
        {
            return await Task.Run(QueryInstalledPrograms);
        }

        /// <summary>
        /// Query the apps folder to query items, most of them containing installed apps.
        /// Note that this method is not comprehensive but gives a good approximation
        /// </summary>
        /// <returns>
        /// Some metadata about installed apps
        /// </returns>
        public static IEnumerable<ApplicationMetaInf> QueryInstalledPrograms()
        {
            var FODLERID_AppsFolder =
                new Guid(
                    "{1e87508d-89c2-42f0-8a7e-645a0f50ca58}"); // GUID taken from https://docs.microsoft.com/en-us/windows/win32/shell/knownfolderid
            ShellObject appsFolder = (ShellObject) KnownFolderHelper.FromKnownFolderId(FODLERID_AppsFolder);

            foreach (var app in (IKnownFolder) appsFolder)
            {
                string name = app.Name; // Friendly app name
                // string appUserModelID = app.ParsingName; // Did not use since near no application defines it
                ImageSource icon = app.Thumbnail?.SmallBitmapSource;
                string path = app.Properties?.System?.Link?.TargetParsingPath?.Value;

                if (path?.Contains(".exe") ?? false) // filter out non executable items
                    yield return new ApplicationMetaInf(name, icon, path);
            }
        }
    }
}