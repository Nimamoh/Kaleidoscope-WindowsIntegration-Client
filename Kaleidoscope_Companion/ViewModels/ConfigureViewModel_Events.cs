using System.Windows.Input;
using kaleidoscope_companion.Models;
using log4net;
using static kaleidoscope_companion.Helpers.ErrorMangementHelper;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace kaleidoscope_companion.ViewModels
{
    public partial class ConfigureViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigureViewModel));

        #region Guards

        public bool CanAddItem
        {
            get
            {
                var mapping = EditionToMapping(AppLayerMappingInEdition);
                return AppLayerMappingInEdition.LayerNumber != -1
                       && AppLayerMappingInEdition.SelectedApp != -1
                       && !AppLayerMappings.Contains(mapping);
            }
        }

        protected override void OnViewReady(object view)
        {
            AppLayerMappings.CollectionChanged += async delegate { await PersistMappings(); };
            AppLayerMappings.CollectionChanged += delegate { NotifyOfPropertyChange(nameof(CanAddItem)); };
            AppLayerMappingInEdition.PropertyChanged += delegate { NotifyOfPropertyChange(nameof(CanAddItem)); };
            AppLayerMappingInEdition.PropertyChanged += delegate { HelpInEditing = MakeHelpInEditing(); };
        }

        #endregion

        public string MakeHelpInEditing()
        {
            const int unset = EditableAppLayerMapping.defaultIndexUnset;

            var uiMapping = EditionToMapping(AppLayerMappingInEdition);

            string GetAppHelpMsg(string appName) => (appName != null) ? $"When {appName} is focused" : "";
            string GetLayerHelpMessage(int layerN) => (layerN > unset) ? $"layer {layerN} is activated" : "";

            string GetLinkingWord(string first, string second) =>
                (!string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(second))
                    ? "then"
                    : "";

            var appHelpMsg = GetAppHelpMsg(uiMapping?.AppName);
            var layerHelpMessage = GetLayerHelpMessage(AppLayerMappingInEdition?.LayerNumber ?? unset);
            var linkingWord = GetLinkingWord(appHelpMsg, layerHelpMessage);

            return $"{appHelpMsg} {linkingWord} {layerHelpMessage}".Trim();
        }

        public void AddItem()
        {
            var uiMapping = EditionToMapping(AppLayerMappingInEdition);
            if (uiMapping == null)
                return;

            AppLayerMappings.Add(uiMapping);
            AppLayerMappingInEdition.reset();
        }

        public void DeleteItem(AppLayerMappingItem item)
        {
            AppLayerMappings.Remove(item);
        }

        public void ChooseCustomExe()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Programs (.exe)|*.exe";
            var result = ofd.ShowDialog();
            
            if (!result ?? false)
                return;

            var path = ofd.FileName;
            var metaInf = ApplicationMetaInf.FromExecutablePathOnly(path);

            InstalledApps.AddOrReplaceOnUniqueImageName(metaInf);
            AppLayerMappingInEdition.SelectedApp = InstalledApps.IndexOf(metaInf);
        }

        public void HandleTypingProcessName(System.Windows.Input.KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs != null && keyEventArgs.Key == Key.Enter)
            {
                var metaInf = ApplicationMetaInf.FromImageNameOnly(TypingProcessName);
                
                InstalledApps.AddOrReplaceOnUniqueImageName(metaInf);
                AppLayerMappingInEdition.SelectedApp = InstalledApps.IndexOf(metaInf);

                TypingProcessName = "";
            }
        }

        private AppLayerMappingItem EditionToMapping(EditableAppLayerMapping inEdition)
        {
            return Silently(delegate
                {
                    var app = InstalledApps[AppLayerMappingInEdition.SelectedApp];
                    return inEdition.ToUIModel(app);
                },
                null);
        }
    }
}