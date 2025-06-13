using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    internal class ListBox_DoubleClick
    {
      public static bool DoubleClick(ListBox list,out ListBoxItem item)
        {
            item = null;
             if (list.SelectedIndex >= 0)
                item = list.ItemContainerGenerator.ContainerFromIndex(list.SelectedIndex) as ListBoxItem;
            if (item == null) return false;
            else if (item.IsMouseOver)
            {
                
               return true;
            }
            return false;
        }
    }
}
