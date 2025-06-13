using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1.Attributes
{
    public delegate bool ResourceDialog(out string path);
    internal class A_ResourceSelect_Par : BaseComparData
    {
        public string path;
        public Func<string, string> process;
        public ResourceDialog dialog;

        public A_ResourceSelect_Par(uint id, string name, string describe) : base(id, name, describe) { 
        }
    }
    internal class A_ResourceSelect : AttributeItem
    {
        A_ResourceSelect_Par par;
        public A_ResourceSelect(A_ResourceSelect_Par par) : base(par.id, par.name, par.describe)
        {
            this.par = par;
        }

        public override object InitGetView(out Panel panel)
        {
            Grid grid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = GridLength.Auto;
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition2.Width = GridLength.Auto;
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);
            Label atr_name = new Label();
            atr_name.Content = name;
            atr_name.SetValue(Grid.ColumnProperty, 0);
            Label path = new Label();
            path.Content = par.path;
            path.SetValue(Grid.ColumnProperty, 1);
            Button selectbutton = new Button();
            selectbutton.Content = "选择";
            selectbutton.SetValue(Grid.ColumnProperty, 2);
            grid.Children.Add(atr_name);
            grid.Children.Add(selectbutton);
            grid.Children.Add(path);
           
            selectbutton.AddHandler(Button.ClickEvent, new RoutedEventHandler(delegate (object sneder, RoutedEventArgs e)
            {
                //选择文件
                string filepath="";
                if (par.dialog(out filepath) ==true)
                {
                    if (par.process != null)
                        path.Content = par.process(filepath);
                    else
                        path.Content = filepath;
                        Editor_GobalVar.GetIns().callback(id, filepath);
                }
            }));
            path.AddHandler(Label.MouseDoubleClickEvent, new MouseButtonEventHandler(delegate (object sneder, MouseButtonEventArgs e)
            {
                //选择文件
                    path.Content = "";
                    Editor_GobalVar.GetIns().callback(id,"Clear");                
            }));
            panel = grid;
            return path;
        }
    }
}
