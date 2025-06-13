using System;
using WpfApp1.Asset;
using WpfApp1.Logic.UILogic;

namespace WpfApp1
{
    /// <summary>
    /// 不安全式，单线程的的数据共享,主打属性部分
    /// </summary>
    internal class Editor_GobalVar
    {
        static Editor_GobalVar vars;
        static object locker = new object();
        public Action<uint, object> callback;
        public AssetManager assetm;
        public BitmapImageCache imagecache;
        public EditorSaveFile file;
        public EditorUi_Logic logic;
        public bool stopatrui = false;//暂停属性数据对属性UI变化而变化
        private Editor_GobalVar()
        {

        }
        public void ClearAll()
        {
            callback = null;
            assetm = null;
            imagecache = null;
            file = null;
            logic = null;
            GC.Collect();
        }
        public static Editor_GobalVar GetIns()
        {
            lock (locker)
            {
                if (vars == null)
                {
                    vars = new Editor_GobalVar();
                }
                return vars;
            }
        }
    }
}
