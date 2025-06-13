using System;
using System.Windows.Controls;
using WpfApp1.Attributes;
internal class TestAttrData : AttributeOperInterFace
{
    Attr_Lable[] lable;
    A_TwoTextEditorWithDouble_Par par1;
    A_TwoTextEditorWithText_Par par2;
    A_Bar_Par par3;
    A_ResourceSelect_Par par4;
    A_Combox_Par par5;
    internal TestAttrData()
    {

        //lable =new Attr_Lable[]{new Attr_Lable("基础:",new uint[] {0,1}),new Attr_Lable("TestCom2",new uint[] {2,3,4})};
        //par1 = new A_TwoTextEditorWithDouble_Par();
        //par1.name = "位置:";
        //par1.id = 0;
        //par1.par1name = "X:";
        //par1.par2name = "Y:";
        //par1.p1 = 0;
        //par1.p2 = 0;
        //par1.describe = "pos";
        ////-----------------
        //par2 = new A_TwoTextEditorWithText_Par();
        //par2.name = "名字:";
        //par2.id = 1;
        //par2.par1name = "姓:";
        //par2.par2name = "名:";
        //par2.p1 = "X";
        //par2.p2 = "X";
        //par2.describe = "name";
        ////s---------
        //par3 = new A_Bar_Par();
        //par3.id = 2;
        //par3.min = 0;
        //par3.max = 360;
        //par3.name = "度数:";
        //par3.p = 180;
        //par3.describe = "degree";
        ////--------------------
        //par4 = new A_ResourceSelect_Par();
        //par4.id = 3;
        //par4.name = "照片";
        //par4.describe = "picture";
        //par4.path = "NULL";
        //par4.dialog = new Microsoft.Win32.OpenFileDialog();
        //par4.dialog.Filter = "照片文件|*.jpg;*.png";
        //par4.dialog.Title = "选择照片";
        ////-------------------
        //par5 = new A_Combox_Par();
        //par5.id = 4;
        //par5.name = "拉选框测试:";
        //par5.describe = "combox";
        //par5.contents = new String[] {"不选","测试0","测试1","测试2","测试3"};
        //par5.defaultindex = 0;
    }

    public void FlashData(Control button, uint whitch)
    {
        throw new NotImplementedException();
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
                return par1;
                break;
            case 1:
                return par2;
                break;
            case 2:
                return par3;
                break;
            case 3:
                return par4;
                break;
            case 4:
                return par5;
                break;
            default:return ComponentType.None;
        }
    }
    public ComponentType GetTypeById(uint id)
    {
        switch (id)
        {
            case 0:
                return ComponentType.A_TwoTextEditorWithDouble;
                break;
            case 1:
                return ComponentType.A_TwoTextEditorWithText;
                break;
            case 2:
                return ComponentType.A_Bar;
                break;
            case 3:
                return ComponentType.A_ResourceSelect;
                break;
            case 4:
                return ComponentType.A_Combox;
                break;
            default: return ComponentType.None;

        }
    }

    public void Inform(uint id, object result)
    {
     //   string[] pp = (string[])result; 
       // MessageBox.Show(id + ":" + pp[0] + "|" + pp[1]);
    }
}

