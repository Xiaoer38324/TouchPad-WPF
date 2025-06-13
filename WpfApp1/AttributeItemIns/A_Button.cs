using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Attributes
{
    internal class A_Button_Par : BaseComparData
    {
        public Action callback;
        public A_Button_Par(uint id, string name, string describe) : base(id, name, describe)
        {
        }
    }
    internal class A_Button : AttributeItem
    {
        A_Button_Par par;
        public A_Button(A_Button_Par a_Button_Par) : base(a_Button_Par.id, a_Button_Par.name, a_Button_Par.describe)
        {
            this.par = a_Button_Par;
        }

        public override object InitGetView(out Panel planel)
        {
            Grid grid = new Grid();
            Button button = new Button();
            button.Content = par.name;
            grid.Children.Add(button);
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                par.callback();
            }));
            planel = grid;
            return button;
        }
    }
}
