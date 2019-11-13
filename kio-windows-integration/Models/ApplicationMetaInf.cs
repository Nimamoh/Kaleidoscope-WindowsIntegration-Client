using System;
using System.Windows.Media;
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
                Log.Warn("Unable to extract image name from path "+Path, e);
            }

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