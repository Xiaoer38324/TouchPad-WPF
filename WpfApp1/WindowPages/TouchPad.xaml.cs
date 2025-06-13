using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using WindowsInput;
using WpfApp1;
using WpfApp1.Attributes;

namespace WpfApp1.Touch
{
    internal struct PadParameter
    {
        public bool cover;
        public bool attribute;
        public bool transparency;
        public bool allowtrans;
        public bool fullscreen;

        public PadParameter(bool cover, bool attribute, bool transparency, bool allowtrans, bool fullscreen)
        {
            this.cover = cover;
            this.attribute = attribute;
            this.transparency = transparency;
            this.allowtrans = allowtrans;
            this.fullscreen = fullscreen;
        }
    }
    /// <summary>
    /// TouchPad.xaml 的交互逻辑
    /// </summary>
    public partial class TouchPad : WindowHideAndShow
    {
        
        private bool Add(Control con,object obj)
        {
            TouchPadPlane.Children.Add(con);
            return true;
        }
        public TouchPad()
        {
            InitializeComponent();
        }
        internal void Init(LayoutInfo info, Touch_BitmapImageCache cache, IntPtr targetwindows, Dictionary<string, VirtualKeyCode[]> keys,PadParameter parameter)
        {
            this.par = parameter;
            this.target = targetwindows;
            if (par.cover)
            {
                ShowInTaskbar = true;
                WindowStyle = WindowStyle.None; 
                ResizeMode = ResizeMode.NoResize;
            }
            if (par.transparency) {
                WindowChrome.SetWindowChrome(this, new WindowChrome() { GlassFrameThickness = new Thickness(-1) }); ;
                Background = Brushes.Transparent;
                if (par.allowtrans) { AllowsTransparency = true; }
            }
             
            Editor_GobalVar.GetIns().imagecache = cache;
            normal = new Touch_Normalization(Add, info);
            normal.SetKeyCode(keys);
        }
        internal void ReSetTargetWindow(IntPtr target,bool fullscreen)
        {
            if (target != IntPtr.Zero)
            {
                this.target = target;
                action.SetGameWindow(target,fullscreen);
            }
        }
        Touch_Normalization normal;
        PadParameter par;
        IntPtr handle=IntPtr.Zero,target;
        TouchPadAction action;
        ScaledWindow scaled;
        bool hide_control= false;//是否设置一直隐藏，如果为一直隐藏，则自动隐藏/显示功能将会关闭
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;
            if (par.cover)
            {
                 action = new TouchPadAction(target,handle,this,par.fullscreen);
                 action.sizechange_end = Window_SizeChanged;
            }
            Thread.Sleep(1000);
         
            SetKeyBoardMode();
            normal.SetRealSize(TouchPadPlane.ActualWidth, TouchPadPlane.ActualHeight);
            normal.Operation(par.attribute,true);
            if (par.transparency)
            {
                scaled = new ScaledWindow();
                scaled.Setting(DoHide, delegate ()
                {
                    hide_control = false;
                    this.Show();
                }, false);
                Continuation_Trigger _trigger = new Continuation_Trigger(delegate ()
                {
                    scaled.Start();
                }, 5);
                TouchPadPlane.MouseDown += delegate (object obj, MouseButtonEventArgs args)
                {
                    if (_trigger.Trigger()) hide_control = true;
                };
            }
        }

        private void Window_SizeChanged()
        {
            if (normal != null)
                normal.ReSize(TouchPadPlane, ActualWidth, ActualHeight);
        }

        void SetKeyBoardMode()
        {
             WindowOperation.SetWindow(new WindowOperation_Par() { noactive = true, notitle = true, settop = true },handle);
        }

        public void DoShow()
        {
            if (hide_control == false)
            {
                this.Show();
            }
        }

        public void DoHide()
        {
            if (!hide_control) this.Hide();
        }
        public void DoClose()
        {
            if(action!=null)
            action.Close();
            if(scaled!= null)
            scaled.Close();
            this.Close();
        }
    }
}
