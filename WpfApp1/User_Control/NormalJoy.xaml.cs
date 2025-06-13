using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsInput;
using WpfApp1.Attributes;
using WpfApp1.Touch;
using WpfApp1.WindowPages;

namespace WpfApp1.User_Control
{
    /// <summary>
    /// TestJoy.xaml 的交互逻辑
    /// </summary>
    public partial class NormalJoy : UserControl,TouchActivator
    {
        /// <summary>
        /// 设置的时候为半径
        /// </summary>
        private int Grip_Range=1;
        private double degree=0;
        private bool[] direct;
        VirtualKeyCode[] keys;
        Action judge;
        public Action<NormalJoy> Move;
        public double Grip_Degree { 
        private set
            {
                
            }
            get
            {
                return (degree + 360) % 360;
            }
        }
        private double distance = 0;
        public double Grip_Distance { private set { } get { return Math.Sqrt(distance); } }
        bool isdown = false;
        public ImageSource GripSource
        {
            get
            {
                return Grip.Source;
            }
            set
            {
                Grip.Source = value;
            }
        }
        public ImageSource GripMaskSource
        {
            get
            {
                return(ImageSource) Grip.OpacityMask.GetValue(ImageBrush.ImageSourceProperty);
            }
            set
            {
                Grip.OpacityMask.SetValue(ImageBrush.ImageSourceProperty, value);
            }
        }
        public ImageSource BackGroundSource
        {
            get
            {
                return Range.Source;
            }
            set
            {
                Range.Source = value;
            }
        }
        private TouchPoint grip_point;
        private double DefLeft, DefTop;
        private Vector target,standup;
        public void Grip_MouseMove(object? sender, TouchEventArgs e)
        {
           
            if (!isdown) return;
            grip_point =e.GetTouchPoint(canvas);
            target.X = grip_point.Position.X - DefLeft;
            target.Y = grip_point.Position.Y - DefTop;
            distance = target.LengthSquared>Grip_Range*Grip_Range?Grip_Range*Grip_Range:target.LengthSquared;
            degree = -Vector.AngleBetween(target,standup);
            if (distance == Grip_Range * Grip_Range)
            {
                target.Normalize();
                target *= Grip_Range;
            }
            Canvas.SetLeft(Grip, DefLeft + target.X);
            Canvas.SetTop(Grip, DefTop + target.Y);
            judge();
        }
        public bool SetRange(int range)
        {
            if (range <= 0) return false;
            Grip_Range = range;
            return true;
        }
        public void Grip_MouseUp(object? sender, TouchEventArgs e)
        {
           
            isdown = false;
            Canvas.SetLeft(Grip, DefLeft);
            Canvas.SetTop(Grip, DefTop);
            for (int i = 0; i < 4; i++)
                if (direct[i])
                {
                    Touch_Core.KeyUp(keys[i]); direct[i] = false;
                }
            e.Handled = true;

        }
        public void Grip_MouseDown(object? sender, TouchEventArgs e)
        {
            isdown = true;
            if (!isdown) return;
            TouchPoint p;
            gpoint = e.GetTouchPoint(canvas).Position;
            target.X = gpoint.X - DefLeft;
            target.Y = gpoint.Y - DefTop;
            distance = target.LengthSquared > Grip_Range * Grip_Range ? Grip_Range * Grip_Range : target.LengthSquared;
            degree = -Vector.AngleBetween(target, standup);
            if (distance == Grip_Range * Grip_Range)
            {
                target.Normalize();
                target *= Grip_Range;
            }
            Canvas.SetLeft(Grip, DefLeft + target.X);
            Canvas.SetTop(Grip, DefTop + target.Y);
            judge();
            e.Handled = true;
        }
        public NormalJoy()
        {
            InitializeComponent();
            Grip.OpacityMask = new ImageBrush();
            judge = DoNothing;
            target = new Vector();
            standup = new Vector(0,-1);
         

        }
        private void Joy_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
        void DoNothing()
        {

        }
        void Judge()
        {
            if (Grip_Degree <  60 || Grip_Degree>300)
            {
                if (!direct[0])
                {
                    Touch_Core.KeyDown(keys[0]);
                    direct[0] = true;
                    System.Diagnostics.Debug.WriteLine("enter" + 0);
                }
            }
            else
            {
                if (direct[0])
                {
                    Touch_Core.KeyUp(keys[0]);
                    direct[0] = false;
                    System.Diagnostics.Debug.WriteLine("leave" + 0);
                }

            }
            for (int i = 1; i < 4; i++)
            {
                if (Grip_Degree < (i * 90) + 60 && Grip_Degree > (((i * 90) + 300) % 360))
                {
                    if (!direct[i])
                    {
                        Touch_Core.KeyDown(keys[i]);
                        direct[i] = true;
                        System.Diagnostics.Debug.WriteLine("enter" + i);
                    }

                }
                else
                {
                    if (direct[i])
                    {
                        Touch_Core.KeyUp(keys[i]);
                        direct[i] = false;

                        System.Diagnostics.Debug.WriteLine("leave" + i);
                    }

                }
            }
        }
        void TouchActivator.Active(VirtualKeyCode[] key)
        {

            direct = new bool[] { false,false,false,false};
            keys = key;
            judge = Judge;
            Window window = Window.GetWindow(this);
            if (System.Windows.SystemParameters.IsTabletPC)
            {
               
                window.TouchMove += Grip_MouseMove;
                window.TouchUp += Grip_MouseUp;
                Grip.TouchDown += Grip_MouseDown;
                Range.TouchDown += Grip_MouseDown;
            }
            else
            {
                
                window.MouseUp += Grip_MMouseUp;
                window.MouseMove += Grip_MMouseMove;
                Grip.MouseDown += Grip_MMouseDown;
                Range.MouseDown += Grip_MMouseDown;
            }
            
        }
        //测试事件
        private Point gpoint;
        public void Grip_MMouseMove(object sender, MouseEventArgs e)
        {
            if (!isdown) return;
            gpoint = e.GetPosition(canvas);
            target.X = gpoint.X - DefLeft;
            target.Y = gpoint.Y - DefTop;
            distance = target.LengthSquared > Grip_Range * Grip_Range ? Grip_Range * Grip_Range : target.LengthSquared;
            degree = -Vector.AngleBetween(target, standup);
            if (distance == Grip_Range * Grip_Range)
            {
                target.Normalize();
                target *= Grip_Range;
            }
            Canvas.SetLeft(Grip, DefLeft + target.X);
            Canvas.SetTop(Grip, DefTop + target.Y);
            judge();
            e.Handled = true;
        }
        bool hasininted = false;
        private void Joy_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DefLeft = (e.NewSize.Width / 2);
            DefTop = e.NewSize.Height / 2;
            Canvas.SetLeft(Grip, DefLeft);
            Canvas.SetTop(Grip, DefTop);
            Grip.Width = e.NewSize.Width / 5;
            Grip.Height = e.NewSize.Height / 5;
            if (!hasininted)
                Grip.RenderTransform = new TranslateTransform(-e.NewSize.Width / 10, -e.NewSize.Height / 10);
            else
            {
                ((TranslateTransform)Grip.RenderTransform).X = -e.NewSize.Width / 10;
                ((TranslateTransform)Grip.RenderTransform).Y = -e.NewSize.Height / 10;
            }
            if (!hasininted) hasininted = true;
            e.Handled = true;
        }

        public void Grip_MMouseUp(object sender, MouseButtonEventArgs e)
        {
            isdown = false;
            Canvas.SetLeft(Grip, DefLeft);
            Canvas.SetTop(Grip, DefTop);
            for (int i = 0; i < 4; i++)
                if (direct[i]) {
                    Touch_Core.KeyUp(keys[i]); direct[i] = false;
                }
            e.Handled = true;
        }
        public void Grip_MMouseDown(object sender, MouseButtonEventArgs e)
        {
            isdown = true;
            if (!isdown) return;
            gpoint = e.GetPosition(canvas);
            target.X = gpoint.X - DefLeft;
            target.Y = gpoint.Y - DefTop;
            distance = target.LengthSquared > Grip_Range * Grip_Range ? Grip_Range * Grip_Range : target.LengthSquared;
            degree = -Vector.AngleBetween(target, standup);
            if (distance == Grip_Range * Grip_Range)
            {
                target.Normalize();
                target *= Grip_Range;
            }
            Canvas.SetLeft(Grip, DefLeft + target.X);
            Canvas.SetTop(Grip, DefTop + target.Y);
            judge();
            e.Handled = true;
        }

        EControlType TouchActivator.GetECT()
        {
            return EControlType.Joy;
        }

      
    }
}
