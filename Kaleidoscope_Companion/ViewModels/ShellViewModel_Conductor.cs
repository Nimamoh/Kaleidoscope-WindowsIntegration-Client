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
        private enum MENUINDEX
        {
            HOME,
            CONF,
            SETTINGS,
            SERIAL_MON,
            DEBUG
        }
        public void NavToHome()
        {
            if (ActiveItem is HomeViewModel) return;
            ActivateItem(homeViewModel);

            SelectedMenuIndex = (int) MENUINDEX.HOME;
        }

        public void NavToConf()
        {
            if (ActiveItem is ConfigureViewModel) return;
            ActivateItem(configureViewModel);
            
            SelectedMenuIndex = (int) MENUINDEX.CONF;
        }

        public void NavToSettings()
        {
            if (ActiveItem is SettingsViewModel) return;
            ActivateItem(settingsViewModel);
            
            SelectedMenuIndex = (int) MENUINDEX.SETTINGS;
        }

        public void NavToSerialMon()
        {
            if (ActiveItem is SerialMonViewModel) return;
            ActivateItem(serialMonViewModel);
            
            
            SelectedMenuIndex = (int) MENUINDEX.SERIAL_MON;
        }

        public void NavToDebug()
        {
            if (ActiveItem is DebugViewModel) return;
            ActivateItem(debugViewModel);
            
            SelectedMenuIndex = (int) MENUINDEX.DEBUG;
        }
    }
}