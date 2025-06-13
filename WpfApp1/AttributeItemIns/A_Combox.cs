using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    internal class A_Combox_Par : BaseComparData
    {
        public int defaultindex=0;
        public object[] contents;

        public A_Combox_Par(uint id, string name, string describe) : base(id, name, describe)
        {
        }
    }
    internal class A_Combox : AttributeItem
    {
        A_Combox_Par par;
        public A_Combox(A_Combox_Par par) : base(par.id, par.name, par.describe)
        {
            this.par = par;
        }

        public override object InitGetView(out Panel planel)
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = GridLength.Auto;
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition1);
            Label atr_name = new Label();
            atr_name.Content = name;
            atr_name.SetValue(Grid.ColumnProperty, 0);
            ComboBox box = new ComboBox();
            foreach (object obj in par.contents)
            {
                box.Items.Add(obj);
            }
            box.SelectedIndex = par.defaultindex;
            box.SetValue(Grid.ColumnProperty, 1);
            grid.Children.Add(atr_name);
            grid.Children.Add(box);
            box.AddHandler(ComboBox.SelectionChangedEvent, new SelectionChangedEventHandler(delegate (object sneder, SelectionChangedEventArgs e)
            {
                Editor_GobalVar.GetIns().callback(id, box.SelectedIndex);
            }));
            planel = grid;
            return box;
        }
    }
}
