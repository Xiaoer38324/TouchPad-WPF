using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.Attributes
{
    internal class Group_Attribute:AttributeDataS
    {
        public uint[] childer;
        public uint myid;
        public Group_Attribute()
        {
            type = EControlType.Group;
            pos = new double[2] { 0, 0 };
            scale = new double[2] { 50, 50 };
        }
    }
    internal class Attr_Group : AttributeOperInterFace
    {
        A_TwoTextEditorWithDouble_Par pos;
        A_TwoTextEditorWithDouble_Par scale;
        A_Button_Par click;
        Attr_Lable[] lable;
        Group_Attribute attr;
        public Attr_Group()
        {
            lable = new Attr_Lable[] { new Attr_Lable("基础：", new uint[] { 0, 1,3})};
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
            click = new A_Button_Par(3,"选择群组","选择预设好的群组");
            click.callback = SelectGroup;
        }
        private void SelectGroup()
        {
            Editor_GobalVar.GetIns().logic.ShowSelector(attr.childer,attr.myid);
        }
        private static object obj = new object();
        private static Attr_Group ins;
        public static Attr_Group GetIns(Group_Attribute button_Attribute)
        {
            lock (obj)
            {
                if (ins == null) ins = new Attr_Group();
                ins.pos.p1 = button_Attribute.pos[0];
                ins.pos.p2 = button_Attribute.pos[1];
                ins.scale.p1 = button_Attribute.scale[0];
                ins.scale.p2 = button_Attribute.scale[1];
                ins.attr = button_Attribute;
                return ins;
            }

        }
        public void FlashData(Control button, uint which)
        {
            switch (which)
            {
                case 128:
                    attr.pos[0] = (double)button.GetValue(Canvas.LeftProperty);
                    attr.pos[1] = (double)button.GetValue(Canvas.TopProperty);
                    attr.scale[0] = (double)button.GetValue(Control.WidthProperty);
                    attr.scale[1] = (double)button.GetValue(Control.HeightProperty);
                    attr.rotate = (double)((RotateTransform)((TransformGroup)button.RenderTransform).Children[1]).Angle;
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
                case 1:
                    return scale;
                case 3:
                    return click;
                case 1024:
                    return attr.hideattr;
                default:
                    return ComponentType.None;
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
                case 3:
                    return ComponentType.A_Button;
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
                case 3:
                    attr.hideattr = (uint[])result;
                    break;
            }
        }
    }
}
