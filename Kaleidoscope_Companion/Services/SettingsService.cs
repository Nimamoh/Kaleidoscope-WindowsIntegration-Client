using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using log4net;

namespace kaleidoscope_companion.Services
{
    /// <summary>
    /// Service taking care of diverse settings applicable to the program
    /// </summary>
    public class SettingsService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SettingsService));

        private readonly string userStartupFolderPath;
        private readonly string executableProgramPath;

        public SettingsService()
        {
            userStartupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            executableProgramPath = Process.GetCurrentProcess().MainModule?.FileName;
        }

        /// <summary>
        /// Check that we can work with user startup folder and
        /// successfully retrieve the executable program path.
        /// Both prerequisite to the option of starting the program along with user login
        /// </summary>
        public bool CanOperateOnStartupWithUser()
        {
            return !string.IsNullOrEmpty(executableProgramPath)
                   && !string.IsNullOrEmpty(userStartupFolderPath);
        }

        /// <summary>
        /// Constructs the Shortcut filename
        /// </summary>
        private string StartupShortcutPath()
        {
            var lnkFileName = Path.GetFileNameWithoutExtension(executableProgramPath);
            return Path.Combine(userStartupFolderPath, lnkFileName + ".lnk");
        }

        /// <summary>
        /// <see cref="ActivateStartProgramWithUserSession"/>
        /// </summary>
        public Task<bool> ActivateStartProgramWithUserSessionAsync()
        {
            return Task.Run(ActivateStartProgramWithUserSession);
        }

        /// <summary>
        /// <see cref="DeactivateStartProgramWithUserSession"/>
        /// </summary>
        public Task<bool> DeactivateStartProgramWithUserSessionAsync()
        {
            return Task.Run(DeactivateStartProgramWithUserSession);
        }

        /// <summary>
        /// <see cref="IsActiveOnStartupWithUserSession"/>
        /// </summary>
        public Task<bool> IsActiveOnStartupWithUserSessionAsync()
        {
            return Task.Run(IsActiveOnStartupWithUserSession);
        }

        /// <summary>
        /// Enable program startup with user login
        /// </summary>
        /// <returns>true if successfully enable, false otherwise.</returns>
        private bool ActivateStartProgramWithUserSession()
        {
            var success = true;
            try
            {
                if (!CanOperateOnStartupWithUser())
                    return false;
                if (IsActiveOnStartupWithUserSession())
                    return true;

                IShellLink link = new ShellLink() as IShellLink;
                link.SetDescription("Kaleidoscope Companion");
                link.SetPath(executableProgramPath);

                IPersistFile file = link as IPersistFile;
                
                file.Save(StartupShortcutPath(), false);
            }
            catch (Exception e)
            {
                Log.Error("Failed to enable on startup", e);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Disable program startup with user login
        /// </summary>
        /// <returns>true if successfully disables, false otherwise</returns>
        private bool DeactivateStartProgramWithUserSession()
        {
            var success = true;
            try
            {
                if (!CanOperateOnStartupWithUser())
                    return false;
                File.Delete(StartupShortcutPath());
            }
            catch (Exception e)
            {
                Log.Error("Failed to disable on startup");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Check that the program is configured to start with user logon
        /// </summary>
        private bool IsActiveOnStartupWithUserSession()
        {
            bool isActive;

            try
            {
                isActive = File.Exists(StartupShortcutPath());
            }
            catch (Exception e)
            {
                Log.Error("Failed to check if program is active on startup", e);
                isActive = false;
            }

            return isActive;
        }
    }
}