using System;
using System.Windows.Media;
using kaleidoscope_companion.Helpers;
using log4net;

namespace kaleidoscope_companion.Models
{
    /// <summary>
    /// Meta information about an application.
    /// Consists of friendly name, image name, its executable path, and an icon
    /// </summary>
    public class ApplicationMetaInf
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationMetaInf));

        private ApplicationMetaInf(string name, ImageSource icon, string exePath, string imageName)
        {
            Name = name;
            Icon = icon;
            Path = exePath;
            ImageName = imageName;
        }

        public static ApplicationMetaInf FromImageNameOnly(string imageName)
        {
            return new ApplicationMetaInf(imageName, null, null, imageName);
        }

        public static ApplicationMetaInf FromExecutablePathOnly(string exePath)
        {
            var imageName = ExtractImageName(exePath);
            var icon = ExtractIcon(exePath);
            return new ApplicationMetaInf(imageName, icon, exePath, imageName);
        }

        public static ApplicationMetaInf Construct(string name, ImageSource icon, string executablePath)
        {
            var imageName = ExtractImageName(executablePath);
            return new ApplicationMetaInf(name, icon, executablePath, imageName);
        }

        /// <param name="executablePath"></param>
        /// <returns>image name of executable or empty if cannot extract</returns>
        private static string ExtractImageName(string executablePath)
        {
            var imageName = "";
            try
            {
                imageName = System.IO.Path.GetFileNameWithoutExtension(executablePath);
            }
            catch (ArgumentException e)
            {
                Log.Warn("Unable to extract image name from path " + executablePath, e);
            }

            return imageName;
        }

        private static ImageSource ExtractIcon(string executablePath)
        {
            ImageSource icon = null;
            try
            {
                icon = System.Drawing.Icon.ExtractAssociatedIcon(executablePath).ToImageSource();
            }
            catch (Exception e)
            {
                Log.Warn("Unable to extract icon from path " + executablePath, e);
            }

            return icon;
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