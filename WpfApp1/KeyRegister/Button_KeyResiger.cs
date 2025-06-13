using System;
using System.Windows;
using System.Windows.Controls;
using WindowsInput;

namespace WpfApp1.Touch
{
    internal class Button_KeyResiger : KeyRegisterItem
    {
        Panel panel=null;
        CheckBox ctrl, shift, alt;
        ComboBox box;
        protected override void InitGetView()
        {
                Grid grid = new Grid();
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition columnDefinition2 = new ColumnDefinition();
                columnDefinition2.Width = new GridLength(1, GridUnitType.Star);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition row2 = new RowDefinition();
                row2.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition row3 = new RowDefinition();
                row3.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition row4 = new RowDefinition();
                row4.Height = new GridLength(5, GridUnitType.Star);
                grid.ColumnDefinitions.Add(columnDefinition);
                grid.ColumnDefinitions.Add(columnDefinition2);
                grid.RowDefinitions.Add(row);
                grid.RowDefinitions.Add(row2);
                grid.RowDefinitions.Add(row3);
                grid.RowDefinitions.Add(row4);
                ctrl = new CheckBox();
                ctrl.Content = "Ctrl";
                Grid.SetColumn(ctrl, 0);
                Grid.SetRow(ctrl,0);
                shift = new CheckBox();
                shift.Content = "Shift";
                Grid.SetColumn(shift, 0);
                Grid.SetRow(shift, 1);
                alt = new CheckBox();
                alt.Content = "Alt";
                Grid.SetColumn(alt, 0);
                Grid.SetRow(alt, 2);
                box = new ComboBox();
                box.ItemsSource = Enum.GetNames(typeof(VirtualKeyCode));
                box.SelectedIndex = 0;
                Grid.SetColumn(box, 1);
                Grid.SetRow(box, 1);
                grid.Children.Add(ctrl);
                grid.Children.Add(shift);
                grid.Children.Add(alt);
                grid.Children.Add(box);
            panel = grid;    
        }

        internal override void Clear()
        {
            ctrl.IsChecked = false;
            shift.IsChecked = false;
            alt.IsChecked = false;
            box.SelectedIndex = 0;
        }

        internal override VirtualKeyCode[] GetKey()
        {
            int size = 1;
            if (ctrl.IsChecked==true) size++;
            if (alt.IsChecked == true) size++;
            if (shift.IsChecked == true) size++;
            VirtualKeyCode[] key = new VirtualKeyCode[size];
            size = 0;
            if (ctrl.IsChecked == true) { key[size] = VirtualKeyCode.LCONTROL; size++; }
            if (alt.IsChecked == true) { key[size] = VirtualKeyCode.LMENU; size++; }
            if (shift.IsChecked == true) { key[size] = VirtualKeyCode.LSHIFT; size++; }
            key[size] = Enum.Parse<VirtualKeyCode>((string)box.SelectedValue);
            return key;
        }

        internal override Panel GetPanel()
        {
            if (panel == null)
                InitGetView();
                return panel;
        }

        internal override bool IsVailed()
        {
            return !(box.SelectedIndex == 0);
        }

        internal override void Reset(VirtualKeyCode[] keys)
        {
            int i = 0;
            if (keys[i]==VirtualKeyCode.LCONTROL)
            {
                ctrl.IsChecked = true;
                i++;
            }
            if (keys[i] == VirtualKeyCode.LMENU)
            {
                alt.IsChecked = true;
                i++;
            }
            if (keys[i] == VirtualKeyCode.LSHIFT)
            {
                shift.IsChecked = true;
                i++;
            }
            box.SelectedValue = keys[i].ToString();
        }
    }
}
