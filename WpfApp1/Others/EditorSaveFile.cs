using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using WpfApp1.Asset;
using WpfApp1.Attributes;
using WpfApp1.Logic.UILogic;

namespace WpfApp1
{
    internal interface FileOperation{
        void DoSave(BaseSaveFile basefile);
        void DoLoad(BaseSaveFile basefile);
        void DoPush(BaseSaveFile basefile);
    }
    internal interface FileCenter
    {
        bool Save();
        bool Load();
        void AddOperation(FileOperation op);
    }
    internal struct AllEditiorDataFileStruct
    {
        public List<AssetFileInfo> resource;
        /// <summary>
        /// 属性的数据
        /// </summary>
        public Dictionary<uint, object> control_data;
        public List<ConBaseInfo> control_info;
        public Com_Editor_Setting setting;

        public AllEditiorDataFileStruct(Com_Editor_Setting setting,List<AssetFileInfo> resource, Dictionary<uint, object> control_data, List<ConBaseInfo> control_info)
        {
            this.resource = resource;
            this.control_data = control_data;
            this.control_info = control_info;
            this.setting = setting;
        }
    }
    public class BaseSaveFile { }
internal class EditorSaveFile:BaseSaveFile, FileCenter
    {
        string path;
        List<FileOperation> ops;
        public List<AssetFileInfo> resource;
        /// <summary>
        /// 属性的数据
        /// </summary>
        public Dictionary<uint, object> control_data;
        public List<ConBaseInfo> control_info;
        public Com_Editor_Setting setting;
        public EditorSaveFile(string path)
        {
            ops = new List<FileOperation>();
            this.path = path;
        }
        public void AddOperation(FileOperation op)
        {
            ops.Add(op);
        }
        public static object ProcessJObjectToReal(JObject jobject)
        {
            object result=null;
            if (jobject["type"] == null) return default;
            EControlType type = (EControlType)(int)jobject["type"];
            switch (type)
            {
                case EControlType.Button:
                    result = jobject.ToObject<Button_Attribute>();
                   
                    break;
                case EControlType.Joy:
                    result= jobject.ToObject<Joy_Attribute>();
                    break;
                case EControlType.Group:
                    result = jobject.ToObject<Group_Attribute>();
                    break;
            }
            if (result == null) return default;
            return result;
        }
        public AllEditiorDataFileStruct LoadWithNoCall(string path)
        {
            AllEditiorDataFileStruct all = SaveALoad.Load<AllEditiorDataFileStruct>(path);
            foreach (uint i in all.control_data.Keys)
            {
                JObject jobject = (JObject)all.control_data[i];
                all.control_data[i] = ProcessJObjectToReal(jobject);

            }
            if (all.control_info == null || all.resource == null || all.control_data == null) return default;
            return all;
        }
        public AllEditiorDataFileStruct LoadWithNoCall()
        {
            return LoadWithNoCall(path);
        }
        //控件更新
        public bool Load()
        {
            AllEditiorDataFileStruct all = LoadWithNoCall(path);
            if (all.control_data == null) return false;
            control_data = all.control_data;
            control_info = all.control_info;
            setting = all.setting;
            resource = all.resource;
            for (int i = 0; i < ops.Count; i++)
            {
                ops[i].DoLoad(this);
            }
            control_data = null;
            control_info = null;
            resource = null;
            return true;
        }

        public bool Save()
        {
            for (int i = 0; i < ops.Count; i++)
            {
                ops[i].DoSave(this);
            }
            AllEditiorDataFileStruct all = new AllEditiorDataFileStruct(setting,resource,control_data,control_info);
            return SaveALoad.Save(all,path);
        }
        //添加控件更新
        public bool Push(string path)
        {
            AllEditiorDataFileStruct all = SaveALoad.Load<AllEditiorDataFileStruct>(path);
            foreach (uint i in all.control_data.Keys)
            {
                JObject jobject = (JObject)all.control_data[i];
                if (jobject["type"] == null) return false;
                EControlType type = (EControlType)(int)jobject["type"];
                switch (type)
                {
                    case EControlType.Button:
                        all.control_data[i] = jobject.ToObject<Button_Attribute>();
                        if (all.control_data[i] == null) return false;
                        break;
                    case EControlType.Joy:
                        all.control_data[i] = jobject.ToObject<Joy_Attribute>();
                        if (all.control_data[i] == null) return false;
                        break;
                    case EControlType.Group:
                        all.control_data[i] = jobject.ToObject<Group_Attribute>();
                        if (all.control_data[i] == null) return false;
                        break;
                }

            }
            if (all.control_info == null || all.resource == null || all.control_data == null) return false;
            uint count =(uint) Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.Count);
            ConBaseInfo info;
            Dictionary<uint, object> fixed_control_data=new Dictionary<uint, object>();
            List<uint> group = new List<uint>();
            double minx=double.MaxValue, miny=double.MaxValue;
            for (int id = 0; id < all.control_info.Count; id++,count++)
            {
                
                info = all.control_info[id];
                fixed_control_data[count] = all.control_data[info.con_id];
                group.Add(count);
                if (minx > ((AttributeDataS)fixed_control_data[count]).pos[0]) minx= ((AttributeDataS)fixed_control_data[count]).pos[1];
                if (miny > ((AttributeDataS)fixed_control_data[count]).pos[1]) miny = ((AttributeDataS)fixed_control_data[count]).pos[0];
                info.con_id = count;
            }
            all.control_data = null;
            all.control_data = fixed_control_data;
            all.control_info.Add(new ConBaseInfo(path, "", EControlType.Group) { con_id = count });
            Group_Attribute con = new Group_Attribute();
            con.pos[0] = minx;
            con.pos[1] = miny;
            con.scale[0] = 50;
            con.scale[1] = 50;
            con.myid = count;
            con.childer = group.ToArray();
            all.control_data.Add(count, con);
            control_data = all.control_data;
            control_info = all.control_info;
            string assetpath = path.Substring(0, path.LastIndexOf('\\')+1)+("Asset\\");
            AssetManager manager = Editor_GobalVar.GetIns().assetm;
            string[] used= AssetManager.GetUsedResource(all.control_data);
            AssetFileInfo fileinfo=default;
            for (int i=0;i<used.Length;i++)
            {
                fileinfo = all.resource.Find(o => { return o.ident == used[i]; });
                if (fileinfo.md5!=null){
                    manager.TryAddAsset(assetpath + fileinfo.md5, fileinfo.ident);
                }
                
            }
            for (int i = 0; i < ops.Count; i++)
            {
                ops[i].DoPush(this);
            }
            //resource = all.resource;
            return true;
        }
    }
}
