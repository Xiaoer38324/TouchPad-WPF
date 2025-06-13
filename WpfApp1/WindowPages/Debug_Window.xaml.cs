using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.WindowPages
{
    /// <summary>
    /// Debug_Window.xaml 的交互逻辑
    /// </summary>
    public partial class Debug_Window : Window
    {
        StringBuilder builder;
        static Debug_Window instance;
        private Debug_Window()
        {
            builder= new StringBuilder();
            InitializeComponent();
        }
        public static Debug_Window GetIns()
        {
            if (instance == null)
            {

                instance = new Debug_Window();
                instance.Show();
            }
            return instance;
        }
        public void OutPut(string text)
        {
            builder.AppendLine(text);
            TextCon.Text=builder.ToString();
        }

    }
}
