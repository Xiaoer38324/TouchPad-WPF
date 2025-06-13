using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// TextInput.xaml 的交互逻辑
    /// </summary>
    public partial class TextInput : Window
    {
        public TextInput()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
