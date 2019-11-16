using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Helpers;
using kaleidoscope_companion.Models;
using kaleidoscope_companion.Services;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace kaleidoscope_companion.ViewModels
{
    public class DebugViewModel : Screen
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DebugViewModel));

        private WinApi winApi;
        private LogErrorHandler logErrorHandler;

        public DebugViewModel(WinApi winApi, LogErrorHandler logErrorHandler)
        {
            this.logErrorHandler = logErrorHandler;
            this.winApi = winApi;
            this.winApi.ProcessOnForeground += delegate(object sender, ProcessEventArgs args)
            {
                int pid = args.Pid;
                try
                {
                    var process = Process.GetProcessById(pid);

                    CurrentWindowPid = pid;
                    CurrentWindowProcessName = process.ProcessName;
                    Logs += process.ProcessName + ":" + pid + Environment.NewLine;
                }
                catch (Exception e)
                {
                   Log.Warn("Failed to get process infos for PID" + pid, e); 
                }
            };
        }

        #region Events

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate(bool close)
        {
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            OnViewReadyAsync().SafeFireAndForget(logErrorHandler);
        }

        private async Task OnViewReadyAsync()
        {
            var applicationMetaInfs = await WinApi.QueryInstalledProgramsAsync();
            InstalledPrograms = new ObservableCollection<ApplicationMetaInf>(applicationMetaInfs);
        }

        #endregion

        #region props

        private string logs;

        public string Logs
        {
            get => logs;
            set
            {
                logs = value;
                NotifyOfPropertyChange(nameof(Log));
            }
        }

        private string currentWindowProcessName;

        public string CurrentWindowProcessName
        {
            get => currentWindowProcessName;
            set
            {
                currentWindowProcessName = value;
                NotifyOfPropertyChange(nameof(CurrentWindowProcessName));
            }
        }

        private int currentWindowPid;

        public int CurrentWindowPid
        {
            get => currentWindowPid;
            set
            {
                currentWindowPid = value;
                NotifyOfPropertyChange(nameof(CurrentWindowPid));
            }
        }

        private string currentWindowPath;

        public string CurrentWindowPath
        {
            get => currentWindowPath;
            set
            {
                currentWindowPath = value;
                NotifyOfPropertyChange(nameof(CurrentWindowPath));
            }
        }

        private ObservableCollection<ApplicationMetaInf> installedPrograms;

        public ObservableCollection<ApplicationMetaInf> InstalledPrograms
        {
            get => installedPrograms;
            set
            {
                installedPrograms = value;
                NotifyOfPropertyChange(nameof(InstalledPrograms));
            }
        }

        #endregion
    }
}