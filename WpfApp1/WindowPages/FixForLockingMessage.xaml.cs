using System;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace WpfApp1.WindowPages
{
    /// <summary>
    /// FixForLockingMessage.xaml 的交互逻辑
    /// </summary>
    public partial class FixForLockingMessage : Window
    {
        private int hHook;
        private bool down = false;
        private static WindowOperation.HookProc hProc;
        public string classname;
        public IntPtr hwnd=IntPtr.Zero;
        public FixForLockingMessage()
        {
            InitializeComponent();
        }
        IntPtr iP;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hProc = MouseHookProc;
            hHook = WindowOperation.SetWindowsHookEx(14, hProc, IntPtr.Zero, 0);

        }
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WindowOperation.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                if ((Int32)wParam == 0x202 && down)
                {
                    POINT point;
                    if (WindowOperation.GetCursorPos(out point))
                    {
                        hwnd = WindowOperation.WindowFromPoint(point);

                        WindowTitle.Text = "窗口标题:" + WindowOperation.GetWindowTitleEEX(hwnd);
                        classname = (WindowOperation.GetClassNameEEX(hwnd));
                    }
                    down = false;
                    CursorPoint.Visibility = Visibility.Visible;
                    WindowOperation.SystemParametersInfo(0x0057, 0, IntPtr.Zero, 2);
                }
                return WindowOperation.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowOperation.UnhookWindowsHookEx(hHook);
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            down = true;
            CursorPoint.Visibility = Visibility.Hidden;
            iP = WindowOperation.LoadCursorFromFile(@"E:\data\c#\function test\Function Test\Function Test\Resource\WindowSelect.ico");
            WindowOperation.SetSystemCursor(iP, WindowOperation.OCR_NORMAL);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string title = "";
            MessageBoxResult result = MessageBox.Show("1.点击确定以后请再5s内切换至全屏游戏界面。\n2.等待5s过后，软件将会获取到激活的窗口，并且发出\"Windows背景应用音效~\"。届时，可以切换到软件查看结果。", "提示", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Task task = new Task(() =>
                {
                    Thread.Sleep(5000);
                     hwnd = WindowOperation.GetForegroundWindow();
                    title = WindowOperation.GetWindowTitleEEX(hwnd);
                    classname = WindowOperation.GetClassNameEEX(hwnd);
                    SoundPlayer soundPlayer = new SoundPlayer(@"C:\Windows\Media\Windows Background.wav");
                    soundPlayer.Play();
                });
                task.Start();
                task.Wait();
                WindowTitle.Text = "全屏窗口标题:" + title;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1.如果窗口不是全屏窗口，请使用上边的图标，按住鼠标点击拖动以后放置到目标窗口上释放鼠标，将会获取窗口的信息。\n2.若是全屏窗口，点击后5s内切换至全屏游戏，等待5s后将会听到检测音效。然后可返回软件查看结果\n3.软件中间的文字为窗口的标题，在使用（1）方法时尽量拖到窗口标题部分（有些游戏由多个窗口组成）。\n4.若不想继续操作直接关闭窗口即可。若需要完成操作，则点击左上角的确定退出", "帮助");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
