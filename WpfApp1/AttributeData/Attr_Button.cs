using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.User_Control;

namespace WpfApp1.Attributes
{
    /*
     * 基础：
     * 背景，Mask，旋转
     * 进阶：
     * 动态效果
     */
    internal class AttributeDataS
    {
        public EControlType type;
        public double[] pos, scale;
        public double rotate;
        /// <summary>
        /// 必须严格保证单调递增有序！
        /// </summary>
        public uint[] hideattr;
    }
    internal class Button_Attribute:AttributeDataS
    {
      
        public string background,mask;
        public int dynamiceffect_default;
        public Button_Attribute()
        {
            type = EControlType.Button;
            pos = new double[2] {0,0};
            scale = new double[2] {50,50};
            background = "ButtonDefImage";
            mask = "ButtonDefMask";
            dynamiceffect_default = 0;
            rotate = 0;
        }
    }
    //自定义控件属性还是蛮简单的哈，先实现AttributeOperInterFace，搞一个数据类，上面那种，然后去AttributeManager里面CreateAttributeData，GetAttributeViewData注册一下就好了。
    internal class Attr_Button : AttributeOperInterFace
    {
        A_TwoTextEditorWithDouble_Par pos;
        A_TwoTextEditorWithDouble_Par scale;
        A_ResourceSelect_Par background;
        A_ResourceSelect_Par mask;
        A_Bar_Par rotate;
        A_Combox_Par dynamiceffect;
        Attr_Lable[] lable;
        Button_Attribute attr;
        private Attr_Button() {
            lable = new Attr_Lable[] { new Attr_Lable("基础：", new uint[] { 0,1,2}), new Attr_Lable("进阶：", new uint[] {3,4,5}) };
            background = new A_ResourceSelect_Par(5, "图标:", "设置按键的图标，即背景图片");
            background.dialog = OpenResourceSelection;
            mask = new A_ResourceSelect_Par(4, "遮罩:", "遮罩图片，用于创建异形按键");
            mask.dialog = OpenResourceSelection;
            rotate = new A_Bar_Par(2, "旋转:", "旋转组件");
            rotate.min = 0;
            rotate.max = 360;
            dynamiceffect = new A_Combox_Par(3, "点击动效:", "按键点击后产生的效果");
            dynamiceffect.contents = new string[] { "无", "变大", "变小" };
            pos = new A_TwoTextEditorWithDouble_Par(0,"位置:","设置组件的位置，以左上角为坐标系原点，向下为Y正方向，向右为X正方向");
            pos.par1name = "X:";
            pos.par2name = "Y:";
            pos.p1 = 0;
            pos.p2 = 0;
            scale = new A_TwoTextEditorWithDouble_Par(1,"大小:","组件大小，Height是高，Width是宽");
            scale.par1name = "Width:";
            scale.par2name = "Height:";
            scale.p1 = 0;
            scale.p2 = 0;
            //background.process = ResourcePathProcess;
            //mask.process = ResourcePathProcess;
            //pos.process = ProcessPos;
        }
        private bool OpenResourceSelection(out string path)
        {
            ResourceWindow window = new ResourceWindow();
            window.SetImageCache(Editor_GobalVar.GetIns().imagecache);
            window.SetMode(true);
            window.SetSource(Editor_GobalVar.GetIns().assetm);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                path = window.path;
                return true;
            }
            path = "";
            return false;
        }
        private static object obj = new object();
        private static Attr_Button ins;
        public static Attr_Button GetIns(Button_Attribute button_Attribute)
        {
            lock (obj)
            {
                if (ins==null) ins = new Attr_Button();
                ins.pos.p1 = button_Attribute.pos[0];
                ins.pos.p2 = button_Attribute.pos[1];
                ins.scale.p1 = button_Attribute.scale[0];
                ins.scale.p2= button_Attribute.scale[1];
                ins.background.path = button_Attribute.background;
                ins.mask.path = button_Attribute.mask;
                ins.rotate.p = button_Attribute.rotate;
                ins.dynamiceffect.defaultindex = button_Attribute.dynamiceffect_default;
                ins.attr = button_Attribute;
                return ins;
            }
           
        }

        public  Attr_Lable[] GetAttr_Lables()
        {
            return lable;
        }

        public  object GetDataById(uint id)
        {
            switch (id)
            {
                case 0:
                    return pos;
                case 1:
                    return scale;
                case 2:
                    return rotate;
                case 3:
                    return dynamiceffect;
                case 4:
                    return mask;
                case 5:
                    return background;
                case 1024:
                    return attr.hideattr;
                default:
                    return ComponentType.None;
            }
        }

        public  ComponentType GetTypeById(uint id)
        {
            switch (id)
            {
                case 0:
                    return ComponentType.A_TwoTextEditorWithDouble;
                case 1:
                    return ComponentType.A_TwoTextEditorWithDouble;
                case 2:
                    return ComponentType.A_Bar;
                case 3:
                    return ComponentType.A_Combox;
                case 4:
                    return ComponentType.A_ResourceSelect;
                case 5:
                    return ComponentType.A_ResourceSelect;
                default:
                    return ComponentType.None;
            }
        }
        public  void Inform(uint id, object result)
        {
            if (Editor_GobalVar.GetIns().stopatrui) return;
            double[] dresult=null;
            switch (id)
            {
                case 0:
                    dresult = (double[])result;

                    if (dresult[1] == 0)
                        attr.pos[0] = dresult[0];
                    else
                        attr.pos[1]= dresult[0];
                    break;
                case 1:
                    dresult = (double[])result;
                    if (dresult[1] == 0)
                        attr.scale[0] = dresult[0];
                    else
                        attr.scale[1] = dresult[0];
                    break;
                case 2:
                    attr.rotate = (double)result;
                    break;
                case 3:
                    attr.dynamiceffect_default = (int)result;
                    break;
                case 4:
                    Editor_GobalVar.GetIns().imagecache.FreeImage(attr.mask);
                    if ((string)result != "Clear")//Clear表示ResourceUi清除了选择
                    {
                      
                        attr.mask = (string)result;
                    }
                    else
                    {
                        attr.mask = "ButtonDefMask";
                    }
                    Editor_GobalVar.GetIns().logic.FlashActived(4);
                    break;
                case 5:
                    Editor_GobalVar.GetIns().imagecache.FreeImage(attr.background);
                    if ((string)result != "Clear")//Clear表示ResourceUi清除了选择
                    {
                      
                        attr.background = (string)result;
                    }
                    else
                    {
                        attr.background = "ButtonDefImage";
                    }
                  Editor_GobalVar.GetIns().logic.FlashActived(5);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="button"></param>
        /// <param name="which">属性号，一般有以下特殊的属性号 128：以实际控件的tsr刷新attr。其他的都是把控件刷新为attr</param>
        public void FlashData(Control button,uint which)
        {
            NormalUButton ubutton = (NormalUButton)button;
            if (which != UInt32.MaxValue)
            switch (which)
            {
                case 4:
                        //不需要重新设置Opmask，因为初始化的时候初始化了默认的
                            ubutton.MaskSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.mask);
                        break;
                case 5:
                            ubutton.Source = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.background);
                        break;
                 case 128:
                        attr.pos[0] = (double)button.GetValue(Canvas.LeftProperty);
                        attr.pos[1] = (double)button.GetValue(Canvas.TopProperty);
                        attr.scale[0]= (double)button.GetValue(Control.WidthProperty);
                        attr.scale[1] = (double)button.GetValue(Control.HeightProperty);
                        attr.rotate = (double)((RotateTransform)((TransformGroup)button.RenderTransform).Children[1]).Angle;
                        break;
                    default:
                    break;
            }
            else
            {
                button.SetValue(Canvas.LeftProperty, attr.pos[0]);
                button.SetValue(Canvas.TopProperty, attr.pos[1]);
                button.SetValue(Control.WidthProperty, attr.scale[0]);
                button.SetValue(Control.HeightProperty, attr.scale[1]);
                button.SetValue(RotateTransform.AngleProperty,attr.rotate);
                ubutton.MaskSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.mask);
                ubutton.Source = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.background);
            }
        }
        private void SetBig(object sender, MouseButtonEventArgs args)
        {
            ScaleTransform scale = ((ScaleTransform)((TransformGroup)((Control)sender).RenderTransform).Children[2]);
            scale.ScaleX = 1.5;
            scale.ScaleY = 1.5;
        }
        private void SetNormal(object sender, MouseButtonEventArgs args)
        {
            ScaleTransform scale = ((ScaleTransform)((TransformGroup)((Control)sender).RenderTransform).Children[2]);
            scale.ScaleX = 1;
            scale.ScaleY =1;
        }
        private void SetSmall(object sender, MouseButtonEventArgs args)
        {
            ScaleTransform scale = ((ScaleTransform)((TransformGroup)((Control)sender).RenderTransform).Children[2]);
            scale.ScaleX = 0.5;
            scale.ScaleY = 0.5;
        }
        public void SetClickAnimation(Control con)
        {
            switch (attr.dynamiceffect_default)
            {
                case 1:
                    con.MouseDown += SetBig;

                    con.MouseUp += SetNormal;
                    //变大
                    break;
                case 2:
                    con.MouseDown += SetSmall;

                    con.MouseUp += SetNormal;
                    //变小
                    break;
            }
        }
    }
}
