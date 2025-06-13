using WindowsInput;
using WpfApp1.Attributes;

namespace WpfApp1
{
    internal interface TouchActivator
    {
        internal void Active(VirtualKeyCode[] key);
        internal EControlType GetECT();
    }

}
