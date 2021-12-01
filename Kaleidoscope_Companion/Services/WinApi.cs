using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Models;
using log4net;
using Microsoft.WindowsAPICodePack.Shell;

namespace kaleidoscope_companion.Services
{
    #region Interop for working with shell links
    //
    // Get from https://stackoverflow.com/questions/4897655/create-a-shortcut-on-desktop/14632782#14632782
    //
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd,
            int fFlags);

        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);

        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath,
            out int piIcon);

        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    #endregion

    /// <summary>
    /// Wrapper of diverse windows platform APIs
    /// </summary>
    public partial class WinApi
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WinApi));

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

            Log.Debug("Listing installed apps...");

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
                    yield return ApplicationMetaInf.Construct(name, icon, path);
            }

            Log.Debug("Listed installed apps...");
        }
    }
}