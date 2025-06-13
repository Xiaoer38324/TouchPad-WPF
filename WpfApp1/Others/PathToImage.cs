using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class PathToImage : IValueConverter
    {
        BitmapPalette myPalette;
        public PathToImage()
        {
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Transparent);
             myPalette = new BitmapPalette(colors);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string x = (string)value;
            if (File.Exists(x))
            {
                Icon icon = Icon.ExtractAssociatedIcon(x);
                return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());//ConverToImageSource(Icon.ExtractAssociatedIcon(x).ToBitmap());
            }
            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
 
}
