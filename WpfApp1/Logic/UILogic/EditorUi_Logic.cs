using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Attributes;
using WpfApp1.User_Control;

namespace WpfApp1.Logic.UILogic
{
    internal struct Com_Editor_Setting
    {
        public  int griddestiny;
        public uint count;
        public bool enableattach;
        public uint[] size;
        public string background;
        public double[] layoutpos;
        public double[] layoutscale;
        public bool enablebackground;
    }
    internal enum EditorState
    {
        Normal, Importing,Selecting
    }
    internal enum EdiorSetting
    {
        GidDestiny, Count,EnableAttach,Size,BackGround,ALL,LayoutPos,Layoutscale,LayoutSize,EnableBackground
    }
    internal class EditorUi_Logic:EditorUIEvent_Listerner,FileOperation
    {
        Con_Editor_Hidder actived=null;
        Com_Editor_Setting _Setting;
        Canvas layout;
        Canvas border;
        EditorState state;
        ImageBrush selectedbg;
        Dictionary<uint, Con_Editor_Hidder> hidders;
        List<Line>[] lines;
        Rectangle rectangle;
        bool boxselection = false;
        Con_Editor_Selector selector;
        bool isdrag = false;
        int dragspeed = 1;
        double x, y, xe, ye;//经常用到的double，直接全局
        int count = 0;
        List<EditorUIEvent_Listern> list;
        /// <summary>
        /// true，必须包含整个控件，false，相交即可
        /// </summary>
        bool selectionmode = false;
        public  EditorUi_Logic()
        {
            _Setting = new Com_Editor_Setting();
            _Setting.griddestiny = 50;
            _Setting.count = 0;
            _Setting.size = new uint[] {16,9};
            _Setting.background = "";
            _Setting.layoutpos = new double[] { 0, 0 };
            _Setting.layoutscale = new double[] { 1, 1 };
            _Setting.enableattach = false;
            state = EditorState.Normal;
            list = new List<EditorUIEvent_Listern>();
            selectedbg = new ImageBrush();
            selectedbg.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resource/Selected.png"));
            hidders = new Dictionary<uint, Con_Editor_Hidder>();
            lines = new List<Line>[2] { new List<Line>(), new List<Line>() };
            rectangle = new Rectangle();
            rectangle.Stroke = System.Windows.Media.Brushes.Black;
            selector = new Con_Editor_Selector();
            
            selector.Visibility = Visibility.Collapsed;
        }
        public  void StartBoxSelect(bool contain)
        {
            
            selectionmode = contain;
            CloseSelector();
            TriggerAllListern(EditorUiEvent.CloseWindow, null);
            SetActive(null);
            boxselection = true;
        }
        public void FixUnKnowSetting()
        {
            if(_Setting.size==null)
            _Setting.size = new uint[] { 16, 9 };
            if(_Setting.background==null)
                _Setting.background = "";
            if (_Setting.griddestiny == 0)
                _Setting.griddestiny = 50;
           if(_Setting.layoutpos==null)
                _Setting.layoutpos = new double[] { 0, 0 };
            if (_Setting.layoutscale == null)
                _Setting.layoutscale = new double[] { 1, 1 };
        }
        public ConBaseData GetActiveInfo()
        {
            if(actived!=null)
            return actived.condata;
            return new ConBaseData();
        }
        public void SetActivedName(string name)
        {
            actived.SetConName(name);
        }
        public void SetSetting(EdiorSetting identifier,object par)
        {
            switch (identifier)
            {
                case EdiorSetting.GidDestiny:
                    _Setting.griddestiny = (int)par;
                    DrawGridLine();
                    break;
                case EdiorSetting.EnableAttach:
                    _Setting.enableattach = (bool)par;
                    break;
                case EdiorSetting.Size:
                    uint[] newvalue= (uint[])par;
                    if (_Setting.size[0] != newvalue[0] || _Setting.size[1] != newvalue[1])
                    {
                        layout.Width = newvalue[0];
                        layout.Height = newvalue[1];
                    }
                    _Setting.size = (uint[])par;
                    DrawGridLine();
                    break;
                case EdiorSetting.BackGround:
                    if(!String.IsNullOrEmpty(_Setting.background))
                    Editor_GobalVar.GetIns().imagecache.FreeImage(_Setting.background);
                    _Setting.background = (string)par;
                   ((ImageBrush)(layout.Background)).ImageSource=Editor_GobalVar.GetIns().imagecache.GetBitmap(_Setting.background);
                    break;
                case EdiorSetting.ALL:
                    Com_Editor_Setting setting = (Com_Editor_Setting)par;
                    _Setting.griddestiny = setting.griddestiny;
                    _Setting.enableattach = setting.enableattach;
                    SetSetting(EdiorSetting.Size,setting.size);
                    SetSetting(EdiorSetting.BackGround, setting.background);
                    break;
                case EdiorSetting.LayoutPos:
                    _Setting.layoutpos = (double[])par;
                    break;
                case EdiorSetting.Layoutscale:
                    _Setting.layoutscale = (double[])par;
                    break;
                case EdiorSetting.EnableBackground:
                    _Setting.enablebackground = (bool)par;
                    break;
            }
        }
        public object GetSetting(EdiorSetting identifier)
        {
            switch (identifier)
            {
                case EdiorSetting.GidDestiny:
                    return _Setting.griddestiny;
                case EdiorSetting.Count:
                    return _Setting.count;
                case EdiorSetting.EnableAttach:
                    return _Setting.enableattach;
                case EdiorSetting.Size:
                    return _Setting.size;
                case EdiorSetting.EnableBackground:
                    return _Setting.enablebackground;
                case EdiorSetting.ALL:
                    Com_Editor_Setting setting = _Setting;
                    setting.count = 0;
                    return setting;
            }
            return null;
        }
        public void SetCanvas(Canvas obj, Canvas border)
        {
            layout = obj;
            this.border = border;
            layout.AddHandler(Canvas.MouseDownEvent, new MouseButtonEventHandler(Com_Editor_Ui_MouseDown),false);
            layout.AddHandler(Canvas.MouseUpEvent, new MouseButtonEventHandler(Com_Editor_Ui_MouseUp));
            layout.AddHandler(Canvas.MouseMoveEvent, new MouseEventHandler(Com_Editor_Ui_MouseMove));
            layout.Children.Add(selector);
            layout.Children.Add(rectangle);
            Canvas.SetZIndex(selector, 0);
        }
        public void AddControl(Control con,ConBaseInfo info)
        {
            Add(con,info);
            state = EditorState.Importing;
        }
        public bool DeleteActived()
        {
            if (actived == null) return false;
            TriggerAllListern(EditorUiEvent.Delete, actived.condata);
            hidders.Remove(actived.condata.conBaseInfo.con_id);
            layout.Children.Remove(actived.condata.con);
            layout.Children.Remove(actived);
            SetActive(null);
            return true;
        }
        public List<ConBaseInfo> GetConInfo()
        {
            List<ConBaseInfo> info=new List<ConBaseInfo>();
            foreach (Con_Editor_Hidder hidder in hidders.Values )
            {
                info.Add(hidder.condata.conBaseInfo);
            }
            return info;
        }
        public void LockCon(uint conid)
        {
            SetActive(hidders[conid]);
            double fx =-(double)(actived.GetValue(Canvas.LeftProperty))  + border.ActualWidth / 2;
            double fy = -(double)(actived.GetValue(Canvas.TopProperty))+  border.ActualHeight / 2;
            layout.SetValue(Canvas.LeftProperty,fx);
            layout.SetValue(Canvas.TopProperty, fy);
        }
        private bool Add(Control con,ConBaseInfo info)
        {
            try
            {
                ConBaseData conBase = new ConBaseData(_Setting.count, info, con);
                Con_Editor_Hidder hidder = new Con_Editor_Hidder();
              
                hidder.SetBaseData(conBase);
             //   hidder.mycontrol = con;
               // hidder.Source = new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\Hidder_Black.png"));
                double tempsize = 0;
                tempsize = (border.ActualHeight > border.ActualWidth ? border.ActualWidth : border.ActualHeight)/5;
                hidder.Height = tempsize;
                hidder.Width = tempsize;
                //   con.Height = border.ActualHeight / 5;
                // con.Width = border.ActualWidth / 5;
                Binding binding_height = new Binding();
                binding_height.Source = con;
                binding_height.Path = new PropertyPath(Con_Editor_Hidder.HeightProperty);
                binding_height.Mode = BindingMode.TwoWay;
                hidder.SetBinding(Control.HeightProperty, binding_height);
                Binding binding_wdith = new Binding();
                binding_wdith.Source = con;
                binding_wdith.Path = new PropertyPath(Con_Editor_Hidder.WidthProperty);
                binding_wdith.Mode = BindingMode.TwoWay;
                hidder.SetBinding(Control.WidthProperty, binding_wdith);
                Binding binding_left = new Binding();
                binding_left.Source = con;
                binding_left.Path = new PropertyPath(Canvas.LeftProperty);
                binding_left.Mode = BindingMode.TwoWay;
                hidder.SetBinding(Canvas.LeftProperty, binding_left);
                Binding binding_top = new Binding();
                binding_top.Source = con;
                binding_top.Path = new PropertyPath(Canvas.TopProperty);
                binding_top.Mode = BindingMode.TwoWay;
                hidder.SetBinding(Canvas.TopProperty, binding_top);
                TransformGroup group = new TransformGroup();
                group.Children.Add(new TranslateTransform());
                group.Children.Add(new RotateTransform());
                hidder.RenderTransform = group;
                con.RenderTransform = group;
                //Binding binding_rot = new Binding();
                //binding_rot.Source = con;
                //binding_rot.Path = new PropertyPath(Control.RenderTransformProperty);
                //binding_rot.Mode = BindingMode.TwoWay;
                //hidder.SetBinding(Control.RenderTransformProperty, binding_rot);
                con.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                con.Arrange(new Rect(0, 0, con.DesiredSize.Width, con.DesiredSize.Height));
                layout.Children.Add(hidder);
                layout.Children.Add(con);
                hidder.Width = 100;
                hidder.Height = 100;
                SetActive(hidder);
                hidder.SetValue(Canvas.ZIndexProperty, 0);
                con.SetValue(Canvas.ZIndexProperty,-1);
                TriggerAllListern(EditorUiEvent.ADD, conBase);
                hidders.Add(_Setting.count, hidder);
                _Setting.count++;
                return true;
            }
            catch 
            {
                return false;
            }
        
        }
        private bool Recover(Control con, ConBaseInfo info)
        {
            try
            {
                ConBaseData conBase = new ConBaseData(info.con_id, info, con);
                Con_Editor_Hidder hidder = new Con_Editor_Hidder();
                hidder.SetBaseData(conBase);
                //   hidder.mycontrol = con;
                // hidder.Source = new BitmapImage(new Uri(@"E:\data\c#\TouchCoreTest\WpfApp1\WpfApp1\Hidder_Black.png"));
                double tempsize = 0;
                tempsize = (border.ActualHeight > border.ActualWidth ? border.ActualWidth : border.ActualHeight) / 5;
                hidder.Height = tempsize;
                hidder.Width = tempsize;
                //   con.Height = border.ActualHeight / 5;
                // con.Width = border.ActualWidth / 5;
                Binding binding_height = new Binding();
                binding_height.Source = con;
                binding_height.Path = new PropertyPath(Con_Editor_Hidder.HeightProperty);
                binding_height.Mode = BindingMode.TwoWay;
                binding_height.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                hidder.SetBinding(Control.HeightProperty, binding_height);
                Binding binding_wdith = new Binding();
                binding_wdith.Source = con;
                binding_wdith.Path = new PropertyPath(Con_Editor_Hidder.WidthProperty);
                binding_wdith.Mode = BindingMode.TwoWay;
                binding_wdith.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                hidder.SetBinding(Control.WidthProperty, binding_wdith);
                Binding binding_left = new Binding();
                binding_left.Source = con;
                binding_left.Path = new PropertyPath(Canvas.LeftProperty);
                binding_left.Mode = BindingMode.TwoWay;
                binding_left.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                hidder.SetBinding(Canvas.LeftProperty, binding_left);
                Binding binding_top = new Binding();
                binding_top.Source = con;
                binding_top.Path = new PropertyPath(Canvas.TopProperty);
                binding_top.Mode = BindingMode.TwoWay;
                binding_top.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                hidder.SetBinding(Canvas.TopProperty, binding_top);
                TransformGroup group = new TransformGroup();
                group.Children.Add(new TranslateTransform());
                group.Children.Add(new RotateTransform());
                hidder.RenderTransform = group;
                con.RenderTransform = group;
                con.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                con.Arrange(new Rect(0, 0, con.DesiredSize.Width, con.DesiredSize.Height));
                layout.Children.Add(hidder);
                layout.Children.Add(con);
                SetActive(hidder);
                hidder.SetValue(Canvas.ZIndexProperty, 0);
                con.SetValue(Canvas.ZIndexProperty, -1);
                hidders.Add(info.con_id, hidder);
                TriggerAllListern(EditorUiEvent.ADD, conBase);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool SetActive(Con_Editor_Hidder myself)
        {
            if (actived != null)
            {
                if (myself != null)
                {
                    if (myself.condata.conBaseInfo.con_id == actived.condata.conBaseInfo.con_id) return false;//是自己还设置什么！
                }

                else
                {
                    TriggerAllListern(EditorUiEvent.CloseWindow, null);
                }
                       
            }
                
            if (state==EditorState.Normal) {
                if (actived != null)
                {
                    actived.SetValue(Canvas.ZIndexProperty, 0);
                    actived.Background = Brushes.Transparent;
                    if (actived.GetType() == typeof(Con_Editor_Selector))
                    {
                        actived.Visibility = Visibility.Collapsed;
                        selector.Visibility = Visibility.Collapsed;
                        selector.FlashCons(TriggerAllListern);
                        selector.Clear();
                    }
                }
                actived = myself;
                if (myself != null)
                {
                    actived.SetValue(Canvas.ZIndexProperty, 1);
                    actived.Background = selectedbg;
                    TriggerAllListern(EditorUiEvent.ACTIVE, myself.condata);
                }
                
                return true;
            }
            else if (state == EditorState.Selecting)
            {
                if (myself.GetType() != typeof(Con_Editor_Selector))
                {
                    selector.Add(myself);
                    myself.Background = selectedbg;
                }
                return true;
            }
            return false;
        }
        public void ActiveUI()
        {
            DrawGridLine();
            if (!String.IsNullOrEmpty(_Setting.background))
                ((ImageBrush)(layout.Background)).ImageSource= Editor_GobalVar.GetIns().imagecache.GetBitmap(_Setting.background);
            layout.SetValue(Canvas.LeftProperty, _Setting.layoutpos[0]);
            layout.SetValue(Canvas.TopProperty, _Setting.layoutpos[1]);
            TransformGroup x = layout.GetValue(Control.RenderTransformProperty) as TransformGroup;
            ScaleTransform sclae = x.Children[0] as ScaleTransform;
            sclae.ScaleX = _Setting.layoutscale[0];
            sclae.ScaleY = _Setting.layoutscale[1];
        }
        private void DrawGridLine()
        {
            layout.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            layout.Arrange(new Rect(0, 0, layout.DesiredSize.Width, layout.DesiredSize.Height));
            int xnumber = (int)(layout.ActualWidth / _Setting.griddestiny)+1;
            int ynumber = (int)(layout.ActualHeight / _Setting.griddestiny)+1;
            if (lines[0].Count > xnumber)
            {
                for (int i=0;i<lines[0].Count-xnumber ;i++)
                {
                    layout.Children.Remove(lines[0][i]);
                }
                lines[0].RemoveRange(0, lines[0].Count- xnumber);
               
            }
            if (lines[1].Count >  ynumber)
            {
                for (int i = 0; i < lines[1].Count-ynumber; i++)
                {
                    layout.Children.Remove(lines[1][i]);
                }
                lines[1].RemoveRange(0, lines[1].Count - ynumber);
            }
            for (int i = 0; i < xnumber; i++)
            {
                Line line;
                if (lines[0].Count - 1 < i)
                {
                    line = new Line();
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.StrokeThickness = 0.5;
                    line.IsHitTestVisible = false;
                    lines[0].Add(line);
                }
                else
                {
                    line = lines[0][i];
                }
               
                line.X1 = i*_Setting.griddestiny;
                line.Y1 = 0;
                line.X2 = i * _Setting.griddestiny;
                line.Y2 = layout.ActualHeight;
              
                if(line.Parent==null)
                    layout.Children.Add(line);
               
            }
            for (int i = 0; i < ynumber; i ++)
            {
                Line line;
                if (lines[1].Count - 1 < i)
                {
                    line = new Line();
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.StrokeThickness = 0.5;
                    line.IsHitTestVisible = false;
                    lines[1].Add(line);
                }
                else
                {
                    line = lines[1][i];
                }
                line.X1 = 0;
                line.Y1 = i * _Setting.griddestiny;
                line.X2 = layout.ActualWidth;
                line.Y2 = i * _Setting.griddestiny;
                if (line.Parent == null)
                    layout.Children.Add(line);

            }
        }
        public Point GetPosition(MouseEventArgs e)
        {
            return e.GetPosition(layout);
        }
        Point lastpoint, thispoint,selectionbox_startpoint, selectionbox_temppoint;
        private void CloseSelector()
        {
            if (actived != null)
            {
                if (actived.GetType() != typeof(Con_Editor_Selector)) return;
                selector.Visibility = Visibility.Collapsed;
                selector.FlashCons(TriggerAllListern);
                SetActive(null);
                selector.Clear();
            }
            
        }
        private void Com_Editor_Ui_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (state == EditorState.Normal)
            {
                isdrag = true;
                if (boxselection)
                {
                    x = 0;y = 0;xe = 0;ye = 0;
                    rectangle.Width = 0;
                    rectangle.Height = 0;
                    rectangle.Visibility = Visibility.Visible;
                    selectionbox_startpoint = e.GetPosition(layout);
                    selectionbox_temppoint = selectionbox_startpoint;
                }
                else
                {
                    CloseSelector();
                    lastpoint = e.GetPosition(border);
                }
               }
            else if(state == EditorState.Importing)
            {

                if (actived == null) return;
                Point point = e.GetPosition(layout);
                Canvas.SetLeft(actived, point.X);
                Canvas.SetTop(actived, point.Y);
                state= EditorState.Normal;
            }
            else if (state == EditorState.Selecting)
            {
                state = EditorState.Normal;
                selector.ClearSelectedBackGround();
                selector.SetSizeToFollowTransform();
                ShowSelector();
            }
        }
        private void Com_Editor_Ui_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isdrag) return;
            if (boxselection)
            {
                selectionbox_temppoint = e.GetPosition(layout);
                if ((selectionbox_temppoint.X - selectionbox_startpoint.X) * xe <= 0 || (selectionbox_temppoint.Y - selectionbox_startpoint.Y) * ye <= 0)
                {
                    if ((selectionbox_temppoint.X - selectionbox_startpoint.X) > 0 && (selectionbox_temppoint.Y - selectionbox_startpoint.Y) > 0)
                    {
                        x = 0; y = 0;
                    }
                    else if ((selectionbox_temppoint.X - selectionbox_startpoint.X) < 0 && (selectionbox_temppoint.Y - selectionbox_startpoint.Y) > 0)
                    {
                        x = -1; y = 0;
                    }
                    else if ((selectionbox_temppoint.X - selectionbox_startpoint.X) > 0 && (selectionbox_temppoint.Y - selectionbox_startpoint.Y) < 0)
                    {
                        x = 0; y = -1;
                    }
                    else
                    {
                        x = -1; y = -1;
                    }
                }
                xe = (selectionbox_temppoint.X - selectionbox_startpoint.X); 
                ye = (selectionbox_temppoint.Y - selectionbox_startpoint.Y);
                rectangle.Width = Math.Abs(selectionbox_temppoint.X - selectionbox_startpoint.X);
                rectangle.Height = Math.Abs(selectionbox_temppoint.Y - selectionbox_startpoint.Y);
                Canvas.SetLeft(rectangle, selectionbox_startpoint.X + rectangle.Width * x);
                Canvas.SetTop(rectangle, selectionbox_startpoint.Y + rectangle.Height * y);
            }
            else
            {
                thispoint = e.GetPosition(border);
                // if ((thispoint.X - lastpoint.X) < 1 && (thispoint.Y - lastpoint.Y) <1) return;
                //   if (thispoint.X == lastpoint.X && thispoint.Y == lastpoint.Y) MessageBox.Show("xx"); 
                x = (thispoint.X - lastpoint.X) * dragspeed;
                y = (thispoint.Y - lastpoint.Y) * dragspeed;
                //if (x >= 0) x = 0;
                //else if (x < -(layout.ActualWidth - border.ActualWidth)) x = -(layout.ActualWidth - border.ActualWidth);
                //if (y >= 0) y = 0;
                //else if (y <= -(layout.ActualHeight - border.ActualHeight)) y = -(layout.ActualHeight - border.ActualHeight);
                Canvas.SetLeft(layout, (double)layout.GetValue(Canvas.LeftProperty) + x);
                Canvas.SetTop(layout, (double)layout.GetValue(Canvas.TopProperty) + y);
                lastpoint = thispoint;
                //   System.Diagnostics.Debug.WriteLine(count+"|"+ (double)layout.GetValue(Canvas.LeftProperty) + "|"+ (double)layout.GetValue(Canvas.TopProperty));
                //   Debug.Content = -(Com_Editor_Ui.ActualWidth - Com_Editor_Ui_Borders.ActualWidth) + "|" + -(Com_Editor_Ui.ActualHeight - Com_Editor_Ui_Borders.ActualHeight);
            }
        }
        private void Com_Editor_Ui_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isdrag = false;
            if (boxselection)
            {
                boxselection = false;
                rectangle.Visibility = Visibility.Collapsed;
                Rect rectangle_rect = new Rect(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);
                Rect targetrect = new Rect();
                selector.Clear();
                foreach (uint id in hidders.Keys)
                {
                    targetrect.Width = (double)hidders[id].GetValue(Control.ActualWidthProperty);
                    targetrect.Height = (double)hidders[id].GetValue(Control.ActualHeightProperty);
                    targetrect.X = Canvas.GetLeft(hidders[id]) -targetrect.Width/2;
                    targetrect.Y = Canvas.GetTop(hidders[id]) -targetrect.Height/2;
                    if (!selectionmode)
                    {
                        if (rectangle_rect.IntersectsWith(targetrect))
                        {
                            selector.Add(hidders[id]);
                        }
                    }
                    else
                    {
                        if (rectangle_rect.Contains(targetrect))
                        {
                            selector.Add(hidders[id]);
                        }
                    }
                    
                };
                ShowSelector();
            }
        }
        public void StartSeleSelect()
        {
            if (state == EditorState.Normal)
            {
                
                selector.Clear();
                SetActive(null);
                state = EditorState.Selecting;
            } 
        }
        private void ShowSelector()
        {
            if (selector.IsEmpty()) { CloseSelector(); return; }
            selector.SetSizeToFollowTransform();
            //Canvas.SetLeft(selector, minx);
            //Canvas.SetTop(selector, miny);
            //selector.Width = maxx - minx;
            //selector.Height = maxy - miny;
            SetActive(selector);
            selector.Visibility = Visibility.Visible;
        }
        public void ShowSelector(uint[] index)
        {
            selector.Clear();
            for (int i = 0; i < index.Length; i++)
            {
                selector.Add(hidders[index[i]]);
            }
            ShowSelector();
        }

        public void ShowSelector(uint[] index,uint companion)
        {
            if (index == null) return;
            selector.Clear();
            for (int i = 0; i < index.Length; i++)
            {
                selector.Add(hidders[index[i]]);
            }
            selector.SetCompanion(hidders[companion]);
            ShowSelector();
        }

        public void AddListern(EditorUIEvent_Listern lis)
        {
            list.Add(lis);
        }

        public void RemoveListern(EditorUIEvent_Listern lis)
        {
            list.Remove(lis);
        }

        public void TriggerAllListern(EditorUiEvent eevent,object obj)
        {
            foreach (EditorUIEvent_Listern lis in list)
            {
                lis.AcceptEvent(eevent,obj);
            }
        }
        //同步当前的控件属性。
        public void FlashActived(uint which)
        {
            if(actived!=null)
            TriggerAllListern(EditorUiEvent.Flash,new object[] { actived.condata, which });
        }

        public void DoSave(BaseSaveFile basefile)
        {
           
            List<ConBaseInfo> con_info = new List<ConBaseInfo>();
            foreach (uint i in hidders.Keys)
            {
                con_info.Add(hidders[i].condata.conBaseInfo);
            }
            
            ((EditorSaveFile)basefile).control_info = con_info;
            ((EditorSaveFile)basefile).setting = _Setting;
        }
        public void DoLoad(BaseSaveFile basefile)
        {
          var loaddata=((EditorSaveFile)basefile).control_info;
            for ( int i=0;i<loaddata.Count;i++) {
                Recover(_Ins_Creator.CreateCon(loaddata[i].type), loaddata[i]);
                FlashActived(uint.MaxValue);
            }
            _Setting = ((EditorSaveFile)basefile).setting;
        }
   
        public void DoPush(BaseSaveFile basefile)
        {
            var loaddata = ((EditorSaveFile)basefile).control_info;
            selector.Clear();
            for (int i = 0; i < loaddata.Count; i++)
            {
                Recover(_Ins_Creator.CreateCon(loaddata[i].type), loaddata[i]);
                FlashActived(uint.MaxValue);
                selector.Add(hidders[loaddata[i].con_id]);
            }
            ShowSelector();
            state = EditorState.Importing;
        }
    }
}
