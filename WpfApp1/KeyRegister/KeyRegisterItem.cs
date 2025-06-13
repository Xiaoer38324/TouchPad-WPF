using System.Windows.Controls;
using WindowsInput;

namespace WpfApp1.Touch
{
    internal abstract class KeyRegisterItem
    {
        protected abstract void InitGetView();//应包含注册事件
        internal abstract VirtualKeyCode[] GetKey();
        internal abstract Panel GetPanel();
        internal abstract void Clear();
        internal abstract void Reset(VirtualKeyCode[] keys);
        internal abstract bool IsVailed();
    }
}
