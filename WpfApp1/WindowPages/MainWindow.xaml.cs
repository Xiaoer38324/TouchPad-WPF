using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsInput;
using WpfApp1.Touch;
using WpfApp1.WindowPages;

namespace WpfApp1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 设置一个新的窗口样式
        /// </summary>
        public const int GWL_STYLE = -16;

        /// <summary>
        /// 标题
        /// </summary>
        public const uint WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */

        /// <summary>
        /// 系统菜单
        /// </summary>
        public const uint WS_SYSMENU = 0x00080000;


        /// <summary>
        /// 设置调整窗口大小的厚的结构的窗口。
        /// </summary>
        public const uint WS_THICKFRAME = 0x00040000;

        /// <summary>
        /// 创建一个可调边框的窗口，与 WS_THICKFRAME 风格相同
        /// </summary>
        public const uint WS_SIZEBOX = WS_THICKFRAME;
        const int WS_EX_TOOLWINDOW = 0x80;
        const int HWND_TOPMOST = -1;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int GWL_EXSTYLE = -20;
        InputSimulator ips;
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        private static extern bool MoveWindow(IntPtr hWnd, int x,int y,int width,int height,bool refreash=true);

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
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern bool SetParent(IntPtr child,IntPtr father);
        public MainWindow()
        {
            InitializeComponent();
          // SourceInitialized += OnSourceInitialized;   
        }
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            //var handle = new WindowInteropHelper(this).Handle;
            //var exstyle = GetWindowLong(handle, GWL_EXSTYLE);
            //SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(exstyle.ToInt32() | WS_EX_NOACTIVATE| WS_EX_TOOLWINDOW));
            //SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            
        }
        private void Test_W_Click_D(object sender, RoutedEventArgs e)
        {
            ips.Keyboard.KeyDown(VirtualKeyCode.VK_W);
          
        }
        private void Test_W_Click_U(object sender, RoutedEventArgs e)
        {
            ips.Keyboard.KeyUp(VirtualKeyCode.VK_W);
            //  MessageBox.Show("Up");
        }
        /// <summary>
        /// 设置点击穿透到后面透明的窗口
        /// </summary>
        ScaleTransform xx;
        Line line;
        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0)
            {
                // Get the Type of InputManager.  
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.  
                object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.  
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.  
                    while (devices.Count > 0)
                    {
                        // Remove the first tablet device in the devices collection.  
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, stylusLogic, new object[] { (uint)0 });
                    }
                }

            }
        }
        private void Sep_OnLoaded(object sender, RoutedEventArgs e)
        {
            DisableWPFTabletSupport();
            //line.IsHitTestVisible = false;
            //Window.Children.Add(line);
            //  slider.AddHandler(Slider.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged));
            //TransformGroup group = new TransformGroup();
            //group.Children.Add(new RotateTransform(90));
            //group.Children.Add(new TranslateTransform(0, 0));
            //tm.RenderTransform = group;
            //TransformCollection xx = (TransformCollection)((TransformGroup)(tm.GetValue(Control.RenderTransformProperty))).GetValue(TransformGroup.ChildrenProperty);
            //((TranslateTransform)xx[1]).X = -40;
            //((TranslateTransform)xx[1]).Y = -40;
            //AllEditiorDataFileStruct all = new AllEditiorDataFileStruct();
            //all.control_data = new Dictionary<uint, object>();
            //all.control_info = new List<ConBaseInfo>();
            //all.resource = new List<Asset.AssetFileInfo>();
            //SaveALoad.Save(all, @"E:\data\tptest\test.tcom");
            //   BindingOperations.ClearAllBindings(t1);
            //Binding posybinding = new Binding() { Source = button, Mode = BindingMode.OneWay, Path = new System.Windows.PropertyPath(Control.ActualHeightProperty) };
            //text.SetBinding(TextBox.TextProperty, posybinding);
            //NormalUButton com = new NormalUButton();
            //com.TheButton.OpacityMask = new ImageBrush();
            //com.TheButton.OpacityMask.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\test2.png")));
            //com.TheButton.Source = new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\照片.jpg"));
            //com.AddHandler(NormalUButton.MouseDownEvent, new MouseButtonEventHandler(imagetest_MouseDown));
            // hidder = new Com_Editor_Hidder(0);
            //hidder.SetValue(Canvas.ZIndexProperty, 1);
            //hidder.Source = new BitmapImage(new Uri(@"C:\Users\曾潇\Desktop\tra.png"));
            //Binding binding_height = new Binding();
            //binding_height.Source = hidder;
            //binding_height.Path = new PropertyPath(Com_Editor_Hidder.ActualHeightProperty);
            //com.SetBinding(Control.HeightProperty, binding_height);
            //Binding binding_wdith = new Binding();
            //binding_wdith.Source = hidder;
            //binding_wdith.Path = new PropertyPath(Com_Editor_Hidder.ActualWidthProperty);
            //com.SetBinding(Control.WidthProperty, binding_wdith);
            //Binding binding_left = new Binding();
            //binding_left.Source = hidder;
            //binding_left.Path = new PropertyPath(Canvas.LeftProperty);
            //com.SetBinding(Canvas.LeftProperty, binding_left);
            //Binding binding_top = new Binding();
            //binding_top.Source = hidder;
            //binding_top.Path = new PropertyPath(Canvas.TopProperty);
            //com.SetBinding(Canvas.TopProperty, binding_top);
            //Window.Children.Add(com);
            //Window.Children.Add(hidder);
            //hidder.Width = 100;
            //hidder.Height = 100;
            //Canvas.SetLeft(hidder, 100);
            //Canvas.SetTop(hidder, 100);
            //hidder.IsHitTestVisible = false;
            //TestTransImage image = new TestTransImage();
            //image.Height = 200;
            //image.Width = 200;
            //image.Source = new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\照片.jpg", UriKind.Absolute));
            //image.OpacityMask = new ImageBrush();
            //image.OpacityMask.SetValue(ImageBrush.ImageSourceProperty, new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\test2.png", UriKind.Absolute)));
            //gg.Children.Add(image);
            //image.AddHandler(TestTransImage.MouseDownEvent, new MouseButtonEventHandler(delegate (object sender, MouseButtonEventArgs e)
            //{
            //    MessageBox.Show("ssx");
            //}));
            //Test_W.AddHandler(Button.MouseDownEvent, new RoutedEventHandler(Test_W_Click_D), true);
            //Test_W.AddHandler(Button.MouseUpEvent, new RoutedEventHandler(Test_W_Click_U), true);
            //ips = new InputSimulator();

            //int number = 50;
            //for (int i = 0; i < Window.ActualWidth; i += number)
            //{
            //    Line line = new Line();
            //    line.X1 = i;
            //    line.Y1 = 0;
            //    line.X2 = i;
            //    line.Y2 = Window.ActualHeight;
            //    line.Stroke = System.Windows.Media.Brushes.Black;
            //    line.StrokeThickness = 0.5;
            //    Window.Children.Add(line);
            //}
            //for (int i = 0; i < Window.ActualHeight; i += number)
            //{
            //    Line line = new Line();
            //    line.X1 = 0;
            //    line.Y1 = i;
            //    line.X2 = Window.ActualWidth;
            //    line.Y2 = i;
            //    line.Stroke = System.Windows.Media.Brushes.Black;
            //    line.StrokeThickness = 0.5;
            //    Window.Children.Add(line);
            //}
        }

        private void imagetest_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //thispoint = e.GetPosition(Window);
            //Canvas.SetLeft(test, thispoint.X);
            //Canvas.SetTop(test, thispoint.Y);
            //e.Handled = true;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern long GetLastError();
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public struct RECT

        {

            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

        }
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hwnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        WinEventDelegate dele,actdele = null;
        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            //GameRegister register = new GameRegister();
            //register.ShowDialog();
            //GameSetting setting = register.GetSetting();
            //InitProcess iprocess = new InitProcess(setting);
            //bool changed;
            //iprocess.Start(new WindowInteropHelper(this).Handle,out changed);
            //Close();

            //  MessageBox.Show(hidder.Width+"|"+hidder.Height);
            //OpenFileDialog o = new OpenFileDialog();
            //o.Filter = "tt|*.tlay";
            //o.ShowDialog();
            //Touch_LoadAndSave save = new Touch_LoadAndSave(o.FileName);
            //Touch_BitmapImageCache cache;
            //LayoutInfo info;
            //if (!save.Load(out cache, out info)) return;
            //KeysRegister register = new KeysRegister();
            //register.Init(info, cache);
            //register.ShowDialog();

            //Close();
            //string path = @"D:\pvz\Ruina_chsV1.21_setup\game\start.exe";
            //        Process pro = Process.Start(path);
            // test = new TouchTest();
            //test.Show();
            //xxc = new WindowInteropHelper(test).Handle;
            ////xpp = new WindowInteropHelper(this).Handle;
            //  new Thread(XXX).Start();
            //if (true)
            //{
            //    SetForegroundWindow(xxc);
            //    MessageBox.Show("3s");
            //}
            //Thread.Sleep(3000);
            //xpp = GetForegroundWindow();
            //MessageBox.Show("ok");
            //SetForegroundWindow(xxc);
            //////RECT r=default;
            //TouchPadAction action = new TouchPadAction(xpp,xxc,test,true);
            //RECT pos= default;
            //if (GetWindowRect(xpp, ref pos))
            //{
            //    if(GetClientRect(xpp,ref r)){
            //        MoveWindow(xxc, pos.Left+1,pos.Bottom-r.Bottom, r.Right - r.Left,((int)(1.03*(r.Bottom - r.Top))));
            //    } 
            //}
            //dele = new WinEventDelegate(WinEventProc);
            //int pid;
            //GetWindowThreadProcessId(xpp, out pid);
            // m_hhook = SetWinEventHook(0x000A, 0x000B, IntPtr.Zero, dele, (uint)pid,0, 0);
            //actdele = new WinEventDelegate(active);
            //act = SetWinEventHook(0x0003, 0x0003, IntPtr.Zero, actdele,0, 0, 0);
            //System.Diagnostics.Debug.WriteLine(GetLastError());
            //  test.SetKeyBoardMode();

        }
        
        void XXX()
        {
            RECT r = default;
            while (true)
            {
                bool full = false;
                GetWindowRect(GetForegroundWindow(),ref r);
                if (r.Left != 0|| r.Top != 0){
                    full = false;
                }
                else{
                    int w = GetSystemMetrics(0);
                    int h = GetSystemMetrics(1);
                    full = ((r.Right - r.Left) == w && (r.Bottom - r.Top) == h);
                }
               
                // bool full = new Rect(r.Left, r.Top, (r.Right - r.Left), (r.Bottom - r.Top)).Contains();
                System.Diagnostics.Debug.WriteLineIf(full,full);

                //System.Diagnostics.Debug.WriteLine(w+ "!" + h);
                //System.Diagnostics.Debug.WriteLine((r.Right-r.Left)+"|"+(r.Bottom- r.Top));
                
                //  MoveWindow(xxc,0,0,r.Right-r.Left,r.Bottom-r.Top);
                Thread.Sleep(1000);
            }
        }
        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);
        bool IsFullScreen(IntPtr window)
        {
            RECT r=default;
            bool full = false;
            GetWindowRect(GetForegroundWindow(), ref r);
            if (r.Left != 0 || r.Top != 0)
            {
                full = false;
            }
            else
            {
                int w = GetSystemMetrics(0);
                int h = GetSystemMetrics(1);
                full = ((r.Right - r.Left) == w && (r.Bottom - r.Top) == h);
            }
            return full;
        }
        IntPtr m_hhook,act,xxc,xpp;
        TouchTest test;
        int i=0;
        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hWinEventHook == m_hhook)
            {
                System.Diagnostics.Debug.WriteLine(eventType);
                RECT r = default;
                RECT pos = default;
                if (GetWindowRect(xpp, ref pos))
                {
                   // MoveWindow(xxc, pos.Left, pos.Top ,pos.Right - pos.Left, ((int)(1 * (pos.Bottom - pos.Top))));

                    if (GetClientRect(xpp, ref r))
                    {
                        MoveWindow(xxc, pos.Left, pos.Bottom - r.Bottom, r.Right - r.Left, (r.Bottom - r.Top));
                    }
                }
            }
        }
        public void active(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if(hWinEventHook == act)
            {
                IntPtr now = GetForegroundWindow();
               if (now != xpp && now != xxc) {
                    test.Hide();
                }
                else
                {
                    test.Show();
                    SetForegroundWindow(xpp);
                }
            }
        }
        bool upup = false;
        private Point thispoint;
        Point Point;
        Rectangle rect;
      //  BitmapImage image = new BitmapImage(new Uri(@"C:\Users\曾潇\Desktop\sss.jpg"));
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            upup = true;
            point = e.GetPosition(Window);
            point2 = point;

            //  System.Diagnostics.Debug.WriteLine(e.GetPosition(test));
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            upup = false;
        //    double minx=double.MaxValue, miny=double.MaxValue, maxx=double.MinValue, maxy = double.MinValue;
        //    Rect rectangle = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);
        //    Rect targetrect = new Rect();
        //    selector.Clear();
        //    foreach (UIElement i in Window.Children)
        //    {
        //        if (i.GetType() == typeof(Rectangle) || i.GetType() == typeof(Con_Editor_Selector)) continue;
        //        targetrect.X = Canvas.GetLeft(i);
        //        targetrect.Y = Canvas.GetTop(i);
        //        targetrect.Width = (double)i.GetValue(Image.WidthProperty);
        //        targetrect.Height = (double)i.GetValue(Image.HeightProperty);
        //        if (rectangle.IntersectsWith(targetrect))
        //        {
        //            if (targetrect.X < minx) minx = targetrect.X;
        //            if (targetrect.Y < miny) miny = targetrect.Y;
        //            if (targetrect.X + targetrect.Width > maxx) maxx = targetrect.X + targetrect.Width;
        //            if (targetrect.Y + targetrect.Height > maxy) maxy = targetrect.Y + targetrect.Height;
        //            selector.Add(i);
        //        }
        //    };
        //    if (selector.IsEmpty()) return;
        //    Canvas.SetLeft(selector,minx);
        //    Canvas.SetTop(selector,miny);
        //    selector.Width = maxx - minx;
        //    selector.Height = maxy - miny; ;
        }

        //private void Label_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (upup == false) return;
        //    point2 = e.GetPosition(Window);
        //    if((point2.X-point.X)*xe<=0|| (point2.Y - point.Y) * ye <= 0)
        //    {
        //        if((point2.X - point.X)>0&& (point2.Y - point.Y) > 0)
        //        {
        //            x = 0;y = 0;
        //        }
        //        else if ((point2.X - point.X) < 0 && (point2.Y - point.Y) > 0)
        //        {
        //            x = -1;y = 0;
        //        }
        //        else if ((point2.X - point.X) > 0 && (point2.Y - point.Y) < 0)
        //        {
        //            x = 0;y= -1;
        //        }
        //        else 
        //        {
        //            x = -1; y = -1;
        //        }
        //    }
        //    xe = (point2.X - point.X);ye = (point2.Y - point.Y);
        //    rect.Width= Math.Abs(point2.X - point.X);
        //    rect.Height = Math.Abs(point2.Y - point.Y);
        //    Canvas.SetLeft(rect, point.X + rect.Width * x);
        //    Canvas.SetTop(rect, point.Y + rect.Height * y);
        //    //if (!upup) return;
        //    //thispoint = e.GetPosition(Window);
        //    //Canvas.SetLeft(test, thispoint.X);
        //    //Canvas.SetTop(test, thispoint.Y);
        //    e.Handled = true;
        //}
        Point point,point2;

        private void NormalButton_TouchDown(object sender, TouchEventArgs e)
        {
           
        }

        int x =0, y = 0;

        private void slider_Copy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //selector.FlashRotate(e.NewValue-e.OldValue);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Matrix rotate = Matrix.Identity;
            //rotate.Rotate(e.NewValue-e.OldValue);
            //Vector vector = new Vector(line.X2-line.X1,line.Y2-line.Y1);
            //vector=rotate.Transform(vector);
            //line.X2 = vector.X + line.X1;
            //line.Y2 = vector.Y + line.X1;

            //selector.FlashScale(e.NewValue,e.OldValue);
        }

        double xe,ye = 0;
        //private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    point = new Point((double)test.GetValue(Canvas.LeftProperty), (double)test.GetValue(Canvas.TopProperty));
        //    //Rect xp = xx.TransformBounds(new Rect(0, 0, 1, 1));
        //    //point.X += xp.X;
        //    //point.Y += xp.Y;
        //    xxx += x * (e.OldValue - e.NewValue);
        //    test.SetValue(Canvas.LeftProperty,point.X+ x * (e.OldValue - e.NewValue));
        //    test.SetValue(Canvas.TopProperty, point.Y+ y * (e.OldValue - e.NewValue));
        //    System.Diagnostics.Debug.WriteLine(xxx);
        //}

        //private void slider_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    GeneralTransform xxx = Window.TransformToVisual(test);
        //    point = xxx.Transform(new Point() { X = Window.ActualWidth / 2, Y = Window.ActualHeight / 2 });
        //}

        //private void slider_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    point = new Point((double)test.GetValue(Canvas.LeftProperty), (double)test.GetValue(Canvas.TopProperty));
        //    Rect xp = xx.TransformBounds(new Rect(0, 0, 1, 1));
        //    point.X += xp.X;
        //    point.Y += xp.Y;
        //    x = -point.X + Window.ActualWidth / 2;
        //     y = -point.Y + Window.ActualHeight / 2;
        //    line.X1 = point.X;
        //    line.Y1 = point.Y;
        //    line.X2 = 0;
        //    line.Y2 = 0;
           
        //}
    }
}
