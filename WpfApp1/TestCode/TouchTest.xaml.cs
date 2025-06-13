using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using WpfApp1.Attributes;
using WpfApp1.Touch;

namespace WpfApp1.WindowPages
{
    /// <summary>
    /// TouchTest.xaml 的交互逻辑
    /// </summary>
    public partial class TouchTest : Window
    {
        const int WS_EX_TOOLWINDOW = 0x80;
        const int HWND_TOPMOST = -1;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;
        IntPtr handle;
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return Environment.Is64BitProcess
                ? GetWindowLong64(hWnd, nIndex)
                : GetWindowLong32(hWnd, nIndex);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return Environment.Is64BitProcess
                ? SetWindowLong64(hWnd, nIndex, dwNewLong)
                : SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        [DllImport("Dwmapi.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();
        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);
  
        public TouchTest()
        {
            InitializeComponent();
        }
        private bool Add(Control c,object obj)
        {
            Test.Children.Add(c);
            return true;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct STYLESTRUCT
        {
            public int styleOld;
            public int styleNew;
        }
        private void ProcessTransparency()
        {
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
            {
                //想要让窗口透明穿透鼠标和触摸等，需要同时设置 WS_EX_LAYERED 和 WS_EX_TRANSPARENT 样式，
                //确保窗口始终有 WS_EX_LAYERED 这个样式，并在开启穿透时设置 WS_EX_TRANSPARENT 样式
                //但是WPF窗口在未设置 AllowsTransparency = true 时，会自动去掉 WS_EX_LAYERED 样式（在 HwndTarget 类中)，
                //如果设置了 AllowsTransparency = true 将使用WPF内置的低性能的透明实现，
                //所以这里通过 Hook 的方式，在不使用WPF内置的透明实现的情况下，强行保证这个样式存在。
                if (msg == (int)0x007C && (long)wParam == GWL_EXSTYLE)
                {
                    STYLESTRUCT styleStruct = (STYLESTRUCT)Marshal.PtrToStructure(lParam, typeof(STYLESTRUCT));
                    styleStruct.styleNew |= 0x00080000;
                    Marshal.StructureToPtr(styleStruct, lParam, false);
                    handled = true;
                }
                return IntPtr.Zero;
            });
            SetTransparentNotHitThrough();

        }
        Touch_Normalization normal;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Touch_LoadAndSave save = new Touch_LoadAndSave(@"D:\g.tlay");
            Touch_BitmapImageCache cache;
            LayoutInfo info;
            if (!save.Load(out cache, out info)) return;
            Editor_GobalVar.GetIns().imagecache = cache;
             normal = new Touch_Normalization(Add, info, Test.ActualWidth, Test.ActualHeight);
            normal.Operation();
            handle = new WindowInteropHelper(this).Handle;
            SetKeyBoardMode();
           

            // ProcessTransparency();
        } 
         public void SetTransparentHitThrough()
        {
            if (DwmIsCompositionEnabled())
            {

                var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | 0x00000020L));
                SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            else
            {
                Background = Brushes.Transparent;
            }
        }

        /// <summary>
        /// 设置点击命中，不会穿透到后面的窗口
        /// </summary>
        public void SetTransparentNotHitThrough()
        {
            if (DwmIsCompositionEnabled())
            {
                var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
                SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW & ~0x00000020L));
                SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            else
            {
                Background = Brushes.Transparent;
            }
        }
       public void SetKeyBoardMode()
        {
            
            var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW));
            SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(normal!=null)
            normal.ReSize(Test,e.NewSize.Width,e.NewSize.Height);
        }

        private void Window_StylusMove(object sender, StylusEventArgs e)
        {

        }
    }
}
