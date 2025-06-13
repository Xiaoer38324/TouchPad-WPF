using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfApp1.Attributes
{
    internal class A_Bar_Par : BaseComparData
    {
        public double p;
        public float min, max;

        public A_Bar_Par(uint id, string name, string describe) : base(id, name, describe)
        {
        }
    }
    internal class A_Bar : AttributeItem
    {
        A_Bar_Par par;
        public A_Bar(A_Bar_Par par) : base(par.id, par.name, par.describe)
        {
            this.par = par;
        }
        protected Slider bar;
        protected RoutedEventHandler eventhandle;
        public override object InitGetView( out Panel planel)
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = GridLength.Auto;
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition2.Width = new GridLength(0.3, GridUnitType.Star);
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            Label atr_name = new Label();
            atr_name.Content = name;
            atr_name.SetValue(Grid.ColumnProperty, 0);
            bar = new Slider();
            bar.Name = "thebar"+id;
            bar.Value = par.p;
            bar.Minimum = par.min;
            bar.Maximum = par.max;
            bar.SetValue(Grid.ColumnProperty, 1);
            TextBox bar_value = new TextBox();
            Binding databind = new Binding() { Source = bar, Path = new PropertyPath(Slider.ValueProperty), Mode = BindingMode.TwoWay };
            bar_value.SetBinding(TextBox.TextProperty,databind);
        //    bar_value.Content = "{Binding Content=Value,ElementName=" + "thebar"+id+"}";
            bar_value.SetValue(Grid.ColumnProperty, 2);
            grid.Children.Add(atr_name);
            grid.Children.Add(bar_value);
            grid.Children.Add(bar);
            eventhandle = new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                Editor_GobalVar.GetIns().callback(id, bar.Value);
            });
            bar.AddHandler(Slider.ValueChangedEvent, eventhandle);
            planel = grid;
            return new object[] { bar, bar_value };
        }
    }
}
