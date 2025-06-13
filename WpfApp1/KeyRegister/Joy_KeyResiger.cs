using System;
using System.Windows;
using System.Windows.Controls;
using WindowsInput;

namespace WpfApp1.Touch
{
    internal class Joy_KeyResiger : KeyRegisterItem
    {
        ComboBox[] boxs;
        Panel panel = null;
        protected override void InitGetView()
        {
            boxs = new ComboBox[4];
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
            row4.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row5 = new RowDefinition();
            row5.Height = new GridLength(5, GridUnitType.Star);
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.ColumnDefinitions.Add(columnDefinition2);
            grid.RowDefinitions.Add(row);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);
            grid.RowDefinitions.Add(row5);
            TextBlock up, left, down, right;
            up = new TextBlock() { Text = "上：" };
            left = new TextBlock() { Text = "左：" };
            down = new TextBlock() { Text = "下：" };
            right = new TextBlock() { Text = "右：" };
            Grid.SetRow(up, 0);
            Grid.SetRow(right, 1);
            Grid.SetRow(down, 2);
            Grid.SetRow(left, 3);
            Grid.SetColumn(up, 0);
            Grid.SetColumn(right, 0);
            Grid.SetColumn(down, 0);
            Grid.SetColumn(left, 0);
            grid.Children.Add(up);
            grid.Children.Add(right);
            grid.Children.Add(down);
            grid.Children.Add(left);
            for (int i = 0; i < 4; i++)
            {
                boxs[i] = new ComboBox();
                boxs[i].ItemsSource = Enum.GetNames(typeof(VirtualKeyCode));
                boxs[i].SelectedIndex = 0;
                boxs[i].SelectedIndex = 0;
                grid.Children.Add(boxs[i]);
            }
            Grid.SetRow(boxs[0], 0);
            Grid.SetColumn(boxs[0], 1);
            Grid.SetRow(boxs[1], 1);
            Grid.SetColumn(boxs[1], 1);
            Grid.SetRow(boxs[2], 2);
            Grid.SetColumn(boxs[2], 1);
            Grid.SetRow(boxs[3], 3);
            Grid.SetColumn(boxs[3], 1);
            panel = grid;
        }

        internal override void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                boxs[i].SelectedIndex = 0;
            }
        }

        internal override VirtualKeyCode[] GetKey()
        {
            VirtualKeyCode[] keys = new VirtualKeyCode[4];
            for (int i = 0; i < 4; i++)
            {
                keys[i] = Enum.Parse<VirtualKeyCode>((string)boxs[i].SelectedValue);
            }
            return keys;
        }

        internal override Panel GetPanel()
        {
            if (panel == null)
                InitGetView();
            return panel;
        }

        internal override bool IsVailed()
        {
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                num += boxs[i].SelectedIndex;
            }
            if (num == 0) return false;
            return true;
        }

        internal override void Reset(VirtualKeyCode[] keys)
        {
            for (int i = 0; i < 4; i++)
            {
                if (keys[i] != VirtualKeyCode.None)
                    boxs[i].SelectedItem = keys[i].ToString();
            }
        }
    }
}
