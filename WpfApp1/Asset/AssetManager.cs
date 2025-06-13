using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WpfApp1.Attributes;

namespace WpfApp1.Asset
{
    public enum AssetFileType
    {
        UNDEF,IMAGE
    }
   public struct AssetFileInfo
    {
        public string ident { get; set; }
        public AssetFileType type { get; set; }
        public string md5 { get; set; }

        public AssetFileInfo(string ident, AssetFileType type, string md5)
        {
            this.ident = ident;
            this.type = type;
            this.md5 = md5;
        }
    }
    public enum AssetOperResult
    {
        ExitTheSame,Fail,Success,ExitTheSameIdent
    }
    //引用计数的对象是工程文件
    public class AssetManager:FileOperation
    {
        List<AssetFileInfo> list;
        string AssetPath = "\\Asset\\";
        string AssetRootPath = @"E:\data\tptest";
        public AssetManager(string AssetRootPath) {
            list = new List<AssetFileInfo>();
            this.AssetRootPath = AssetRootPath;
            if (!Directory.Exists(AssetRootPath + AssetPath)) Directory.CreateDirectory(AssetRootPath + AssetPath);
        }
        public bool ReSetResourceList(List<AssetFileInfo> list)
        {
            if (list != null)
            {
                this.list = list;
                return true;
            }
            return false;

        }
        public bool Remove(string identity)
        {
            try
            {
                AssetFileInfo info = list.Find(t => (t.ident == identity));
                if (info.md5 == null) return false;
                if(File.Exists(AssetRootPath + AssetPath + info.md5))
                File.Delete(AssetRootPath + AssetPath + info.md5);
                list.Remove(info);
                return true;
            }catch(IOException io)
            {
                MessageBox.Show(identity+":当前文件有对象正在使用，无法删除");
                return false;
            }
        }
        public bool RemoveByMd5(string md5)
        {
            try
            {
                AssetFileInfo info = list.Find(t => (t.md5 == md5));
                if (info.md5 == null) return false;
                if (File.Exists(AssetRootPath + AssetPath + info.md5))
                    File.Delete(AssetRootPath + AssetPath + info.md5);
                list.Remove(info);
                return true;
            }
            catch (IOException io)
            {
                MessageBox.Show(md5 + ":当前文件有对象正在使用，无法删除");
                return false;
            }
        }
        public List<AssetFileInfo>  GetResourceList()
        {
            return list;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath">路径</param>
        /// <param name="identification">资源标识</param>
        /// <returns></returns>
        public AssetOperResult TryAddAsset(string filepath, string identification)
        {
            try
            {
                string md5 = GetMD5HashFromFile(filepath);
                if (list.Exists(t => (t.md5 == md5))) {
                    return AssetOperResult.ExitTheSame;
                }
                if (list.Exists(t => (t.ident == identification))) return AssetOperResult.ExitTheSameIdent;
              string assetpath = AssetRootPath + AssetPath + md5;
                if(!File.Exists(assetpath))
                File.Copy(filepath, assetpath);
                list.Add(new AssetFileInfo(identification,GetFileType(filepath.Substring(filepath.LastIndexOf('.')+1)),md5));
            return AssetOperResult.Success;
            }
            catch
            {
                identification = "";
                return AssetOperResult.Fail;
            }
           

        }
        private AssetFileType GetFileType(string type)
        {
            if (type == "png" || type == "jpg") return AssetFileType.IMAGE;
            return AssetFileType.UNDEF;
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 =MD5.Create();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        //public static string IsSystemAsset(string ident)
        //{
        //    switch (ident)
        //    {
        //        case "ButtonDefImage":
        //            return @"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\ButtonDefImage.png";
        //        case "ButtonDefMask":
        //            return @"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\ButtonDefMask.png";
        //        case "JoyBGDef":
        //            return @"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\JoyBGDef.png";
        //        case "GroupDefImage":
        //            return @"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\GroupDefImage.png";
        //    }
        //    return "";
        //}
        public static string IsSystemAsset(string ident)
        {
            switch (ident)
            {
                case "ButtonDefImage":
                    return @"pack://application:,,,/Resource/ButtonDefImage.png";
                case "ButtonDefMask":
                    return @"pack://application:,,,/Resource/ButtonDefMask.png";
                case "JoyBGDef":
                    return @"pack://application:,,,/Resource/JoyBGDef.png";
                case "GroupDefImage":
                    return @"pack://application:,,,/Resource/GroupDefImage.png";
            }
            return "";
        }
        /// <summary>
        /// 申请一个使用权
        /// </summary>
        /// <param name="identification"></param>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public bool TryGetAsset(string identification,out string assetpath)
        {
            assetpath = IsSystemAsset(identification);
            if (!String.IsNullOrEmpty(assetpath)) return true;
            AssetFileInfo info = list.Find(t => t.ident == identification);
            if (info.md5==null)//默认值
            {
                assetpath = "";
                return false;
            }
            else
            {
                assetpath = AssetRootPath + AssetPath+ info.md5;
                return true;
            }
        }
        public string GetMd5ByIdentification(string id)
        {
            AssetFileInfo info = list.Find(o => { return o.ident == id; });
            if (info.md5 != null)
            {
                return info.md5;
            }
            return null;
        }
        public void DoSave(BaseSaveFile basefile)
        {
            ((EditorSaveFile)basefile).resource = list;
        }

        public void DoLoad(BaseSaveFile basefile)
        {
            list = ((EditorSaveFile)basefile).resource;
        }
        //添加控件更新
        public static string[] GetUsedResource(Dictionary<uint, object> control_data)
        {
            List<string> resource = new List<string>();
            Type type;
            foreach (uint i in control_data.Keys)
            {
                type = control_data[i].GetType();
                if (type == typeof(Button_Attribute))
                {
                    Button_Attribute attr = ((Button_Attribute)control_data[i]);
                    if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.background))&&!resource.Contains(attr.background))
                        resource.Add(attr.background);
                    if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.mask))&& !resource.Contains(attr.mask))
                        resource.Add(attr.mask);
                }
                else if (type == typeof(Joy_Attribute))
                {
                    Joy_Attribute attr = ((Joy_Attribute)control_data[i]);
                    if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.background))&& !resource.Contains(attr.background))
                        resource.Add(attr.background);
                    if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.gmask)) && !resource.Contains(attr.gmask))
                        resource.Add(attr.gmask);
                    if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.gsource)) && !resource.Contains(attr.gsource))
                        resource.Add(attr.gsource);
                }
            }
            return resource.ToArray();
        }

        public void DoPush(BaseSaveFile basefile)
        {
          
        }
    }
}
