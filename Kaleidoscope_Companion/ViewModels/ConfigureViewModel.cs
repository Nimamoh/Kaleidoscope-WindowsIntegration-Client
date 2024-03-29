﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using kaleidoscope_companion.Events;
using kaleidoscope_companion.Exceptions;
using kaleidoscope_companion.Helpers;
using kaleidoscope_companion.Models;
using kaleidoscope_companion.Services;
using static System.Math;

namespace kaleidoscope_companion.ViewModels
{
    public partial class ConfigureViewModel : Screen
    {
        private readonly PersistenceService persistenceService;
        private readonly SerialPort keyboardSerialPort;
        private readonly LogErrorHandler logErrorHandler;
        private readonly ISet<ApplicationLayerMapping> appWideAppLayerMappings;
        private readonly IEventAggregator eventAggregator;

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

        private string typingProcessName;

        public string TypingProcessName
        {
            get => typingProcessName;
            set
            {
                typingProcessName = value;
                NotifyOfPropertyChange(nameof(TypingProcessName));
            }
        }

        public ObservableCollection<AppLayerMappingItem> AppLayerMappings { get; } =
            new ObservableCollection<AppLayerMappingItem>();

        public ConfigureViewModel(
            SerialPort keyboardSerialPort, 
            IEventAggregator eventAggregator, 
            LogErrorHandler logErrorHandler,
        PersistenceService persistenceService, 
            ISet<ApplicationLayerMapping> appWideAppLayerMappings)
        {
            this.logErrorHandler = logErrorHandler;
            this.keyboardSerialPort = keyboardSerialPort;
            this.appWideAppLayerMappings = appWideAppLayerMappings;
            this.persistenceService = persistenceService;
            this.eventAggregator = eventAggregator;
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Initialize(cancellationToken).SafeFireAndForget(logErrorHandler);
            return Task.CompletedTask;
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            var apps = await WinApi.QueryInstalledProgramsAsync();
            InstalledApps = new ObservableCollection<ApplicationMetaInf>(apps);

            var maxLayer = -1;
            try
            {
                maxLayer = await WindowsIntegrationFocusApi.TotalLayerCountAsync(keyboardSerialPort);
            }
            catch (WindowsIntegrationFocusApiException e)
            {
                Log.Error("Failed while communicating with keyboard", e);
                await eventAggregator.PublishOnUIThreadAsync(new SerialPortOffline(), cancellationToken);
            }

            AvailableLayers = Enumerable.Range(0, Max(0, maxLayer));

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
            var uiMappings = mappings.Select(mapping =>
            {
                var metaInf = InstalledApps.FirstOrDefault(meta => meta.ImageName == mapping.ProcessName);
                if (metaInf == null && mapping.ExePath != null)
                {
                    var computedMetaInf = ApplicationMetaInf.FromExecutablePathOnly(mapping.ExePath);
                    InstalledApps.AddOrReplaceOnUniqueImageName(computedMetaInf); // Update installed apps
                    metaInf = computedMetaInf;
                }

                return mapping.ToUIModel(metaInf);
            });
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
                AppLayerMappings.Select(item =>
                {
                    var metaInf = InstalledApps.FirstOrDefault(meta => meta.ImageName == item.ImageName);
                    return item.ToModel(metaInf);
                }));

            // Persist to the application
            AppLayerMappingsToAppWide(mappings);

            // Persist on disk
            try {
                await persistenceService.Save(mappings);
            } catch(Exception e) {
                Log.Error("Failed to save the mappings...", e);
            }
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