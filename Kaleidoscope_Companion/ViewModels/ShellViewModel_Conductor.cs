using System.Threading.Tasks;

namespace kaleidoscope_companion.ViewModels
{
    public partial class ShellViewModel
    {
        private readonly SerialMonViewModel serialMonViewModel;
        private readonly HomeViewModel homeViewModel;
        private readonly ConfigureViewModel configureViewModel;
        private readonly SettingsViewModel settingsViewModel;
        private readonly DebugViewModel debugViewModel;
        
        /**
         * Correspondance index and menu as seen in shell view.
         * XXX: If you see any problem with menu selection, make sure this enum is correctly reflecting
         * the view and don't forget to update SelectedMenuIndex.
         */
        private enum Menuindex
        {
            Home,
            Conf,
            Settings,
            SerialMon,
            Debug
        }
        public async Task NavToHome()
        {
            if (ActiveItem is HomeViewModel) return;
            await ActivateItemAsync(homeViewModel);

            SelectedMenuIndex = (int) Menuindex.Home;
        }

        public async Task NavToConf()
        {
            if (ActiveItem is ConfigureViewModel) return;
            await ActivateItemAsync(configureViewModel);
            
            SelectedMenuIndex = (int) Menuindex.Conf;
        }

        public async Task NavToSettings()
        {
            if (ActiveItem is SettingsViewModel) return;
            await ActivateItemAsync(settingsViewModel);
            
            SelectedMenuIndex = (int) Menuindex.Settings;
        }

        public async Task NavToSerialMon()
        {
            if (ActiveItem is SerialMonViewModel) return;
            await ActivateItemAsync(serialMonViewModel);
            
            SelectedMenuIndex = (int) Menuindex.SerialMon;
        }

        public async Task NavToDebug()
        {
            if (ActiveItem is DebugViewModel) return;
            await ActivateItemAsync(debugViewModel);
            
            SelectedMenuIndex = (int) Menuindex.Debug;
        }
    }
}