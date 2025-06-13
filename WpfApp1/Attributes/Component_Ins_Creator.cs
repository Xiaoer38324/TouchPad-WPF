using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.User_Control;

namespace WpfApp1.Attributes
{
    internal class BaseComparData
    {
        internal uint id;
        internal string name;
        internal string describe;

        public BaseComparData(uint id, string name, string describe)
        {
            this.id = id;
            this.name = name;
            this.describe = describe;
        }
    }
    //添加组件更新
    internal static class _Ins_Creator
    {
     internal static  AttributeItem Create(ComponentType type,object data){
            BaseComparData par;
            switch (type)
            {
                case ComponentType.A_TwoTextEditorWithText:
                    par = (A_TwoTextEditorWithText_Par)data;
                    return new A_TwoTextEditorWithText((A_TwoTextEditorWithText_Par)par);
                    break;
                case ComponentType.A_TwoTextEditorWithDouble:
                   par = (A_TwoTextEditorWithDouble_Par)data;
                    return new A_TwoTextEditorWithDouble((A_TwoTextEditorWithDouble_Par)par);
                    break;
                case ComponentType.A_Bar:
                    par = (A_Bar_Par)data;
                    return new A_Bar((A_Bar_Par)par);
                    break;
                case ComponentType.A_ResourceSelect:
                    par = (A_ResourceSelect_Par)data;
                    return new A_ResourceSelect((A_ResourceSelect_Par)par);
                    break;
                case ComponentType.A_Combox:
                    par = (A_Combox_Par)data;
                    return new A_Combox((A_Combox_Par)par);
                    break;
                case ComponentType.A_Bar_NAO:
                    par = (A_Bar_Par)data;
                    return new A_Bar_ReturnNewAndOld((A_Bar_Par)par);
                case ComponentType.A_Button:
                    par = (A_Button_Par)data;
                    return new A_Button((A_Button_Par)par);
                default:
                    return new A_NoneComponet(((BaseComparData)data));
                    break;
            }
        }
        //添加控件更新
        internal static Control CreateCon(EControlType type,bool image=true)
        {
            switch (type)
            {
                case EControlType.Button:
                    NormalUButton button = new NormalUButton();
                    button.TheButton.OpacityMask = new ImageBrush();
                    if (image)
                    {
                        button.TheButton.OpacityMask.SetValue(ImageBrush.ImageSourceProperty, Editor_GobalVar.GetIns().imagecache.GetBitmap("ButtonDefMask"));
                        button.TheButton.Source = Editor_GobalVar.GetIns().imagecache.GetBitmap("ButtonDefImage");
                    }
                    return button;
                case EControlType.Joy:
                    NormalJoy joy = new NormalJoy();
                    if (image)
                    {
                        joy.BackGroundSource = Editor_GobalVar.GetIns().imagecache.GetBitmap("JoyBGDef");
                        joy.GripMaskSource = Editor_GobalVar.GetIns().imagecache.GetBitmap("ButtonDefMask");
                        joy.GripSource = Editor_GobalVar.GetIns().imagecache.GetBitmap("ButtonDefMask");
                    }
                    return joy;
                case EControlType.Group:
                    Con_Editor_Group group = new Con_Editor_Group();
                    group.OpacityMask = new ImageBrush(Editor_GobalVar.GetIns().imagecache.GetBitmap("ButtonDefMask"));
                    group.Background =new ImageBrush(Editor_GobalVar.GetIns().imagecache.GetBitmap("GroupDefImage"));
                    return group;
            }
            return null;
        }
    }
}
