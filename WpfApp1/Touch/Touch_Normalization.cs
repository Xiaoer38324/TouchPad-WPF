using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsInput;
using WpfApp1.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp1.Touch
{
    internal struct LayoutInfo
    {
       public List<object> attributes;
        public List<string> names;
        public double width;
        public double height;
    }
    internal class Touch_Normalization
    {
        Func<Control,object, bool> callback;
        LayoutInfo info;
        double scalex=0,scaley=0;
        bool inited = false;
        
        internal Touch_Normalization(Func<Control, object, bool> callback, LayoutInfo info,double realwidth,double realheight)
        {
            this.callback = callback;
           
            scalex =   realwidth/ info.width;
            scaley = realheight / info.height;
         //   info.width = realwidth;
           // info.height = realheight;
            this.info = info;
        }
        internal Touch_Normalization(Func<Control, object, bool> callback, LayoutInfo info)
        {
            this.callback = callback;
            this.info = info;
        }
        internal void SetRealSize(double realwidth, double realheight)
        {
            scalex = realwidth / info.width;
            scaley = realheight / info.height;
         //   info.width = realwidth;
          //  info.height = realheight;
        }
        internal void ReSize(Canvas canvas,double realwidth, double realheight)
        {
            if (!inited) return;
            scalex = realwidth / info.width;
            scaley = realheight / info.height;
            //System.Diagnostics.Debug.WriteLine(realwidth + "?n" + realheight);
            //System.Diagnostics.Debug.WriteLine(info.width + "?o" + info.height);
            //info.width = realwidth;
            //info.height = realheight;
            TranslateTransform trans;
            double consize_scale;
            AttributeDataS data;
            Control c;
            for (int i= 0;i<canvas.Children.Count;i++)
            {
                c = canvas.Children[i] as Control;
                trans = ((TranslateTransform)((TransformGroup)c.RenderTransform).Children[0]);
                data = info.attributes[i] as AttributeDataS;
                c.SetValue(Canvas.LeftProperty, data.pos[0] * scalex);
                c.SetValue(Canvas.TopProperty, data.pos[1] * scaley);
              //  System.Diagnostics.Debug.WriteLine(c.ActualWidth+"|"+c.ActualHeight);
                consize_scale = data.scale[0] / data.scale[1];
                if (data.scale[0] * scalex > data.scale[1] * scaley)
                {
                    trans.X = -data.scale[1] * scaley * consize_scale / 2;
                    trans.Y = -data.scale[1] * scaley / 2;
                    c.SetValue(Control.HeightProperty, data.scale[1] * scaley);
                    c.SetValue(Control.WidthProperty, data.scale[1] * scaley * consize_scale);
                    
                }
                else
                {
                    trans.X = -data.scale[0] * scalex / 2;
                    trans.Y = -data.scale[0] * scalex * consize_scale / 2;
                    c.SetValue(Control.WidthProperty, data.scale[0] * scalex);
                    c.SetValue(Control.HeightProperty, (data.scale[0] * scalex) / consize_scale);
                }
              //  System.Diagnostics.Debug.WriteLine(c.Width + "}" + c.Height);
            }
        }
        internal void SetKeyCode(Dictionary<string, VirtualKeyCode[]> keys)
        {
            this.keys = keys;
        }
        Dictionary<string, VirtualKeyCode[]> keys;
        internal bool Operation(bool attribute = true, bool truecon = true, Dictionary<string, VirtualKeyCode[]> keys =null)
        {
            if (truecon == false && attribute == true) return false;
            if (inited) return false;
            if (keys == null) keys = this.keys;
            AttributeDataS data;
            double consize_scale;
            for (int i = 0; i < info.attributes.Count; i++)
            {
                data =GetCopy (info.attributes[i]) as AttributeDataS;
                string e_name = info.names[i];
                //1、如果是不需要的对象就不更新了
                if (!Filiter(data.type)) continue;
                //2、实例化对应的控件对象，并回调
                Control control;
                if (truecon)
                    control = _Ins_Creator.CreateCon(data.type,false);
                else
                    control = new Label();
                if (!callback(control, e_name)) continue;
                TransformGroup group= new TransformGroup();
                group.Children.Add(new TranslateTransform(-data.scale[0]*scalex / 2,-data.scale[1] * scaley / 2));
                group.Children.Add(new RotateTransform(data.rotate));
                group.Children.Add(new ScaleTransform(1,1));
                control.RenderTransform = group;
                consize_scale = data.scale[0] / data.scale[1];
                if (attribute)
                {
                    //1、对数据进行拉伸变换
                    data.pos[0] *= scalex;
                    data.pos[1] *= scaley;
                    
                    if (data.scale[0] * scalex > data.scale[1] * scaley)
                    {
                        data.scale[1] *= scaley;
                        data.scale[0] = (data.scale[1]*consize_scale);
                    }
                    else
                    {
                        data.scale[0] *= scalex;
                        data.scale[1] = (data.scale[0]/consize_scale);
                    }
                    //3、对对象对象进行属性刷新;
                    if (keys != null && keys.ContainsKey(e_name) && keys[e_name]!= null)
                            FlashCon(control, data, keys[e_name][keys[e_name].Length - 1]);
                        else
                            FlashCon(control, data);
                    
                   
                }
                else
                {
                    //若不需要属性刷新，则仅刷新基本信息
                    control.SetValue(Canvas.LeftProperty, data.pos[0]*scalex);
                    control.SetValue(Canvas.TopProperty, data.pos[1] * scaley);
                    if (data.scale[0] * scalex > data.scale[1] * scaley)
                    {
                        control.SetValue(Control.HeightProperty, data.scale[1] * scaley);
                        control.SetValue(Control.WidthProperty,  data.scale[1]* scaley * consize_scale);
                    }
                    else
                    {
                        control.SetValue(Control.WidthProperty, data.scale[0] * scalex);
                        control.SetValue(Control.HeightProperty, (data.scale[0] * scalex)/ consize_scale);
                    }
                }
                if (keys != null && keys[e_name] !=null)
                    //4、控件对象进行功能刷新暂时不搞,现在搞了。
                ((TouchActivator)control).Active(keys[e_name]);
            }
            inited = true;
            return true;

        }
        private object GetCopy(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            object copy = JsonConvert.DeserializeObject(json);
            copy=EditorSaveFile.ProcessJObjectToReal((JObject)copy);
           return copy;
        }
        //控件更新
        private bool Filiter(EControlType type)
        {
            if (type == EControlType.Button) return true;
            else if (type == EControlType.Joy) return true;
            return false;
        }
        //控件更新
        private void FlashCon(Control control,AttributeDataS data,VirtualKeyCode code=VirtualKeyCode.None)
        {
            switch (data.type)
            {
                case EControlType.Button:
                    ((Touch_BitmapImageCache)Editor_GobalVar.GetIns().imagecache).nextcode = code;
                    Attr_Button.GetIns((Button_Attribute)data).FlashData(control, uint.MaxValue);
                    Attr_Button.GetIns((Button_Attribute)data).SetClickAnimation(control);
                    break;
                case EControlType.Joy:
                    Attr_Joy.GetIns((Joy_Attribute)data).FlashData(control, uint.MaxValue);
                    break;
            }
        }
    }
}
