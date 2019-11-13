using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using kio_windows_integration.Helpers;
using kio_windows_integration.Models;
using kio_windows_integration.Services;

namespace kio_windows_integration.ViewModels
{
    public partial class ConfigureViewModel : Screen
    {
        private readonly PersistenceService persistenceService;
        private readonly SerialPort keyboardSerialPort;
        private readonly ErrorMangementHelper errorHelper;
        private readonly ISet<ApplicationLayerMapping> appWideAppLayerMappings;

        private IEnumerable<int> availableLayers;

        public IEnumerable<int> AvailableLayers
        {
            get => availableLayers;
            set
            {
                availableLayers = value;
                NotifyOfPropertyChange(nameof(AvailableLayers));
            }
        }

        private ObservableCollection<ApplicationMetaInf> installedApps = new ObservableCollection<ApplicationMetaInf>();

        public ObservableCollection<ApplicationMetaInf> InstalledApps
        {
            get => installedApps;
            set
            {
                installedApps = value;
                NotifyOfPropertyChange(nameof(InstalledApps));
            }
        }

        public EditableAppLayerMapping AppLayerMappingInEdition { get; } = new EditableAppLayerMapping();

        private string helpInEditing;

        public string HelpInEditing
        {
            get => helpInEditing;
            set
            {
                helpInEditing = value;
                NotifyOfPropertyChange(nameof(HelpInEditing));
            }
        }

        public ObservableCollection<AppLayerMappingItem> AppLayerMappings { get; } =
            new ObservableCollection<AppLayerMappingItem>();

        public ConfigureViewModel(SerialPort keyboardSerialPort, ErrorMangementHelper errorHelper,
            PersistenceService persistenceService, ISet<ApplicationLayerMapping> appWideAppLayerMappings)
        {
            this.errorHelper = errorHelper;
            this.keyboardSerialPort = keyboardSerialPort;
            this.appWideAppLayerMappings = appWideAppLayerMappings;
            this.persistenceService = persistenceService;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            var apps = await WinApi.QueryInstalledProgramsAsync();
            InstalledApps = new ObservableCollection<ApplicationMetaInf>(apps);

            var maxLayer = await
                errorHelper.NotifyWholeAppOnKeyboardCommunicationLoss(
                    async () => await WindowsIntegrationFocusApi.TotalLayerCountAsync(keyboardSerialPort),
                    Task.FromResult(-1));
            AvailableLayers = Enumerable.Range(0, maxLayer - 1);

            await LoadMappings();
        }

        /// <summary>
        /// Load mappings (ui and app wide) from persistence model
        /// </summary>
        private async Task LoadMappings()
        {
            var mappings = await persistenceService.load();
            
            AppLayerMappingsToAppWide(mappings);
            
            AppLayerMappings.Clear();
            var uiMappings = mappings.Select(mapping => mapping.ToUIModel(InstalledApps));
            foreach (var item in uiMappings)
            {
                AppLayerMappings.Add(item);
            }
        }

        /// <summary>
        /// Persists the mappings according to the UI model
        /// on the application and on the disk
        /// </summary>
        private async Task PersistMappings()
        {
            var mappings = new HashSet<ApplicationLayerMapping>(
                AppLayerMappings.Select(item => item.ToModel()));
            
            // Persist to the application
            AppLayerMappingsToAppWide(mappings);

            // Persist on disk
            await persistenceService.Save(mappings);
        }

        /// <summary>
        /// Persist the mappings app wide
        /// </summary>
        private void AppLayerMappingsToAppWide(ISet<ApplicationLayerMapping> mappings)
        {
            // Copy on application
            appWideAppLayerMappings.Clear();
            foreach (var mapping in mappings)
            {
                appWideAppLayerMappings.Add(mapping);
            }
        }
    }
}