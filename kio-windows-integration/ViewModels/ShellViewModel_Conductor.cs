namespace kio_windows_integration.ViewModels
{
    public partial class ShellViewModel
    {
        private readonly SerialMonViewModel serialMonViewModel;
        private readonly HomeViewModel homeViewModel;
        private readonly ConfigureViewModel configureViewModel;
        private readonly DebugViewModel debugViewModel;
        
        public void NavToHome()
        {
            if (ActiveItem is HomeViewModel) return;
            ActivateItem(homeViewModel);
        }

        public void NavToConf()
        {
            if (ActiveItem is ConfigureViewModel) return;
            ActivateItem(configureViewModel);
        }

        public void NavToSerialMon()
        {
            if (ActiveItem is SerialMonViewModel) return;
            ActivateItem(serialMonViewModel);
        }

        public void NavToDebug()
        {
            if (ActiveItem is DebugViewModel) return;
            ActivateItem(debugViewModel);
        }
    }
}