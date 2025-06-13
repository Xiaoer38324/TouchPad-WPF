using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WpfApp1
{

    public abstract class BitmapImageCache
    {
        protected struct ImageInfo
        {
           public BitmapImage image;
           public uint cite;

            public ImageInfo(BitmapImage image, uint cite)
            {
                this.image = image;
                this.cite = cite;
            }
        }
       protected  Dictionary<string, ImageInfo> cache;
        public BitmapImageCache()
        {
            cache = new Dictionary<string, ImageInfo>();
        }
        public abstract bool FreeImage(string ident);
        protected abstract BitmapImage GetImage(string imagePath);
        public abstract BitmapImage GetBitmap(string ident, string oldident = "");
    }
}
