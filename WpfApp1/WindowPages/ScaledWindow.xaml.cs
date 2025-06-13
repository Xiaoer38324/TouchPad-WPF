using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;
using WpfApp1;
using WpfApp1.WindowPages;
using static System.Collections.Specialized.BitVector32;

namespace WpfApp1
{
    /// <summary>
    /// ScaledWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScaledWindow : Window
    {
        public ScaledWindow()
        {
            InitializeComponent();
        }
        Action enter_logic,leave_logic;//目标窗口
        Point click_point;//点击时的位置，偏移量
        POINT cursor_point_screen;
        RECT window_rect;
        IntPtr self;//自身窗口
        int clickpoint_x=0;
        int clickpoint_y=0;
        bool clicked = false;
        Continuation_Trigger c_trigger;
        WindowFunction function;
        bool kill;//关闭窗口时，是隐藏还是把窗口永久关闭
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.SystemParameters.IsTabletPC)
            {

                Scride_Grid.TouchMove += Grid_TouchMove ;
                Scride_Grid.TouchUp += Grid_TouchUp ;
                Scride_Grid.TouchDown += Grid_TouchDown;
                Scride_Grid.TouchLeave += Grid_TouchLeave ;
                Debug_Window.GetIns().OutPut("是平板啊");
            }
            else
            {

                Scride_Grid.MouseUp += Grid_MouseUp;
                Scride_Grid.MouseMove += Grid_MouseMove;
                Scride_Grid.MouseDown += Grid_MouseDown;
                Scride_Grid.MouseLeave += Grid_MouseLeave;
            }
            WindowChrome.SetWindowChrome(this, new WindowChrome() { GlassFrameThickness = new Thickness(-1) });
            Background = Brushes.Transparent;
            self =  new WindowInteropHelper(this).Handle;
            c_trigger = new Continuation_Trigger(Leave,2,500);
            WindowOperation.AddWindowStyle(0x80 | 0x08000000, self);
            function=new WindowFunction();
        }
        /// <summary>
        /// 设置逻辑数据
        /// </summary>
        /// <param name="enter_logic">进入时触发</param>
        /// <param name="leave_logic">离开时触发</param>
        /// <param name="kill">如果为true，则离开时关闭窗口，不可复用。反之隐藏，可以复用。若想多次使用请设置为false。默认为true</param>
        public void Setting(Action enter_logic,Action leave_logic,bool kill=true)
        {
            this.enter_logic = enter_logic; 
            this.leave_logic=leave_logic;
            this.kill = kill;
        }
        private void Enter()
        {
            function.KeepForeground(self);
            enter_logic?.Invoke();
        }
        
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            click_point = PointToScreen(e.GetPosition(this));
            WindowOperation.GetWindowRect(self, ref window_rect);
            clickpoint_x = (int)click_point.X - window_rect.Left;
            clickpoint_y = (int)click_point.Y - window_rect.Top;
            window_rect.Right = window_rect.Right - window_rect.Left;
            window_rect.Bottom = window_rect.Bottom - window_rect.Top;
            if (!c_trigger.Trigger())
            {
                clicked = true;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            clicked = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!clicked) return;
            WindowOperation.GetCursorPos(out cursor_point_screen);
            WindowOperation.MoveWindow(self,cursor_point_screen.X-clickpoint_x,cursor_point_screen.Y- clickpoint_y,window_rect.Right,window_rect.Bottom,true);
            
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            clicked = false;
        }

        public void Leave()
        {
            leave_logic?.Invoke();
            if (kill) Close();
            else
                Hide();
        }
        public void Start() {
            if (ShowActivated) Show();
            Enter();
        }
        private void Grid_TouchDown(object? sender, TouchEventArgs e)
        {

            
            click_point = PointToScreen(e.GetTouchPoint(this).Position);
            WindowOperation.GetWindowRect(self, ref window_rect);
            clickpoint_x = (int)click_point.X - window_rect.Left;
            clickpoint_y = (int)click_point.Y - window_rect.Top;
            window_rect.Right = window_rect.Right - window_rect.Left;
            window_rect.Bottom = window_rect.Bottom - window_rect.Top;
            if (!c_trigger.Trigger())
            {
                clicked = true;
            }
        }

        private void Grid_TouchLeave(object? sender, TouchEventArgs e)
        {
            clicked = false;

            Debug_Window.GetIns().OutPut("leave润了");
        }
        
      
        private void Grid_TouchMove(object? sender, TouchEventArgs e)
        {
            if (!clicked) return;
            WindowOperation.GetCursorPos(out cursor_point_screen);
            WindowOperation.MoveWindow(self, cursor_point_screen.X - clickpoint_x, cursor_point_screen.Y - clickpoint_y, window_rect.Right, window_rect.Bottom, true);
        }

        private void Grid_TouchUp(object? sender, TouchEventArgs e)
        {
            clicked = false;
        }
        protected override void OnClosed(EventArgs e)
        {
            function.KeepForeground(IntPtr.Zero);
            base.OnClosed(e);
        }
    }
    /// <summary>
    /// 单线程连续触发器
    /// </summary>
    class Continuation_Trigger
    {
        Action finish_event=null;
        public int totaltimes=0;
        int times = 0;
        int interval_time = 500;
        DateTime time = DateTime.MinValue;
        public Continuation_Trigger(Action finish_event, int totaltimes=2,int interval_time=500)
        {
            this.finish_event = (finish_event!=null? finish_event:throw new ArgumentNullException("Fininsh Event can't be null"));
            if(totaltimes>1)
            this.totaltimes = totaltimes;
            this.interval_time = interval_time;
        }
        public bool Trigger()
        {
            DateTime now = DateTime.Now;
            System.Diagnostics.Debug.WriteLine((now - time).TotalMilliseconds);
            if ((now - time).TotalMilliseconds <= interval_time )
            {
                times++;
                
            }
            else
            {
                times = 1;
            }
            time = now;
            if (times >= totaltimes)
            {
                finish_event();
                times = 0;
                time = DateTime.MinValue;
                return true;
            }
            return false;
        }
    }
}
