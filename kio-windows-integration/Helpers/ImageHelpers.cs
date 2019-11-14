using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace kio_windows_integration.Helpers
{
    public static class ImageHelpersExtensions
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            var bmap = icon.ToBitmap();
            var hIcon = bmap.GetHicon();

            try
            {
                Int32Rect rect = new Int32Rect(0, 0, bmap.Width, bmap.Height);
                return Imaging
                    .CreateBitmapSourceFromHIcon(hIcon, rect,
                        BitmapSizeOptions.FromWidthAndHeight(bmap.Width, bmap.Height));
            }
            finally
            {
                DeleteObject(hIcon);
            }
        }
    }

    public class ImageHelpers
    {
    }
}