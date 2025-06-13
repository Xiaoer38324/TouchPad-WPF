namespace WpfApp1
{
    struct SoftWare_Setting
    {
        public string datapath;
        public string cache;
    }
    internal class SoftWare_GlobalVar
    {
        static SoftWare_GlobalVar vars;
        static object locker = new object();

        SoftWare_Setting t_setting;
        internal SoftWare_Setting setting {  get { return t_setting; } }
        private SoftWare_GlobalVar()
        {

        }
        public static SoftWare_GlobalVar GetIns()
        {
            lock (locker)
            {
                if (vars == null) vars = new SoftWare_GlobalVar();
                return vars;
            }
        }
        public void PutSetting(SoftWare_Setting setting)
        {
            t_setting = setting;
        }
    }
}
