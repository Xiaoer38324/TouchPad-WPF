using System;
using System.Windows.Controls;
using WpfApp1.User_Control;

namespace WpfApp1.Attributes
{

    internal class Attr_Selector : AttributeOperInterFace
    {
        A_Bar_Par _rotate;//2
        A_Bar_Par con_rotate;//3
        A_Bar_Par _scale;//4
        A_Bar_Par con_scale;//5
        A_Bar_Par both;//6
        A_TwoTextEditorWithDouble_Par pos;//0
        Attr_Lable[] lable;

        private static object obj = new object();
        private static Attr_Selector ins;

        public Attr_Selector()
        {
            lable = new Attr_Lable[] { new Attr_Lable("基础：", new uint[] { 0, 2, 3,4,5,6})};
            _rotate = new A_Bar_Par(2,"整体旋转:","控件围绕框选点为中心旋转");
            _rotate.min = 0;
            _rotate.max = 360;
            _rotate.p = 0;
            con_rotate = new A_Bar_Par(3, "控件自转:", "控件围绕自己中心旋转");
            con_rotate.min = 0;
            con_rotate.max = 360;
            con_rotate.p = 0;
            _scale = new A_Bar_Par(4, "整体缩放:", "控件围绕框选点为中心缩放");
            _scale.min = 1;
            _scale.max = 7;
            _scale.p = 4;
            con_scale = new A_Bar_Par(5, "控件自缩:", "控件围绕自己中心缩放");
            con_scale.min = 1;
            con_scale.max = 7;
            con_scale.p = 4;
            both = new A_Bar_Par(6, "比例缩放:", "控件围绕自己中心缩放");
            both.min = 1;
            both.max = 7;
            both.p = 4;
            pos = new A_TwoTextEditorWithDouble_Par(0, "位置:", "设置组件的位置，以左上角为坐标系原点，向下为Y正方向，向右为X正方向");
            pos.par1name = "X:";
            pos.par2name = "Y:";
            pos.p1 = 0;
            pos.p2 = 0;
            r_scale = new double[2];
            rcon_scale= new double[2];
            r_oldpos = new double[2];
            r_rotate = new double[2];
            rcon_rotate = new double[2];
        }

        public static Attr_Selector GetIns()
        {
            lock (obj)
            {
                if (ins == null) ins = new Attr_Selector();
                ins._rotate.p = 0;
                ins.con_rotate.p = 0;
                ins.con_scale.p = 4;
                ins._scale.p = 4;
                ins.both.p = 4;
                return ins;
            }

        }
        double[] r_scale, rcon_scale, r_oldpos, r_rotate, rcon_rotate;
        public void FlashData(Control con, uint which)
        {
            Con_Editor_Selector selector = (Con_Editor_Selector)con;
            if (which != UInt32.MaxValue)
                switch (which)
                {
                    case 0:
                        selector.FlashPos(r_oldpos[0], r_oldpos[1]);
                        break;
                    case 2:
                        selector.FlashRotate(r_rotate[0] - r_rotate[1]);
                        break;
                    case 3:
                        selector.FlashSelfRotate(rcon_rotate[0] - rcon_rotate[1]);
                        break;
                    case 4:
                        selector.FlashScale(r_scale[0], r_scale[1]);
                        break;
                    case 5:
                        selector.FlashSelfScale(rcon_scale[0],rcon_scale[1]);
                        break;
                    case 6:
                        selector.FlashScale(r_scale[0], r_scale[1]);
                        selector.FlashSelfScale(rcon_scale[0], rcon_scale[1]);
                        break;
                    default:
                        break;
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
                case 2:
                    return _rotate;
                case 3:
                    return con_rotate;
                case 4:
                    return _scale;
                case 5:
                    return con_scale;
                case 6:
                    return both;
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
                case 2:
                    return ComponentType.A_Bar_NAO;
                case 3:
                    return ComponentType.A_Bar_NAO;
                case 4:
                    return ComponentType.A_Bar_NAO;
                case 5:
                    return ComponentType.A_Bar_NAO;
                case 6:
                    return ComponentType.A_Bar_NAO;
                default:
                    return ComponentType.None;
            }
        }
        public void Inform(uint id, object result)
        {
            
            switch (id)
            {
                case 0:
                    double[] dresult= (double[])result;
                    if (dresult[1] == 0)
                    {
                        r_oldpos[0] = dresult[0] - pos.p1;
                        r_oldpos[1] = 0;
                        pos.p1 = dresult[0];
                    }
                    else
                    {
                        r_oldpos[1] = dresult[0] - pos.p2;
                        r_oldpos[0] = 0;
                        pos.p2 = dresult[0];
                    }
                    //System.Diagnostics.Debug.WriteLine(xx+ "||" + yy);
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    Editor_GobalVar.GetIns().logic.FlashActived(0);
                    break;
                case 2:
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    r_rotate = (double[])result;
                    Editor_GobalVar.GetIns().logic.FlashActived(2);
                    break;
                case 3:
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    rcon_rotate = (double[])result;
                    Editor_GobalVar.GetIns().logic.FlashActived(3);
                    break;
                case 4:
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    r_scale = (double[])result;
                    Editor_GobalVar.GetIns().logic.FlashActived(4);
                    break;
                case 5:
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    rcon_scale = (double[])result;
                    Editor_GobalVar.GetIns().logic.FlashActived(5);
                    break;
                case 6:
                    if (Editor_GobalVar.GetIns().stopatrui) break;
                    rcon_scale = (double[])result;
                    r_scale = (double[])result;
                    Editor_GobalVar.GetIns().logic.FlashActived(6);
                    break;
                default:
                    break;
            }
        }
    }
}
