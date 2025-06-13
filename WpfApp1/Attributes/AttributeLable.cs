using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    internal abstract class AttributeLable
    {
        protected string name;

        protected LinkedList<AttributeItem> attributeItem;
        internal AttributeLable()
        {
            attributeItem = new LinkedList<AttributeItem>();
        }
        internal string GetName()
        {
            return name;
        }
        internal Grid GetGrid(Func<uint, object> callback)
        {
            Grid onelable = new Grid();
            int i = 0;
            foreach (AttributeItem item in attributeItem)
            {
                //RowDefinition definition = new RowDefinition();
                //definition.Height = GridLength.Auto;
                //onelable.RowDefinitions.Add(definition);
                //Panel p = item.InitGetView(callback);
                //p.SetValue(Grid.RowProperty, i);
                //onelable.Children.Add(p);
                //i++;
            }
            return onelable;
        }
    }
}
