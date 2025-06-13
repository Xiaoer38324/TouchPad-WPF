using System;
using WpfApp1.User_Control;

namespace WpfApp1
{
    internal  class SpecialUIChecker
    {
       static object obj=new object();
       static  SpecialUIChecker checker;
        //分别代表着 位移 缩放 旋转
        bool[] enabletrs;

        public SpecialUIChecker()
        {
            enabletrs = new bool[] {true,true,true};
        }
        public static SpecialUIChecker GetIns()
        {
            lock (obj)
            {
                if (checker == null) checker = new SpecialUIChecker();
                return checker;
            }
        }
        public bool CheckSpecial(Type type)
        {
            if (type == typeof(Con_Editor_Selector))
            {
              
                return true;
            }
            return false;
        }
        public bool CheckSpecial(Type type,out uint id)
        {
            if (type == typeof(Con_Editor_Selector))
            {
                id = uint.MaxValue;
                return true;
            }
            id = 0;
            return false;
        }
        public  bool[] EnabelTSRBind(Type type)
        {
            if (type == typeof(Con_Editor_Selector)) {
                enabletrs[0] = true;
                enabletrs[1] = false;
                enabletrs[2] = false;
            }
            else if (type == typeof(Con_Editor_Group))
            {
                enabletrs[0] = true;
                enabletrs[1] = true;
                enabletrs[2] = false;
            }
            else
            {
                enabletrs[0] = true;
                enabletrs[1] = true;
                enabletrs[2] = true;
            }
            return enabletrs;
        }
    }
}
