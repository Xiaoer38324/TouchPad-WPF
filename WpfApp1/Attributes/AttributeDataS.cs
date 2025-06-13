using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    //组件更新
    enum ComponentType{
    A_TwoTextEditorWithText,None,A_TwoTextEditorWithDouble,A_Bar,A_ResourceSelect,A_Combox,A_Bar_NAO,A_Button
    }
    internal struct Attr_Lable
    {
       internal uint[] index;
        internal string name;
        internal Attr_Lable(string name,uint[] index)
        {
            this.index = index;
            this.name = name;
        }
    }
    internal  interface AttributeOperInterFace
    {
        public  Attr_Lable[] GetAttr_Lables();//获取UI生成数据
        public  object GetDataById(uint id);//获取实际数据
        public  ComponentType GetTypeById(uint id);//获取UI生成类型
        public  void Inform(uint id, object result);
        public void FlashData(Control button,uint whitch);

    }
}
