using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Attributes
{
    internal class A_TwoTextEditorWithDouble_Par : BaseComparData
    {
        internal string par1name;
        internal string par2name;
        public double p1;
        public double p2;
        public Func<double,object, double> process;
        public A_TwoTextEditorWithDouble_Par(uint id, string name, string describe) : base(id, name, describe)
        {
        }
    }
    internal class A_TwoTextEditorWithDouble : AttributeItem
    {
        A_TwoTextEditorWithDouble_Par par = null;
        double[] result;
        double correctp1, correctp2;
        internal A_TwoTextEditorWithDouble(A_TwoTextEditorWithDouble_Par par) : base(par.id, par.name, par.describe)
        {
            this.par = par;
            correctp1 = par.p1;
            correctp2= par.p2;
            result = new double[2];
        }
        public override object InitGetView(out Panel planel)
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
            columnDefinition4.Width = new GridLength(1, GridUnitType.Star); ;
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
            TextBox firstbox = new TextBox();
            TextBox secondbox = new TextBox();
            firstbox.MaxLength = 6;
            secondbox.MaxLength = 6;
            firstbox.Text = par.p1.ToString();
            secondbox.Text = par.p2.ToString();
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
            firstbox.AddHandler(TextBox.TextChangedEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                double value;
                if (Double.TryParse(firstbox.Text, out value)) {
                    result[1] = 0;
                    if (par.process == null)
                    {
                        result[0] = value;
                        Editor_GobalVar.GetIns().callback(id,result);
                    }
                    else
                    {
                        result[0] = par.process(value, true);
                        Editor_GobalVar.GetIns().callback(id, result);
                    }
                        
                    correctp1 = value;
                }
               

            }));
            firstbox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                double value;
                if (!Double.TryParse(firstbox.Text, out value)) { firstbox.Text = correctp1.ToString(); };
            }));
            secondbox.AddHandler(TextBox.TextChangedEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                double value;
                if (Double.TryParse(secondbox.Text, out value))
                {
                    result[1] = 1;
                    if (par.process == null)
                    {
                        result[0] = value;
                        Editor_GobalVar.GetIns().callback(id, result);
                    }
                    else
                    {
                        result[0] = par.process(value, true);
                        Editor_GobalVar.GetIns().callback(id, result);
                    }
                    correctp2 = value;
                }
            }));
            secondbox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler(delegate (object sender, RoutedEventArgs e) {
                double value;
                if (!Double.TryParse(secondbox.Text, out value)) { secondbox.Text = correctp2.ToString(); };
            }));
            InputMethod.SetIsInputMethodEnabled(firstbox,false);
            InputMethod.SetIsInputMethodEnabled(secondbox, false);
            firstbox.AddHandler(TextBox.PreviewTextInputEvent,new TextCompositionEventHandler(delegate (object sender, TextCompositionEventArgs e) {
                Regex re = new Regex("[^0-9.-]+");
                e.Handled = re.IsMatch(e.Text);
            }));
            secondbox.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(delegate (object sender, TextCompositionEventArgs e) {
                Regex re = new Regex("[^0-9.-]+");
                e.Handled = re.IsMatch(e.Text);
            }));
            planel = grid;
            return new TextBox[] {firstbox,secondbox};
        }
    }
}
