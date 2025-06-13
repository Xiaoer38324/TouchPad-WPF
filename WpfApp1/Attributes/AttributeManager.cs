using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Attributes
{
    internal class Rotate : IValueConverter
    {
        TransformGroup group;
        RotateTransform rot=null;
        public void SetTransform(TransformGroup group)
        {
            this.group = group;
            rot = group.Children[1] as RotateTransform;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            rot.Angle = (double)value;
            return group;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return rot.Angle;
        }
    }
    internal class PosOffset : IValueConverter
    {
        TranslateTransform tra;
        public void SetTransform(TranslateTransform tra)
        {
            this.tra = tra;
        }
        public PosOffset() { }
        //con to slider
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double tvalue;
            if (Double.TryParse((string)value, out tvalue))
            {
                if ((bool)parameter) {
                    tra.X = -tvalue / 2; 
                }
                else
                {
                    tra.Y = -tvalue / 2;
                }
                return tvalue;
            }
            return 0;
        }
        //slider to con
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            return ((double)value).ToString();

        }

    }
    internal class PosConver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double tvalue;
            if(Double.TryParse((string)value,out tvalue))
            {
                return tvalue;
            }
            return 0;
        }
        //slider to con
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return ((double)value).ToString();

        }
    }
        public enum EControlType
    {
        None,Button,Joy,Group,Selector
    }
    internal class AttributeManager : EditorUIEvent_Listern,FileOperation
    {
        struct BindingPar
        {
            internal Binding posxbinding=null;
            internal Binding posybinding = null;
            internal Binding rotatebinding = null;
            internal Binding widthbinding = null;
            internal Binding heightbinding = null;
            internal PosOffset offset = null;
            internal Rotate rotate = null;

            public BindingPar(Com_Attributes com_Attributes)
            {
                object obj;
                obj = com_Attributes.GetCom(0);
                if (obj!=null)
                {
                    TextBox[] pos = (TextBox[])obj;//固定 0位置 1大小 2 旋转
                    if (pos != null)
                    {
                        PosConver poscov = new PosConver();
                        posxbinding = new Binding() { Source = pos[0], Mode = BindingMode.TwoWay, Path = new System.Windows.PropertyPath(TextBox.TextProperty), Converter = poscov };
                        posybinding = new Binding() { Source = pos[1], Mode = BindingMode.TwoWay, Path = new System.Windows.PropertyPath(TextBox.TextProperty), Converter = poscov };
                    }
                }
                obj = com_Attributes.GetCom(2);
                if (obj != null)
                {
                    Slider slider = ((Slider)((object[])obj)[0]);
                    if (slider != null)
                    {
                        rotate = new Rotate();
                        rotatebinding = new Binding() { Source = slider, Mode = BindingMode.TwoWay, Path = new System.Windows.PropertyPath(Slider.ValueProperty), Converter = rotate };
                    }
                }
                obj = com_Attributes.GetCom(1);
                if (obj != null)
                {
                    TextBox[] size = (TextBox[])obj;
                    if (size != null)
                    {
                        offset = new PosOffset();
                        widthbinding = new Binding() { Source = size[0], Mode = BindingMode.TwoWay, Path = new System.Windows.PropertyPath(TextBox.TextProperty), Converter = offset, ConverterParameter = true };
                        heightbinding = new Binding() { Source = size[1], Mode = BindingMode.TwoWay, Path = new System.Windows.PropertyPath(TextBox.TextProperty), Converter = offset, ConverterParameter = false };
                    }
                }

            }
        }
        EControlType nowtype;
        Dictionary<EControlType, Com_Attributes> attr_ui;
        Dictionary<uint, object> control_data;//对应的实际的ui数据，是最根本的储存数据，是不同的结构体
        ScrollViewer scroll;
        Dictionary<EControlType, BindingPar> binding;
        public AttributeManager(ScrollViewer scroll)
        {
            this.scroll = scroll;
            control_data = new Dictionary<uint, object>();
            attr_ui = new Dictionary<EControlType, Com_Attributes>();
            binding = new Dictionary<EControlType, BindingPar>();
        }
        //添加控件更新
        private AttributeOperInterFace GetAttributeViewData(EControlType type,uint con_id)
        {
            switch (type)
            {
                case EControlType.Button :
                    return Attr_Button.GetIns((Button_Attribute)control_data[con_id]);
                    break;
                case EControlType.Joy:
                    return Attr_Joy.GetIns((Joy_Attribute)control_data[con_id]);
                case EControlType.Group:
                    return Attr_Group.GetIns((Group_Attribute)control_data[con_id]);
                case EControlType.Selector:
                    return Attr_Selector.GetIns();
                    break;
            }
            throw new Exception("没有目标控件属性视图类型");
        }
        //添加控件更新
        private object CreateAttributeData(EControlType type)
        {
            switch (type)
            {
                case EControlType.Button:
                    return new Button_Attribute();
                    break;
                case EControlType.Joy:
                    return new Joy_Attribute();
                    break;
                case EControlType.Group:
                    return new Group_Attribute();
                    break;
            }
            throw new Exception("没有目标控件数据类型");
        }
        Control lastcon;
        uint lastconid;
        public void AcceptEvent(EditorUiEvent eevent, object obj)
        {
            ConBaseData data;
            switch (eevent)
            {
                case EditorUiEvent.ACTIVE:
                    try
                    {
                        data = (ConBaseData)obj;
                        bool isspecial = SpecialUIChecker.GetIns().CheckSpecial(data.con.GetType());
                        if (!isspecial)
                        if (!control_data.ContainsKey(data.conBaseInfo.con_id))
                        {
                            control_data.Add(data.conBaseInfo.con_id, CreateAttributeData(data.conBaseInfo.type));
                        }
                        if (data.conBaseInfo.type != nowtype)//与现在的不一样，要切换UI
                        {
                            if (attr_ui.Count > 0)
                            {
                                attr_ui[nowtype].GetView().Visibility = System.Windows.Visibility.Collapsed;//隐藏对应属性UI
                            }
                            if (!attr_ui.ContainsKey(data.conBaseInfo.type))//不一样，且没有包含这种UI界面数据
                            {
                                //不存在该UI界面，则生成属性UI界面。
                                attr_ui[data.conBaseInfo.type] = new Com_Attributes();
                                attr_ui[data.conBaseInfo.type].Init(GetAttributeViewData(data.conBaseInfo.type, data.conBaseInfo.con_id));
                                binding[data.conBaseInfo.type] = new BindingPar(attr_ui[data.conBaseInfo.type]);

                            }
                            nowtype = data.conBaseInfo.type;
                        }
                        bool[] enabletrs = SpecialUIChecker.GetIns().EnabelTSRBind(data.con.GetType());
                        Editor_GobalVar.GetIns().stopatrui = true;

                        //BindingOperations.ClearAllBindings(pos[0]);
                        //BindingOperations.ClearAllBindings(pos[1]);
                        //BindingOperations.ClearAllBindings(size[0]);
                        //BindingOperations.ClearAllBindings(size[1]);
                        //BindingOperations.ClearAllBindings(slider);
                        if (lastcon != null)
                        {
                            if (!SpecialUIChecker.GetIns().CheckSpecial(lastcon.GetType()))
                            {
                                AttributeDataS basedata = (AttributeDataS)control_data[lastconid];
                                basedata.pos[0] = (double)lastcon.GetValue(Canvas.LeftProperty);
                                basedata.pos[1] = (double)lastcon.GetValue(Canvas.TopProperty);
                                basedata.scale[0] = (double)lastcon.GetValue(Control.WidthProperty);
                                basedata.scale[1] = (double)lastcon.GetValue(Control.HeightProperty);
                                //解绑之后数据会消失，得即时保存
                                Transform rotate = lastcon.RenderTransform;
                                BindingOperations.ClearAllBindings(lastcon);
                                // BindingOperations.ClearBinding();
                                lastcon.RenderTransform = rotate;
                                lastcon.SetValue(Canvas.LeftProperty, basedata.pos[0]);
                                lastcon.SetValue(Canvas.TopProperty, basedata.pos[1]);
                                lastcon.SetValue(Control.WidthProperty, basedata.scale[0]);
                                lastcon.SetValue(Control.HeightProperty, basedata.scale[1]);
                            }
                            else
                            {
                                double x = (double)lastcon.GetValue(Canvas.LeftProperty);
                                double y = (double)lastcon.GetValue(Canvas.TopProperty);
                                double w = (double)lastcon.GetValue(Control.WidthProperty);
                                double h = (double)lastcon.GetValue(Control.HeightProperty);
                                Transform rotate = lastcon.RenderTransform;
                                BindingOperations.ClearAllBindings(lastcon);
                                lastcon.RenderTransform = rotate;
                                lastcon.SetValue(Canvas.LeftProperty, x);
                                lastcon.SetValue(Canvas.TopProperty, y);
                                lastcon.SetValue(Control.WidthProperty, w);
                                lastcon.SetValue(Control.HeightProperty, h);
                            }
                       
                        }
                        //初始化数据
                        //把相关属性进行捆绑
                        scroll.Content = attr_ui[nowtype].GetView();
                        TextBox[] size=null;
                        if (enabletrs[0])
                        {
                            if (isspecial) {
                                AttributeOperInterFace x = GetAttributeViewData(data.conBaseInfo.type, data.conBaseInfo.con_id);
                                var xp = (A_TwoTextEditorWithDouble_Par)x.GetDataById(0);
                                ((A_TwoTextEditorWithDouble_Par)x.GetDataById(0)).p1 = Canvas.GetLeft(data.con);
                                ((A_TwoTextEditorWithDouble_Par)x.GetDataById(0)).p2 = Canvas.GetTop(data.con);
                            }//特殊控件的位置一般是不以Ui为准，而是UI以实际为准，而普通控件是以Ui数据为准，方便更新。
                            data.con.SetBinding(Canvas.LeftProperty, binding[data.conBaseInfo.type].posxbinding);
                            data.con.SetBinding(Canvas.TopProperty, binding[data.conBaseInfo.type].posybinding);
                        }
                        TransformGroup group=null;
                        if (enabletrs[2])
                        {
                             group= (TransformGroup)data.con.RenderTransform;
                            binding[data.conBaseInfo.type].rotate.SetTransform(group);
                            data.con.SetBinding(Control.RenderTransformProperty, binding[data.conBaseInfo.type].rotatebinding);
                        }
                        if (enabletrs[1])
                        {
                           if(group==null) group = (TransformGroup)data.con.RenderTransform;
                            binding[data.conBaseInfo.type].offset.SetTransform(group.Children[0] as TranslateTransform);
                            data.con.SetBinding(Control.WidthProperty, binding[data.conBaseInfo.type].widthbinding);
                            data.con.SetBinding(Control.HeightProperty, binding[data.conBaseInfo.type].heightbinding);
                        }
                        attr_ui[nowtype].SetData(GetAttributeViewData(data.conBaseInfo.type, data.conBaseInfo.con_id));
                        lastcon = data.con;
                        lastconid = data.conBaseInfo.con_id;
                        attr_ui[nowtype].GetView().Visibility = System.Windows.Visibility.Visible;
                        Editor_GobalVar.GetIns().stopatrui = false;
                        //TODO 旋转
                    }
                    catch
                    {
                        attr_ui[nowtype].GetView().IsEnabled = false;
                    }
                    break;
                case EditorUiEvent.Flash:
                     data =(ConBaseData)((object[])obj)[0];
                    attr_ui[data.conBaseInfo.type].FlashData(data.con, (uint)((object[])obj)[1]);
                    break;
                case EditorUiEvent.FlashConToData:
                    data = (ConBaseData)obj;
                    FlashConVarToConData(data.conBaseInfo.con_id,data.con,data.conBaseInfo.type);
                    break;
                case EditorUiEvent.CloseWindow:
                    attr_ui[nowtype].GetView().Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case EditorUiEvent.Delete:
                    data = (ConBaseData)obj;
                    TextBox[] pos2 = (TextBox[])attr_ui[nowtype].GetCom(0);//固定 0位置 1大小 2 旋转
                    TextBox[] size2 = (TextBox[])attr_ui[nowtype].GetCom(1);
                    Slider slider2 = ((Slider)((object[])attr_ui[nowtype].GetCom(2))[0]);
                    BindingOperations.ClearAllBindings(pos2[0]);
                    BindingOperations.ClearAllBindings(pos2[1]);
                    BindingOperations.ClearAllBindings(size2[0]);
                    BindingOperations.ClearAllBindings(size2[1]);
                    BindingOperations.ClearAllBindings(slider2);
                    BindingOperations.ClearAllBindings(data.con);
                    attr_ui[data.conBaseInfo.type].GetView().Visibility = System.Windows.Visibility.Collapsed;
                    control_data.Remove(data.conBaseInfo.con_id);
                    lastcon = null;
                    lastconid = UInt32.MaxValue;
                    scroll.Content = null;
                    break;
            }
        }
        /// <summary>
        /// 把实际的空间的TRS设置到数据中
        /// </summary>
        /// <param name="id"></param>
        public void FlashConVarToConData(uint id,Control con,EControlType type)
        {
            //注，这样子可行是因为selector比较特殊，其无参数需要保存，若需要保存参数，则不可以直接获取AttributeOperInterFace，因为Attr_Con系列类，是单利类，这样子可能会覆盖当前所选相同控件内容。
            AttributeOperInterFace datas = GetAttributeViewData(type,id);
            datas.FlashData(con,128);//128是实现该功能的属性号
        }
        public void DoSave(BaseSaveFile basefile)
        {
            ((EditorSaveFile)basefile).control_data = control_data;
        }

        public void DoLoad(BaseSaveFile basefile)
        {
           control_data=((EditorSaveFile)basefile).control_data;
        }

        public void DoPush(BaseSaveFile basefile)
        {
            Dictionary<uint, object> temp = ((EditorSaveFile)basefile).control_data;
            foreach(uint key in temp.Keys)
            {
                control_data.Add(key, temp[key]);
            }
        }
        //添加控件更新
    }
}
