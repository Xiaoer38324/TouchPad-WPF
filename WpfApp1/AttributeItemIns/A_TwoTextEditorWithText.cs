using System.Windows;
using System.Windows.Controls;


namespace WpfApp1.Attributes
{
    internal class A_TwoTextEditorWithText_Par : BaseComparData
    {
        internal string par1name;
        internal string par2name;
        public string p1;
        public string p2;

        public A_TwoTextEditorWithText_Par(uint id, string name, string describe) : base(id, name, describe)
        {
        }
    }
    internal class A_TwoTextEditorWithText : AttributeItem
    {
        A_TwoTextEditorWithText_Par par = null;
        internal A_TwoTextEditorWithText(A_TwoTextEditorWithText_Par par) : base(par.id, par.name, par.describe)
        {
            this.par = par;
        }
        public override object InitGetView(out Panel panel)
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = GridLength.Auto;
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = GridLength.Auto;
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition2.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition columnDefinition3 = new ColumnDefinition();
            columnDefinition3.Width = GridLength.Auto;
            ColumnDefinition columnDefinition4 = new ColumnDefinition();
            columnDefinition4.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            grid.ColumnDefinitions.Add(columnDefinition3);
            grid.ColumnDefinitions.Add(columnDefinition4);
            Label atr_name = new Label();
            Label atr_first = new Label();
            Label atr_second = new Label();
            atr_name.Content = name;
            atr_first.Content = par.par1name;
            atr_second.Content = par.par2name;
          TextBox   firstbox = new TextBox();
            TextBox secondbox = new TextBox();
            firstbox.Text = par.p1;
            secondbox.Text = par.p2;
            atr_name.SetValue(Grid.ColumnProperty, 0);
            atr_first.SetValue(Grid.ColumnProperty, 1);
            atr_second.SetValue(Grid.ColumnProperty, 3);
            grid.Children.Add(atr_name);
            grid.Children.Add(atr_first);
            grid.Children.Add(atr_second);
            grid.Children.Add(firstbox);
            grid.Children.Add(secondbox);
            firstbox.SetValue(Grid.ColumnProperty, 2);
            secondbox.SetValue(Grid.ColumnProperty, 4);
            firstbox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                Editor_GobalVar.GetIns().callback(id,new string[] {firstbox.Text,secondbox.Text});
            }));
            secondbox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                Editor_GobalVar.GetIns().callback(id, new string[] { firstbox.Text, secondbox.Text });
            }));
            panel = grid;
            return new TextBox[] {firstbox,secondbox};
        }
    }
}