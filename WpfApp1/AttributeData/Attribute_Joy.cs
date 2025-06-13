using System;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.User_Control;

namespace WpfApp1.Attributes
{
    internal class Joy_Attribute:AttributeDataS{

        
        public string background, gmask,gsource;
        public Joy_Attribute()
        {
            type = EControlType.Joy;
            this.pos = new double[2] {0,0};
            this.scale = new double[2] {50,50};
            this.background = "JoyBGDef";
            this.gmask = "ButtonDefMask";
            this.gsource = "ButtonDefMask";
            this.rotate = 0;
        }
    }

    internal class Attr_Joy : AttributeOperInterFace
    {
        A_TwoTextEditorWithDouble_Par pos;
        A_TwoTextEditorWithDouble_Par scale;
        A_ResourceSelect_Par background;
        A_ResourceSelect_Par gmask;
        A_ResourceSelect_Par gsource;
        A_Bar_Par rotate;
        Attr_Lable[] lable;
        Joy_Attribute attr;
        private Attr_Joy()
        {
            lable = new Attr_Lable[] { new Attr_Lable("基础：", new uint[] { 0, 1, 2 }), new Attr_Lable("进阶：", new uint[] { 3, 4, 5 }) };
            background = new A_ResourceSelect_Par(5, "背景图片:", "设置按键的图标，即背景图片");
            background.dialog = OpenResourceSelection;
            gmask = new A_ResourceSelect_Par(4, "握把遮罩:", "遮罩图片，用于创建异形摇杆中间按键");
            gmask.dialog = OpenResourceSelection;
            rotate = new A_Bar_Par(2, "旋转:", "旋转组件");
            rotate.min = 0;
            rotate.max = 360;
            gsource = new A_ResourceSelect_Par(3, "握把图片:", "摇杆中心图片，圆形摇杆中间的地方");
            gsource.dialog = OpenResourceSelection;
            pos = new A_TwoTextEditorWithDouble_Par(0, "位置:", "设置组件的位置，以左上角为坐标系原点，向下为Y正方向，向右为X正方向");
            pos.par1name = "X:";
            pos.par2name = "Y:";
            pos.p1 = 0;
            pos.p2 = 0;
            scale = new A_TwoTextEditorWithDouble_Par(1, "大小:", "组件大小，Height是高，Width是宽");
            scale.par1name = "Width:";
            scale.par2name = "Height:";
            scale.p1 = 0;
            scale.p2 = 0;
            //background.process = ResourcePathProcess;
            //gsource.process = ResourcePathProcess;
            //gmask.process = ResourcePathProcess;
        }
        private bool OpenResourceSelection(out string path)
        {
            ResourceWindow window = new ResourceWindow();
            window.SetImageCache(Editor_GobalVar.GetIns().imagecache);
            window.SetSource(Editor_GobalVar.GetIns().assetm);
            window.SetMode(true);
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
        private static Attr_Joy ins;
        public static Attr_Joy GetIns(Joy_Attribute joy)
        {
            lock (obj)
            {
                if (ins == null) ins = new Attr_Joy();
                ins.pos.p1 = joy.pos[0];
                ins.pos.p2 = joy.pos[1];
                ins.scale.p1 = joy.scale[0];
                ins.scale.p2 = joy.scale[1];
                ins.background.path = joy.background;
                ins.gmask.path = joy.gmask;
                ins.rotate.p = joy.rotate;
                ins.gsource.path = joy.gsource;
                ins.attr = joy;
                return ins;
            }

        }

        public Attr_Lable[] GetAttr_Lables()
        {
            return lable;
        }

        public object GetDataById(uint id)
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
                    return gsource;
                case 4:
                    return gmask;
                case 5:
                    return background;
                default:
                    return null;
            }
        }

        public ComponentType GetTypeById(uint id)
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
                    return ComponentType.A_ResourceSelect;
                case 4:
                    return ComponentType.A_ResourceSelect;
                case 5:
                    return ComponentType.A_ResourceSelect;
                default:
                    return ComponentType.None;
            }
        }

        public void Inform(uint id, object result)
        {
            if (Editor_GobalVar.GetIns().stopatrui) return;
            double[] dresult = null;
            switch (id)
            {
                case 0:
                    dresult = (double[])result;
                    if (dresult[1] == 0)
                        attr.pos[0] = dresult[0];
                    else
                        attr.pos[1] = dresult[0];
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
                    Editor_GobalVar.GetIns().imagecache.FreeImage(attr.gsource);
                    if ((string)result != "Clear")//Clear表示ResourceUi清除了选择
                    {
                       
                        attr.gsource = (string)result;
                    }
                    else
                    {
                        attr.gsource = "ButtonDefMask";
                    }
                    Editor_GobalVar.GetIns().logic.FlashActived(3);
                    break;
                case 4:
                    Editor_GobalVar.GetIns().imagecache.FreeImage(attr.gmask);
                    if ((string)result != "Clear")//Clear表示ResourceUi清除了选择
                    {
                       
                        attr.gmask = (string)result;
                    }
                    else
                    {
                        attr.gmask = "ButtonDefMask";
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
                        attr.background = "JoyBGDef";
                    }
                    attr.background= (string)result;
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
        public void FlashData(Control ajoy,uint which)
        {
            NormalJoy joy = (NormalJoy)ajoy;
            if(which!=UInt32.MaxValue)
            switch (which)
            {
               
                case 3:
                    joy.GripSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.gsource);
                        break;
                case 4:
                    joy.GripMaskSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.gmask);
                        break;
                case 5:
                    joy.BackGroundSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.background);
                        break;
                case 128:
                        attr.pos[0] = (double)ajoy.GetValue(Canvas.LeftProperty);
                        attr.pos[1] = (double)ajoy.GetValue(Canvas.TopProperty);
                        attr.scale[0] = (double)ajoy.GetValue(Control.WidthProperty);
                        attr.scale[1] = (double)ajoy.GetValue(Control.HeightProperty);
                        attr.rotate = (double)((RotateTransform)((TransformGroup)joy.RenderTransform).Children[1]).Angle;
                        break;
                    default:
                    break;
            }
            else
            {
                joy.SetValue(Canvas.LeftProperty, attr.pos[0]);
                joy.SetValue(Canvas.TopProperty, attr.pos[1]);
                joy.SetValue(Control.WidthProperty, attr.scale[0]);
                joy.SetValue(Control.HeightProperty, attr.scale[1]);
                joy.SetValue(RotateTransform.AngleProperty, attr.rotate);
                joy.GripSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.gsource);
                joy.GripMaskSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.gmask);
                joy.BackGroundSource = Editor_GobalVar.GetIns().imagecache.GetBitmap(attr.background);
                joy.SetRange((int)(attr.scale[0] < attr.scale[1] ? attr.scale[0] : attr.scale[1])/2);
                        
            }
        }
    }
}
