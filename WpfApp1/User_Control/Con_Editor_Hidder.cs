using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Logic.UILogic;

namespace WpfApp1.User_Control
{
    /// <summary>
    /// 这个控件只是为了防止按倒控件触发一系列时间所使用的，就是挡住控件，进行控件摆布的遮挡控件
    /// </summary>
    internal class Con_Editor_Hidder:Label
    {
        protected ConBaseData cdata;
      //  private TranslateTransform trans;
       // public Control mycontrol;//真实的控件
        public ConBaseData condata { private set { } get { return cdata; } }
        public Con_Editor_Hidder(){
            this.AddHandler(MouseDownEvent, new MouseButtonEventHandler(Mouse_Down));
            this.AddHandler(MouseUpEvent, new MouseButtonEventHandler(Mouse_Up));
            this.AddHandler(MouseMoveEvent, new MouseEventHandler(Mouse_Move));
            AddHandler(MouseLeaveEvent, new MouseEventHandler(Lost_Focus));
            Content = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Background = Brushes.Black;
            //trans = new TranslateTransform();
           // ((TransformGroup)(RenderTransform)).Children.Add(trans);
        }
        public bool SetConName(string name)
        {
            if (cdata.con == null) return false;
            cdata.conBaseInfo.name = name;
            return true;
        }
        public bool SetBaseData(ConBaseData data)
        {
            if (cdata.con == null) { cdata = data; data.con.RenderTransform = RenderTransform; ; return true; }
            return false;
        }
        private void Lost_Focus(object sender, MouseEventArgs e)
        {
            isdrag = false;
            e.Handled = true;
        }
        protected bool isdrag = false;
       protected Point thispoint;
        protected double attachdismax, attachdismin;
        protected int griddestiny;
        protected virtual void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            griddestiny= (int)Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.GidDestiny);
            attachdismin = griddestiny / 10;
            attachdismax = griddestiny - attachdismin;
            enableattach= (bool)Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.EnableAttach);
            isdrag = true;
            Editor_GobalVar.GetIns().logic.SetActive(this);
            e.Handled = true;
        }
        double x, y;
        int count = 0;
        protected bool enableattach;
        protected virtual void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (!isdrag) return;
            thispoint = Editor_GobalVar.GetIns().logic.GetPosition(e);
            x = thispoint.X % griddestiny;
            y = thispoint.Y % griddestiny;
            //System.Diagnostics.Debug.WriteLine(x+"|"+y);
            if (enableattach&&((x<attachdismin||x> attachdismax) && (y < attachdismin ||y > attachdismax)))
            {
                x = thispoint.X / griddestiny;
                y = thispoint.Y / griddestiny;
                Canvas.SetLeft(this, Math.Round(x)*griddestiny);
                Canvas.SetTop(this, Math.Round(y)* griddestiny);
            }
            else {
                Canvas.SetLeft(this, thispoint.X);
                Canvas.SetTop(this, thispoint.Y);
                //TranslateTransform t = ((TransformGroup)RenderTransform).Children[1] as TranslateTransform;
                //System.Diagnostics.Debug.WriteLine("+"+t.X);
            }
            e.Handled = true;

        }
        protected virtual void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            isdrag = false;
            //e.Handled = true;
        }

    }
}
