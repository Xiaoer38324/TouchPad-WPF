using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WindowsInput;
using WpfApp1.Asset;

namespace WpfApp1
{
    internal class Touch_BitmapImageCache : BitmapImageCache
    {
        Dictionary<string, BitmapImage> map;
        public VirtualKeyCode nextcode = VirtualKeyCode.None;
        internal Touch_BitmapImageCache():base()
        {
            cache = null;
            map = new Dictionary<string, BitmapImage>();
        }
        public void Put(string md5,BitmapImage image)
        {
            map.Add(md5, image);
        }
        public override bool FreeImage(string ident)
        {
            throw new NotImplementedException();
        }
        bool ReplaceCodeRange(VirtualKeyCode code)
        {
            int codeint = (int)nextcode;
                if (codeint >= 49 && codeint <= 90)
                return true;
            else if (code == VirtualKeyCode.LBUTTON || code == VirtualKeyCode.MBUTTON || code == VirtualKeyCode.RBUTTON)//鼠标左右中
            {
                return true;
            }
            else if (codeint >= 37 && codeint <= 40)//上下左右
                return true;
            else if (codeint >= 16 && codeint <= 18)//ctrl shift alt
                return true;
            else if (code == VirtualKeyCode.SPACE || code == VirtualKeyCode.TAB || code == VirtualKeyCode.ESCAPE)
                return true;
            return false;
        }
        public override BitmapImage GetBitmap(string ident, string oldident = "")
        {
            if (nextcode!=VirtualKeyCode.None&& ReplaceCodeRange(nextcode))
            {
                if (ident == "ButtonDefImage")
                {
                    string newident = Enum.GetName(nextcode);
                    if (newident != null)
                    {
                        ident = newident;
                        if (!map.ContainsKey(ident))
                        {
                           
                            newident = @"pack://application:,,,/Resource/ButtonImage/" + ident+".png";
                            BitmapImage image = new BitmapImage(new Uri(newident));
                            map.Add(ident, image);
                            return image;
                        }
                    }
                    nextcode = VirtualKeyCode.None;
                }
                
            }
             if (map.ContainsKey(ident))
                return map[ident];
            else
            {
                string path = AssetManager.IsSystemAsset(ident);
                if ( String.IsNullOrEmpty(path)==false)
                {
                    BitmapImage image = new BitmapImage(new Uri(path));
                    map.Add(ident,image);
                    return image;
                }
            }
            return null;
        }
        protected override BitmapImage GetImage(string imagePath)
        {
            throw new NotImplementedException();
        }
    }
}
