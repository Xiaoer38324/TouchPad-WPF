using WindowsInput;

namespace WpfApp1.Touch
{
    internal static class Touch_Core
    {
       static InputSimulator ips=new InputSimulator();
        internal static void KeyDown(VirtualKeyCode[] keys)
        {
            ips.Keyboard.KeyDown(keys);
        }
        internal static void KeyDown(VirtualKeyCode keys)
        {
            ips.Keyboard.KeyDown(keys);
        }
        internal static void KeyUp(VirtualKeyCode[] keys)
        {
            ips.Keyboard.KeyUp(keys);
        }
        internal static void KeyUp(VirtualKeyCode keys)
        {
            ips.Keyboard.KeyUp(keys);
        }
    }
}
