using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1.User_Control
{
    internal class Con_Editor_Selector : Con_Editor_Hidder
    {
        List<Con_Editor_Hidder> collection;
        Con_Editor_Hidder companion;
        double minx = double.MaxValue, miny = double.MaxValue, maxx = double.MinValue, maxy = double.MinValue;
        double ix, iy, iw, ih;
        TranslateTransform trans;
        public Con_Editor_Selector()
        {

           // oldpoint = new Point();
            collection = new List<Con_Editor_Hidder>();
            cdata = new ConBaseData(uint.MaxValue,new ConBaseInfo("selectbox","box",Attributes.EControlType.Selector),this);
            trans = new TranslateTransform();
            RenderTransform = new TransformGroup();
            ((TransformGroup)RenderTransform).Children.Add(trans);
        }
        public bool IsEmpty()
        {
            return collection.Count == 0;
        }
        /// <summary>
        /// 这个控件将会跟随移动，但不会有其他影响
        /// </summary>
        /// <param name="con"></param>
        public void SetCompanion(Con_Editor_Hidder con)
        {
            companion = con;
        }
        public void Clear()
        {
            collection.Clear();
            companion = null;
        }
        public void Add(Con_Editor_Hidder ui)
        {
            collection.Add(ui);
        }
        public void FlashPos(double dx,double dy)
        {
            if (collection.Count == 0) return;
            for (int i=0;i<collection.Count;i++)
            {
                collection[i].SetValue(Canvas.LeftProperty, (double)collection[i].GetValue(Canvas.LeftProperty)+dx);
                collection[i].SetValue(Canvas.TopProperty, (double)collection[i].GetValue(Canvas.TopProperty) + dy);
            }
            if (companion != null)
            {
                companion.SetValue(Canvas.LeftProperty, (double)companion.GetValue(Canvas.LeftProperty) + dx);
                companion.SetValue(Canvas.TopProperty, (double)companion.GetValue(Canvas.TopProperty) + dy);
            }
        }
        public void FlashSelfRotate(double angle)
        {
            if (collection.Count == 0) return;
            for (int i = 0; i < collection.Count; i++)
            {
                ((RotateTransform)((TransformGroup)(collection[i].condata.con.RenderTransform)).Children[1]).Angle += angle;
            }
        }
        private void CalRange()
        {
            if (ix-iw/2 < minx) minx = ix-iw/2;
            if (iy - ih / 2 < miny) miny = iy - ih / 2;
            if (ix + iw/2 > maxx) maxx = ix + iw/2;
            if (iy + ih/2 > maxy) maxy = iy + ih/2;
        }
        private void RestRangePar()
        {
            minx = double.MaxValue; miny = double.MaxValue; maxx = double.MinValue; maxy = double.MinValue;
        }
        public void ClearSelectedBackGround()
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].Background = Brushes.Transparent;
            }
        }
        private void SetAutoSize()
        {
            Width = maxx - minx;
            Height = maxy - miny;
            trans.X = -Width / 2;
            trans.Y = -Height / 2;
        }
        public void SetSizeToFollowTransform()
        {
            RestRangePar();
            for (int i = 0; i < collection.Count; i++)
            {
                ix = Canvas.GetLeft(collection[i]);
                iy = Canvas.GetTop(collection[i]);
                iw = (double)collection[i].GetValue(Control.WidthProperty);
                ih = (double)collection[i].GetValue(Control.HeightProperty);
                CalRange();
            }
            Canvas.SetLeft(this, minx + (maxx - minx) / 2);
            Canvas.SetTop(this, miny + (maxy - miny) / 2);
            SetAutoSize();
        }
        public void FlashRotate(double angle)
        {
            if (collection.Count == 0) return;
            System.Diagnostics.Debug.WriteLine(angle);
            Matrix rotate = Matrix.Identity;
            rotate.Rotate(angle);
            Vector vector = new Vector();
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);
            RestRangePar();
            for (int i = 0; i < collection.Count; i++)
            {
                ix = Canvas.GetLeft(collection[i]);
                iy = Canvas.GetTop(collection[i]);
                iw = (double)collection[i].GetValue(Control.ActualWidthProperty);
                ih = (double)collection[i].GetValue(Control.ActualHeightProperty);
                vector.X =ix - x;
                vector.Y =iy- y;
                vector = rotate.Transform(vector);
                collection[i].SetValue(Canvas.LeftProperty, x + vector.X);
                collection[i].SetValue(Canvas.TopProperty,y + vector.Y);
                CalRange();
            }
            SetAutoSize();
        }
        public void FlashCons(Action<EditorUiEvent, object> callback)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                callback(EditorUiEvent.FlashConToData, collection[i].condata);
            }
        }
        public void FlashSelfScale(double newScaleIndex,double oldScaleIndex)
        {
            if (collection.Count == 0) return;
            double D=0;
            RestRangePar();
            for (int i = 0; i < collection.Count; i++)
            {
                ix = Canvas.GetLeft(collection[i]);
                iy = Canvas.GetTop(collection[i]);
                iw = (double)collection[i].GetValue(Control.ActualWidthProperty);
                ih = (double)collection[i].GetValue(Control.ActualHeightProperty);
              //  D = (newScaleIndex / oldScaleIndex) - 1;
                collection[i].SetValue(Control.WidthProperty,iw/oldScaleIndex * newScaleIndex);
                collection[i].SetValue(Control.HeightProperty, ih/oldScaleIndex * newScaleIndex);
                TranslateTransform translate = ((TransformGroup)collection[i].RenderTransform).Children[0] as TranslateTransform;
                translate.X = -iw / oldScaleIndex * newScaleIndex / 2;
                translate.Y = -ih / oldScaleIndex * newScaleIndex / 2;
                //Canvas.SetLeft(collection[i],ix-iw*D/2);
                //Canvas.SetTop(collection[i], iy-ih*D/2);
                CalRange();
            }

            SetAutoSize();
        }
        public void FlashScale(double newScaleIndex, double oldScaleIndex)
        {
            if (collection.Count == 0) return;
            RestRangePar();
            Vector vector = new Vector();
            double x = Canvas.GetLeft(this);
            double y = Canvas.GetTop(this);
            for (int i = 0; i < collection.Count; i++)
            {
                ix = Canvas.GetLeft(collection[i]);
                iy = Canvas.GetTop(collection[i]);
                iw = (double)collection[i].GetValue(Control.ActualWidthProperty);
                ih = (double)collection[i].GetValue(Control.ActualHeightProperty);
                vector.X = ix  - x;
                vector.Y = iy  - y;
                vector = vector / oldScaleIndex * newScaleIndex;
                Canvas.SetLeft(collection[i], x+vector.X);
                Canvas.SetTop(collection[i], y+vector.Y);
                CalRange();
            }
            SetAutoSize();
        }
         protected override  void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            base.Mouse_Down(sender,e);
            //oldpoint = Editor_GobalVar.GetIns().logic.GetPosition(e);
            //Point d = e.GetPosition(this);
          //  FlashPos(d.X,d.Y);
            e.Handled = true;
        }
       // Point oldpoint;
        protected override void Mouse_Move(object sender, MouseEventArgs e)
        {
            base.Mouse_Move(sender, e);
            if (!isdrag) return;
           // Point p = Editor_GobalVar.GetIns().logic.GetPosition(e);
           // FlashPos(p.X-oldpoint.X,p.Y-oldpoint.Y);
         //   oldpoint = p;
            e.Handled = true;

        }
        protected override void Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            base.Mouse_Up(sender,e);
           // e.Handled = true;
        }
    }
}
