using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using kio_windows_integration.Models;
using static kio_windows_integration.ViewModels.ConfigureViewModel;

namespace kio_windows_integration.ViewModels
{
    static class ConfigureViewModelExtensions
    {
        #region Helper UI data to model

        /// <summary>
        /// Convert an <see cref="EditableAppLayerMapping" /> to an <see cref="AppLayerMappingItem"/>
        /// completing informations with <see cref="ApplicationMetaInf"/>
        /// </summary>
        public static AppLayerMappingItem ToUIModel(this EditableAppLayerMapping inEdition,
            ApplicationMetaInf applicationMetaInf)
        {
            var layer = inEdition.LayerNumber;
            return applicationMetaInf.ToUiModel(layer);
        }

        /// <summary>
        /// Creates a <see cref="AppLayerMappingItem"/> from an <see cref="ApplicationMetaInf"/> and a layer number
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="layerN"></param>
        /// <returns></returns>
        public static AppLayerMappingItem ToUiModel(this ApplicationMetaInf meta, int layerN)
        {
            return new AppLayerMappingItem(meta?.Icon, meta?.Name, meta?.ImageName, layerN);
        }

        /// <summary>
        /// Creates a <see cref="AppLayerMappingItem"/> from <see cref="ApplicationLayerMapping" />
        /// using <see cref="ApplicationMetaInf"/>s to complete informations
        /// </summary>
        public static AppLayerMappingItem ToUIModel(this ApplicationLayerMapping mapping,
            ApplicationMetaInf appMetaInf)
        {
            if (mapping == null)
                return null;

            return new AppLayerMappingItem(appMetaInf?.Icon, 
                appMetaInf?.Name ?? mapping.ProcessName, 
                mapping.ProcessName, 
                mapping.Layer);
        }

        /// <summary>
        /// Convert a <see cref="AppLayerMappingItem"/> to a <see cref="ApplicationLayerMapping"/>
        /// using <see cref="ApplicationMetaInf"/> to complete additionnal informations
        /// </summary>
        public static ApplicationLayerMapping ToModel(this AppLayerMappingItem item, ApplicationMetaInf metaInf)
        {
            return new ApplicationLayerMapping(item.ImageName, item.LayerNumber, metaInf?.Path);
        }

        /// <summary>
        /// Add (or replace first occurence of meta inf) based on ImageName uniqueness
        /// </summary>
        public static void AddOrReplaceOnUniqueImageName(this ObservableCollection<ApplicationMetaInf> collection, ApplicationMetaInf item)
        {
            var existing = collection.FirstOrDefault(meta => meta.ImageName == item.ImageName);
            if (existing != null)
                collection.Remove(existing);
            collection.Add(item);
        }

        #endregion
    }

    public partial class ConfigureViewModel
    {
        /// <summary>
        /// Represent a mapping between an application and a keyboard layer (UI model)
        /// </summary>
        public class AppLayerMappingItem
        {
            public ImageSource AppIcon { get; }

            /// <summary>
            /// Friendly application name
            /// </summary>
            public string AppName { get; }

            /// <summary>
            /// Image name, corresponds to the process name
            /// </summary>
            public string ImageName { get; }

            /// <summary>
            /// Keyboard layer number
            /// </summary>
            public int LayerNumber { get; }

            public AppLayerMappingItem(ImageSource appIcon, string appName, string imageName, int layerNumber)
            {
                AppIcon = appIcon;
                AppName = appName;
                ImageName = imageName;
                LayerNumber = layerNumber;
            }


            #region EqualsHashCode

            protected bool Equals(AppLayerMappingItem other)
            {
                return AppName == other.AppName && LayerNumber == other.LayerNumber;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((AppLayerMappingItem) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((AppName != null ? AppName.GetHashCode() : 0) * 397) ^ LayerNumber;
                }
            }

            #endregion
        }

        /// <summary>
        /// Represent a mapping between an application and a keyboard layer (UI editing forms model)
        /// </summary>
        public class EditableAppLayerMapping : PropertyChangedBase
        {
            public const int defaultIndexUnset = -1;

            private int selectedApp;

            public int SelectedApp
            {
                get => selectedApp;
                set
                {
                    selectedApp = value;
                    NotifyOfPropertyChange(nameof(SelectedApp));
                }
            }

            private int layerNumber;

            public int LayerNumber
            {
                get => layerNumber;
                set
                {
                    layerNumber = value;
                    NotifyOfPropertyChange(nameof(LayerNumber));
                }
            }

            public void reset()
            {
                SelectedApp = defaultIndexUnset;
                LayerNumber = defaultIndexUnset;
            }

            public EditableAppLayerMapping(int selectedApp = defaultIndexUnset, int layerNumber = defaultIndexUnset)
            {
                SelectedApp = selectedApp;
                LayerNumber = layerNumber;
            }
        }
    }
}