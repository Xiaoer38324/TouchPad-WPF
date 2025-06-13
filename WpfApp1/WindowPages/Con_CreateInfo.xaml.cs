using System;
using System.Windows;
using WpfApp1.Attributes;

namespace WpfApp1
{
    /// <summary>
    /// Con_CreateInfo.xaml 的交互逻辑
    /// </summary>
    public partial class Con_CreateInfo : Window
    {
        public string name,remarks;
        public EControlType type;
        public Con_CreateInfo()
        {
            InitializeComponent();
            string[] type= Enum.GetNames(typeof(EControlType));
            type[type.Length-1] = "None";
            type[type.Length - 2] = "None";
            _Type.ItemsSource = type;
            _Type.SelectedIndex = 1;
        }
        public void DisableType()
        {
            _Type.Visibility = Visibility.Collapsed;
        }
        public void SetTitle(string title)
        {
            Title = title;
        }
        private void APPLY_Click(object sender, RoutedEventArgs e)
        {
            if (_Name.Text.Replace(" ","") == "" || _Type.SelectedItem == null|| (string)_Type.SelectedItem == "None")
            {
                MessageBox.Show("空名称或者无类型控件是不被允许的");
                e.Handled = true;
                return;
            }
            name = _Name.Text;
            remarks = _Reamrk.Text;
            //控件更新
                switch ((string)_Type.SelectedItem)
                {
                    case "Joy":
                        type = EControlType.Joy;
                        break;
                    case "Button":
                        type = EControlType.Button;
                        break;
                    case "Group":
                        type = EControlType.Group;
                    break;
            }
            DialogResult = true;
            Close();
        }
    }
}
