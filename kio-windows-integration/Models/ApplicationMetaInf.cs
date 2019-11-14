using System;
using System.Windows.Media;
using kio_windows_integration.Helpers;
using log4net;

namespace kio_windows_integration.Models
{
    /// <summary>
    /// Meta informations about an application.
    /// Consits of friendly name, image name, its executable path, and an icon
    /// </summary>
    public class ApplicationMetaInf
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationMetaInf));

        public ApplicationMetaInf(string name, ImageSource icon, string exePath)
        {
            Name = name;
            Icon = icon;
            Path = exePath;

            var imageName = "";
            try
            {
                imageName = System.IO.Path.GetFileNameWithoutExtension(Path);
            }
            catch (ArgumentException e)
            {
                Log.Warn("Unable to extract image name from path " + Path, e);
            }

            ImageName = imageName;
        }

        public ApplicationMetaInf(string exePath)
        {
            var imageName = System.IO.Path.GetFileNameWithoutExtension(exePath);
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath).ToImageSource();

            Name = imageName;
            Icon = icon;
            Path = exePath;
            ImageName = imageName;
        }

        /// <summary>
        /// Friendly name of an application
        /// </summary>
        public string Name { get; }

        public ImageSource Icon { get; }

        /// <summary>
        /// The whole path of the executive file
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The image name of the application.
        /// Consists of the the filename of the exe file.
        /// </summary>
        public string ImageName { get; }
    }
}