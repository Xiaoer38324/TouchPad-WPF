using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Attributes;

namespace WpfApp1
{
    /// <summary>
    /// Con_Search.xaml 的交互逻辑
    /// </summary>
    public partial class Con_Search : Window
    {
        public Con_Search()
        {
            InitializeComponent();
        }
        
        public void SetSource(List<ConBaseInfo> info)
        {
          
            ConList.ItemsSource = info;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ConList.ItemsSource);
            view.Filter = Filter;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(ConList.Items.Count<1)
            MessageBox.Show("没有任何数据");
            else if(ConList.Items.Count==1){
                Editor_GobalVar.GetIns().logic.LockCon(((ConBaseInfo)(ConList.Items[0])).con_id);
                Close();
            }
            else
            {
                if (ConList.SelectedValue == null) MessageBox.Show("选择你要定位的控件");
                else
                {
                    Editor_GobalVar.GetIns().logic.LockCon(((ConBaseInfo)(ConList.SelectedValue)).con_id);
                    Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Type_Filter.ItemsSource = Enum.GetNames(typeof(EControlType));
            Type_Filter.SelectedIndex = 0;
        }
        private bool Filter(object obj)
        {
            ConBaseInfo info = (ConBaseInfo)obj;
            if (Type_Filter.SelectedValue ==null|| (string)Type_Filter.SelectedValue == "None"||Enum.GetName(info.type)==(string)Type_Filter.SelectedValue)
            {
                if (info.name.Contains(Name_Filter.Text)||String.IsNullOrEmpty(Name_Filter.Text))
                {
                    if (info.remarks.Contains(Remarks_Filter.Text)|| String.IsNullOrEmpty(Name_Filter.Text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ConList.ItemsSource!=null)
            CollectionViewSource.GetDefaultView(ConList.ItemsSource).Refresh();
        }

        private void Type_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConList.ItemsSource != null)
                CollectionViewSource.GetDefaultView(ConList.ItemsSource).Refresh();
        }
    }
}
