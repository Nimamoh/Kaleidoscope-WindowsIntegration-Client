using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Helpers;
using kaleidoscope_companion.Services;

namespace kaleidoscope_companion.ViewModels
{
    public partial class SettingsViewModel : Screen
    {
        private readonly SettingsService settingsService;
        private LogErrorHandler logErrorHandler;

        #region UI

        private bool isStartingWithUserSession;

        public bool IsStartingWithUserSession
        {
            get => isStartingWithUserSession;
            set
            {
                isStartingWithUserSession = value;
                NotifyOfPropertyChange(nameof(IsStartingWithUserSession));
            }
        }

        #endregion

        public SettingsViewModel(SettingsService settingsService, LogErrorHandler logErrorHandler)
        {
            this.settingsService = settingsService;
            this.logErrorHandler = logErrorHandler;
        }

        #region UI lifecycle

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            OnViewReadyAsync().SafeFireAndForget(logErrorHandler);
        }

        private async Task OnViewReadyAsync()
        {
            var isActive = await settingsService.IsActiveOnStartupWithUserSessionAsync();
            IsStartingWithUserSession = isActive;
        }

        #endregion

        #region Events

        public void StartWithUserSessionCbx()
        {
            StartWithUserSessionCbxAsync().SafeFireAndForget(logErrorHandler);
        }

        public bool CanStartWithUserSessionCbx()
        {
            return settingsService.CanOperateOnStartupWithUser();
        }

        private async Task StartWithUserSessionCbxAsync()
        {
            bool enabled;
            if (!IsStartingWithUserSession)
                enabled = await settingsService.ActivateStartProgramWithUserSessionAsync();
            else
                enabled = !(await settingsService.DeactivateStartProgramWithUserSessionAsync());

            IsStartingWithUserSession = enabled;
        }

        #endregion
    }
}