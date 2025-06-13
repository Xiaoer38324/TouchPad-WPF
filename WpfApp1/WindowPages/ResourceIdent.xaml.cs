using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// ResourceIdent.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceIdent : Window
    {
        public string name;
        public ResourceIdent()
        {
            InitializeComponent();
        }
        public void SetInfo(string imagepath,string name)
        {
            Name.Text = name;
            PREVIEW.Source = new BitmapImage(new Uri(imagepath));
        }
        private void YES_Click(object sender, RoutedEventArgs e)
        {
            if(Name.Text.Contains("||")) { MessageBox.Show("不能包含||非法字符");return; }
            name = Name.Text;
            DialogResult = true;
            Close();
        }
    }
}
