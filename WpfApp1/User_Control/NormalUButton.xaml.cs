
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsInput;
using WpfApp1.Attributes;
using WpfApp1.Touch;
namespace WpfApp1.User_Control
{
    /// <summary>
    /// NormalUButton.xaml 的交互逻辑
    /// </summary>
    public partial class NormalUButton : UserControl, TouchActivator
    {

        public VirtualKeyCode[] keys;
        public ImageSource Source
        {
            get
            {
                return  TheButton.Source;
            }
            set
            {
                TheButton.Source = value;
            }
        }
        public ImageSource MaskSource
        {
            get
            {
                return (ImageSource)TheButton.OpacityMask.GetValue(ImageBrush.ImageSourceProperty);
            }
            set
            {
                TheButton.OpacityMask.SetValue(ImageBrush.ImageSourceProperty,value);
            }
        }
        public NormalUButton()
        {
            InitializeComponent();
            
        }
        void Mouse_Down(object sender, TouchEventArgs e)
        {
            Touch_Core.KeyDown(keys);
            e.Handled= true;
        }
        void Mouse_Up(object sender, TouchEventArgs e)
        {
            Touch_Core.KeyUp(keys);
            e.Handled = true;
        }
        void TouchActivator.Active(VirtualKeyCode[] key)
        {
            keys = key;
            if (System.Windows.SystemParameters.IsTabletPC)
            {
                TheButton.TouchDown += Mouse_Down;
                TheButton.TouchUp += Mouse_Up;
            }
            else
            {
                TheButton.MouseDown += MMouse_Down;
                TheButton.MouseUp += MMouse_Up;
            }

            
        }
        //测试事件
        void MMouse_Down(object sender, MouseButtonEventArgs e)
        {
            Touch_Core.KeyDown(keys);
            e.Handled = true;
        }
        void MMouse_Up(object sender, MouseButtonEventArgs e)
        {
            Touch_Core.KeyUp(keys);
            e.Handled = true;
        }
        EControlType TouchActivator.GetECT()
        {
            return EControlType.Button;
        }
    }
}
