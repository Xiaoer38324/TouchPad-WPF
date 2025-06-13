using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    /// <summary>
    /// 自定义属性控件的时候，记得要去Attribute里面注册 还有Component_Ins_Creator
    /// </summary>
    internal abstract class AttributeItem
    {

        protected string name;
        protected string describe;
        protected uint id;
       internal AttributeItem(uint id,string name,string describe)
        {
            this.id = id;
            this.name = name;
            this.describe = describe;

        }
        public abstract object InitGetView(out Panel planel);//应包含注册事件
    }
}
