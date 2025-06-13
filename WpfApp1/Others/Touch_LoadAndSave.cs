using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Media.Imaging;
using WpfApp1.Touch;

namespace WpfApp1
{
    internal class Touch_LoadAndSave
    {
        string path;
        public Touch_LoadAndSave(string path)
        {
            this.path = path;
        }
        public bool Load(out Touch_BitmapImageCache cache,out LayoutInfo info)
        {
            cache = new Touch_BitmapImageCache();
            info = default;
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                BitmapImage image;
                Stream stream=null;
                try
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        stream = entry.Open();
                        if (!entry.Name.Equals("layout"))
                        {
                            image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                            Thread.Sleep(1);
                          // image.Freeze();
                            cache.Put(entry.Name, image);
                        }
                        else
                        {
                           info= SaveALoad.Load<LayoutInfo>(stream);
                            for (int i=0;i<info.attributes.Count;i++)
                            {
                                info.attributes[i]=EditorSaveFile.ProcessJObjectToReal((JObject)info.attributes[i]);
                            }
                        }
                      
                    }
                }
                catch(Exception io)
                {
                    return false;
                }
                }

            return true;
        }
    }
}
