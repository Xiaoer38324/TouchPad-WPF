using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp1.Logic.UILogic;

namespace WpfApp1
{
    /// <summary>
    /// Editor_Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Editor_Setting : Window
    {
       internal Com_Editor_Setting com_Editor_Setting;
        private string backgroundpath="";
        public Editor_Setting()
        {
            InitializeComponent();
            GridNumber.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(delegate (object sender, TextCompositionEventArgs e) {
                Regex re = new Regex("[^0-9]+");
                e.Handled = re.IsMatch(e.Text);
            }));
            SizeWidth.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(delegate (object sender, TextCompositionEventArgs e) {
                Regex re = new Regex("[^0-9]+");
                e.Handled = re.IsMatch(e.Text);
            }));
            SizeHeight.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(delegate (object sender, TextCompositionEventArgs e) {
                Regex re = new Regex("[^0-9]+");
                e.Handled = re.IsMatch(e.Text);
            }));
        }
        public bool sizechange=false;
        internal void SetSetingData(Com_Editor_Setting set)
        {

            com_Editor_Setting = set;
            Enableattach.IsChecked=com_Editor_Setting.enableattach;
            GridNumber.Text = com_Editor_Setting.griddestiny.ToString();
            SizeWidth.Text = com_Editor_Setting.size[0].ToString();
            SizeHeight.Text = com_Editor_Setting.size[1].ToString();
            backgroundpath = com_Editor_Setting.background;

        }
        private bool CheackDifference()
        {
            if (com_Editor_Setting.enableattach != Enableattach.IsChecked || com_Editor_Setting.griddestiny != int.Parse(GridNumber.Text) || com_Editor_Setting.background != backgroundpath)
            {
                return true;
            }
            else if (com_Editor_Setting.size[0].ToString() != SizeWidth.Text || com_Editor_Setting.size[1].ToString() != SizeHeight.Text)
            {
                sizechange = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Save()
        {
            if (Enableattach.IsChecked == true)
                com_Editor_Setting.enableattach = true;
            else

                com_Editor_Setting.enableattach = false;
            com_Editor_Setting.griddestiny = int.Parse(GridNumber.Text);
            com_Editor_Setting.size[0] = uint.Parse(SizeWidth.Text);
            com_Editor_Setting.size[1] = uint.Parse(SizeHeight.Text);
            com_Editor_Setting.background = backgroundpath;
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(CheackDifference())
            Save();
            Close();
        }
        private bool OpenResourceSelection(out string path)
        {
            ResourceWindow window = new ResourceWindow();
            window.SetImageCache(Editor_GobalVar.GetIns().imagecache);
            window.SetSource(Editor_GobalVar.GetIns().assetm);
            window.SetMode(true);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                path = window.path;
                return true;
            }
            path = "";
            return false;
        }
        private void BackGround_Click(object sender, RoutedEventArgs e)
        {
            OpenResourceSelection(out backgroundpath);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CheackDifference())
            {

                if(MessageBox.Show("已修改设置，是否保存", "警告", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    Save();
                }
            }
        }
    }
}
