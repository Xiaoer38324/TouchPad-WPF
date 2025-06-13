using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    internal class Editor_BitmapImageCache:BitmapImageCache
    {
    
        public override bool FreeImage(string ident)
        {
            if (!cache.ContainsKey(ident)) return false;
            ImageInfo info = cache[ident];
            info.cite--;
            if (info.cite <= 0)
            {
                cache.Remove(ident);
            }
            else cache[ident] = info;
            return true;
        }
        protected override BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    ms.Dispose();
                }
            }
            return bitmap;
        }
        protected  BitmapImage GetImage2(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            return bitmap;
        }
        public override BitmapImage GetBitmap(string ident, string oldident = "")
        {
            if (!String.IsNullOrEmpty(oldident))
                FreeImage(oldident);
            if (cache.ContainsKey(ident))
            {
                ImageInfo info = cache[ident];
                info.cite++;
                cache[ident] = info;
                return cache[ident].image;
            }
            else
            {
                string imagepath;
                if (Editor_GobalVar.GetIns().assetm.TryGetAsset(ident, out imagepath))
                {
                    if(imagepath.StartsWith("pack"))
                        cache[ident] = new ImageInfo(GetImage2(imagepath), 1);
                    else
                        cache[ident] = new ImageInfo(GetImage(imagepath), 1);
                    return cache[ident].image;
                }
                else
                    return null;
            }

        }
    }
}
