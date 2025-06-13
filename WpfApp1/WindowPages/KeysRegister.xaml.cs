using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsInput;
using WpfApp1.Attributes;

namespace WpfApp1.Touch
{
    /// <summary>
    /// KeysRegister.xaml 的交互逻辑
    /// </summary>
    public partial class KeysRegister : Window
    {
        LayoutInfo info;
        List<Control> cons;
        public VirtualKeyCode[][]  keys;
        Dictionary<EControlType, KeyRegisterItem> views;
        EControlType currenttype;
        public KeysRegister()
        {
            cons = new List<Control>();
            views = new Dictionary<EControlType, KeyRegisterItem>();
            InitializeComponent();
        }
        bool Add(Control con,object obj)
        {
            con.Background = Brushes.LightBlue;
            con.MouseDown+= Mouse_Down;
            cons.Add(con);
            Preview.Children.Add(con);
            return true;
        }
        void Mouse_Down(object sender,MouseEventArgs e)
        {
            SelectIndex(cons.IndexOf((Control)sender));
        }
        int currentindex = -1;
        void SelectIndex(int index)
        {
            if (currentindex >= 0)
            {
                if (views[currenttype].IsVailed())
                {

                    cons[currentindex].Background = Brushes.LightGreen;
                    keys[currentindex] = views[currenttype].GetKey();
                }
            }
            currentindex = index;
            SwitchView(((AttributeDataS)info.attributes[index]).type);
            currenttype = ((AttributeDataS)info.attributes[index]).type;
            Con_Name.Text = info.names[currentindex];
        }
        void SwitchView(EControlType newtype)
        {
            if (!views.ContainsKey(newtype))
            {
                views[newtype] = SpawnView(newtype);
                Grid.SetRow(views[newtype].GetPanel(), 1);
                View.Children.Add(views[newtype].GetPanel());
            }
            if(currenttype!=EControlType.None)
            views[currenttype].GetPanel().Visibility = Visibility.Collapsed;
            views[newtype].GetPanel().Visibility = Visibility.Visible;
            views[newtype].Clear();
            if (keys[currentindex]!=null)
            views[newtype].Reset(keys[currentindex]);
        }
        //控件更新
        KeyRegisterItem SpawnView(EControlType type)
        {
            switch (type)
            {
                case EControlType.Button:
                    return  new Button_KeyResiger();
                case EControlType.Joy:
                    return new Joy_KeyResiger();
            }
            return null;
        }

        internal void Init(LayoutInfo info)
        {
            this.info = info;
            keys = new VirtualKeyCode[info.attributes.Count][];
           // Preview.Height = Preview.Width * info.height / info.width;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Touch_Normalization normal = new Touch_Normalization(Add, info, Preview.ActualWidth, Preview.ActualHeight);
            normal.Operation(false,false);

        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("请确保文件符合布局要求");
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "注册文件|*.treg";
            if (open.ShowDialog() == true)
            {
                keys = SaveALoad.Load<VirtualKeyCode[][]>(open.FileName);
                DialogResult = true;
                Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "注册文件|*.treg";
            if (save.ShowDialog() == true)
            {
                if (!SaveALoad.Save(keys, save.FileName)) MessageBox.Show("保存失败");
            }
        }
        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result= MessageBox.Show("保存？", "提示", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                DialogResult = true;
            }
            else if(result==MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
