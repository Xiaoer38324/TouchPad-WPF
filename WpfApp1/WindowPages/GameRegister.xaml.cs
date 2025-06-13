using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsInput;
using WpfApp1.Touch;
namespace WpfApp1
{
    internal struct GameSetting
    {
       public string gamepath;
        public string helper;
        public string layout;
        public string keymap;
        public bool allowtransparency= false;
        public bool covermode=true;
        public bool halfscreenmode=false;
        public bool transparency=true ;
        public bool attribute=true;
        public bool manuallocking=false;
        //第一个数据是ProcessName，第二个是窗口类名以逗号区分。
        public string lock_message;
        public bool launcher=false;
        public GameSetting()
        {
            gamepath = "";
            helper = "";
            layout = "";
            keymap = "";
            lock_message = ",";
        }
    }
    /// <summary>
    /// GameRegister.xaml 的交互逻辑
    /// </summary>
    public partial class GameRegister : Window
    {
        GameSetting setting;
        GameSetting cover_hight,cover_mid,cover_low;
        public GameRegister()
        {

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cover_hight = new GameSetting() {
                allowtransparency = true,
                covermode = true,
                halfscreenmode = false,
                transparency = true,
                attribute = true,
                manuallocking = false
            };
            cover_mid = new GameSetting()
            {
                allowtransparency = false,
                covermode = true,
                halfscreenmode = false,
                transparency = true,
                attribute = true,
                manuallocking = false
            };
            cover_low = new GameSetting()
            {
                allowtransparency = false,
                covermode = true,
                halfscreenmode = false,
                transparency = true,
                attribute = false,
                manuallocking = false
            };
            setting = new GameSetting();
            presuppose.ItemsSource = new string[] {"自定义","质量-覆盖", "均衡-覆盖","性能-覆盖" };
            presuppose.SelectedIndex = 2;
            presuppose.SelectionChanged += SetPreSuppose;
            SetPreSuppose(null,null);
        }
        void SetPreSuppose(object sender, SelectionChangedEventArgs e) {
            string gamepath, helper, keymap, layout;
            gamepath = setting.gamepath;
            helper = setting.helper;
            keymap = setting.keymap;
            layout = setting.layout;
            switch (presuppose.SelectedIndex)
            {
                case 1:
                    setting = cover_hight;
                    setting.gamepath=gamepath;
                    setting.helper=helper;
                    setting.keymap=keymap;
                    setting.layout=layout;
                    FlashCheckBox(setting);
                    break;
                case 2:
                    setting = cover_mid;
                    setting.gamepath = gamepath;
                    setting.helper = helper;
                    setting.keymap = keymap;
                    setting.layout = layout;
                    FlashCheckBox(setting);
                    break;
                case 3:
                    setting = cover_low;
                    setting.gamepath = gamepath;
                    setting.helper = helper;
                    setting.keymap = keymap;
                    setting.layout = layout;
                    FlashCheckBox(setting);
                    break;
            }
        }
        void SetGameSetting(GameSetting setting)
        {
            this.setting = setting;
            FlashCheckBox(setting);
        }
        private void cover_mode_Checked(object sender, RoutedEventArgs e)
        {
            if (cover_mode.IsChecked == true)
            {
                if (transparency.IsChecked != true)
                    transparency.IsChecked = true;
            }
            presuppose.SelectedIndex = 0;
        }

        private void transparency_Checked(object sender, RoutedEventArgs e)
        {
            if (transparency.IsChecked == false)
            {
                if (cover_mode.IsChecked == true)
                    cover_mode.IsChecked = false;
            }
            presuppose.SelectedIndex = 0;
        }

        private void allow_transarency_Checked(object sender, RoutedEventArgs e)
        {
            presuppose.SelectedIndex = 0;
            if (transparency.IsChecked != true)
                transparency.IsChecked = true;
        }

        private void allow_attribute_Checked(object sender, RoutedEventArgs e)
        {
            presuppose.SelectedIndex = 0;
        }

        private void manual_locking_Checked(object sender, RoutedEventArgs e)
        {
            if (manual_locking.IsChecked==true)
            {
                GameInfo.IsEnabled = false;
               
            }
            else
            {
                GameInfo.IsEnabled = true;
            }
            presuppose.SelectedIndex = 0;
        }

        private void half_screen_Checked(object sender, RoutedEventArgs e)
        {
            presuppose.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "应用程序|*.exe";
            if (dialog.ShowDialog() == true)
            {
                game_path.Text= dialog.FileName;
            };
        }

        private void layout_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "布局文件|*.tlay";
            if (dialog.ShowDialog() == true)
            {
                setting.layout = dialog.FileName;
            };
            FlashCheckBox(setting);
        }
        string GetJson()
        {
            KeysRegister keysRegister = new KeysRegister();

            LayoutInfo info;
            Touch_BitmapImageCache cache;
            new Touch_LoadAndSave(setting.layout).Load(out cache, out info);
            keysRegister.Init(info);
            keysRegister.ShowDialog();
            if (keysRegister.DialogResult == true)
            {
                VirtualKeyCode[][] code = keysRegister.keys;
                if (code == null)
                {
                    MessageBox.Show("出现错误", "提示");

                }
                else
                {
                    return SaveALoad.CovertoJson(code);
                }
            }
            return null;
        }

        private void map_file_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result= MessageBox.Show("是否依据布局编辑输入方式？\n是：通过文本输入。\n否：通过文件输入", "", MessageBoxButton.YesNoCancel);
            if (result==MessageBoxResult.Yes&&!String.IsNullOrEmpty(setting.layout))
            {
                string json = GetJson();
              if (json != null)
                {
                    setting.keymap = json;
                }
            }
            else if(result==MessageBoxResult.No)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "映射文件|*.treg";
                if (dialog.ShowDialog() == true)
                {
                    setting.keymap = File.ReadAllText(dialog.FileName);
                };
            }
            FlashCheckBox(setting);
        }


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "游戏配置文件|*.tgame";
            if (dialog.ShowDialog()==true)
            {
                string cachepath = SoftWare_GlobalVar.GetIns().setting.cache + dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\')+1);
                if (!Directory.Exists(cachepath)) Directory.CreateDirectory(cachepath);
                GameSetting saved = GetSetting();
                saved.gamepath = "";
                if (File.Exists(saved.layout))
                {
                    File.Copy(saved.layout, cachepath + "\\layout");
                }
                else
                {
                    MessageBox.Show("未能找到布局文件，保存取消","提示");
                    return;
                }
               
                if (!Directory.Exists(cachepath)) Directory.CreateDirectory(cachepath);
                SaveALoad.Save(saved, cachepath+"\\gamesetting");
                if (!dialog.FileName.EndsWith("tgame")) dialog.FileName += ".tgame";
                ZipFile.CreateFromDirectory(cachepath, dialog.FileName);
                Directory.Delete(cachepath,true);
            }
            
        }
       internal bool CheckVailable(GameSetting setting)
        {
            if (!String.IsNullOrEmpty(setting.layout) &&!String.IsNullOrEmpty(setting.keymap))
            {
                if (!setting.manuallocking){
                    if(File.Exists(setting.gamepath))
                    return true;
                }
                else
                {
                        return true;
                }
            }
            return false;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CheckVailable(GetSetting()))
            {
             MessageBoxResult result=MessageBox.Show("您似乎没有设置游戏路径/布局/映射(其中一个或多个)。该配置无法使用。\n是:依然退出\n否:继续编辑","警告",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) {
                    e.Cancel = true;
                    DialogResult = false;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("保存？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DialogResult = true;
                }
                else
                    DialogResult = false;
            }
        }

        void FlashCheckBox(GameSetting setting)
        {
            cover_mode.IsChecked = setting.covermode;
            allow_attribute.IsChecked = setting.attribute;
            transparency.IsChecked = setting.transparency;
            if (transparency.IsChecked==true&&setting.allowtransparency) {
                allow_transarency.IsChecked = true;
            }
            else{
                allow_transarency.IsChecked = false;
            }
            if (!setting.covermode && setting.halfscreenmode)
            {
                half_screen.IsChecked = true;
            }
            else
            {
                half_screen.IsChecked = false;
            }
            if (!String.IsNullOrEmpty(setting.gamepath)) game_path.Text = setting.gamepath;
            if (!String.IsNullOrEmpty(setting.helper)) helper.Text = setting.helper;
            if (!String.IsNullOrEmpty(setting.layout)) layout_file.Foreground = Brushes.Green;
            else layout_file.Foreground = Brushes.Red;
            if (!String.IsNullOrEmpty(setting.keymap)) map_file.Foreground = Brushes.Green;
            else map_file.Foreground = Brushes.Red;
        }
        internal GameSetting GetSetting()
        {
            GameSetting setting = this.setting;
            setting.transparency = transparency.IsChecked == true ? true : false;
            if(setting.transparency)
                setting.allowtransparency = allow_transarency.IsChecked == true ? true : false;
            setting.covermode = cover_mode.IsChecked == true ? true : false;
            if(!setting.covermode)
                setting.halfscreenmode = half_screen.IsChecked == true ? true : false;
            setting.attribute = allow_attribute.IsChecked == true ? true : false;
            setting.gamepath = game_path.Text;
            setting.helper = helper.Text;
            setting.manuallocking= manual_locking.IsChecked == true ? true : false;
            return setting;
        }
    }
}
