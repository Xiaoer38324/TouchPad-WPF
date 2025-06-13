using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    internal class A_NoneComponet : AttributeItem
    {
        public A_NoneComponet(BaseComparData data) : base(data.id, data.name, data.describe)
        {

        }

        public override object InitGetView(out Panel panel)
        {
            panel = null;
            return null;
        }
    }
}
