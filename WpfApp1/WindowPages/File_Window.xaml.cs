using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
    /// File_Window.xaml 的交互逻辑
    /// </summary>
    public partial class File_Window : Window
    {
        public File_Window()
        {
            InitializeComponent();
        }
        internal void Start(ItemCollection collection)
        {
            SettingList.SelectionChanged += delegate (object sender, SelectionChangedEventArgs e)
            {
                if(SettingList.SelectedItem != null) {

                    GameSetting setting = ((GameInfo)SettingList.SelectedValue).gameSetting;
                    Description.Text = GetSettingDescription(setting);
                }
                else
                {
                    Description.Text = "";
                }
            };
            foreach (var x in collection)
            {
                SettingList.Items.Add(x);
            }
            ShowDialog();
        }
        string GetSettingDescription(GameSetting setting)
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine("游戏路径:"+setting.gamepath);
            description.AppendLine("覆盖模式:"+setting.covermode);
            description.AppendLine("启用属性:" + setting.attribute);
            description.AppendLine("点击穿透:" + setting.allowtransparency);
            description.AppendLine("手动锁定:"+setting.manuallocking);
            string[] lock_info = setting.lock_message.Split(',');
            description.AppendLine("进程辅助信息:" + (!String.IsNullOrEmpty(lock_info[0])? lock_info[0]:"无"));
            description.AppendLine("窗口辅助信息:" + (!String.IsNullOrEmpty(lock_info[1]) ? lock_info[1] : "无"));
            description.AppendLine("备注:" + setting.helper);
            return description.ToString();
        }

        private void ItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要删除吗？", "警告",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
              string infodir=SoftWare_GlobalVar.GetIns().setting.datapath+((GameInfo)SettingList.SelectedValue).Name;
                if (Directory.Exists(infodir))
                {
                    Directory.Delete(infodir, true);
                }
                else
                {
                    MessageBox.Show("实际文件已不存在","提示");
                }
                SettingList.Items.Remove(SettingList.SelectedValue);
            }
           
        }

        private void ItemEditor_KeyMaping_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            File_Guide file_Guide = new File_Guide();
            file_Guide.Start(Guide_Command.ReKeyMap, ((GameInfo)SettingList.SelectedValue).Name, ((GameInfo)SettingList.SelectedValue).gameSetting);
            int select = SettingList.SelectedIndex;
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            SettingList.SelectedIndex = -1;
            info.gameSetting = file_Guide.gameSetting;
            SettingList.Items[select] = info;
            this.ShowDialog();
        }
        private void ItemEditor_GameInfo_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            File_Guide file_Guide = new File_Guide();
            file_Guide.Start(Guide_Command.ReGame, ((GameInfo)SettingList.SelectedValue).Name, ((GameInfo)SettingList.SelectedValue).gameSetting);
            int select=SettingList.SelectedIndex;
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            SettingList.SelectedIndex = -1;
            info.Name = file_Guide.file_name;
            info.gameSetting = file_Guide.gameSetting;
            info.Path = file_Guide.gameSetting.gamepath;
            SettingList.Items[select] = info;
            this.ShowDialog();
        }
        private void ItemEditor_Disposition_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            File_Guide file_Guide = new File_Guide();
            file_Guide.Start(Guide_Command.ReCheck, ((GameInfo)SettingList.SelectedValue).Name, ((GameInfo)SettingList.SelectedValue).gameSetting);
            int select = SettingList.SelectedIndex;
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            SettingList.SelectedIndex = -1;
            info.gameSetting = file_Guide.gameSetting;
            SettingList.Items[select] = info;
            this.ShowDialog();
        }

        private void Setting_Import(object sender, RoutedEventArgs e)
        {
            this.Hide();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "配置文件|*.tgame";
            if (open.ShowDialog() == true)
            {
                File_Guide file_Guide = new File_Guide();
                GameSetting nameApath = new GameSetting();//仅仅用于储存用户输入的游戏与名字信息
                file_Guide.Start(Guide_Command.ReGame,"",nameApath,false);
                if (file_Guide.DialogResult == true)
                {
                    nameApath = file_Guide.gameSetting;
                    string setting_fold = SoftWare_GlobalVar.GetIns().setting.datapath + file_Guide.file_name;
                    if (!Directory.Exists(setting_fold))
                        Directory.CreateDirectory(setting_fold);
                    ZipFile.ExtractToDirectory(open.FileName, setting_fold);
                    GameSetting setting = SaveALoad.Load<GameSetting>(setting_fold + "\\gamesetting");
                    setting.gamepath = nameApath.gamepath;
                    setting.layout = SoftWare_GlobalVar.GetIns().setting.datapath + file_Guide.file_name + "\\layout";
                    setting.manuallocking = nameApath.manuallocking;
                    SaveALoad.Save(setting, setting_fold + "\\gamesetting");
                    GameInfo gameinfo=new GameInfo(nameApath.gamepath,file_Guide.file_name,setting);
                    SettingList.Items.Add(gameinfo);
                }
               
            }
            this.ShowDialog();
        }

        private void DoGuide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            File_Guide file_Guide = new File_Guide();
            file_Guide.Start(Guide_Command.Guide,"",new GameSetting());
            if(file_Guide.DialogResult==true)
            SettingList.Items.Add(new GameInfo(file_Guide.gameSetting.gamepath,file_Guide.file_name,file_Guide.gameSetting));
            this.ShowDialog();
        }

        private void Fix_Luncher(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ProcessSelect select_process=new ProcessSelect();
            if (select_process.ShowDialog() == true)
            {
                int select = SettingList.SelectedIndex;
                GameInfo info = ((GameInfo)SettingList.SelectedValue);
                SettingList.SelectedIndex = -1;
                string[] lock_info = info.gameSetting.lock_message.Split(",");
                lock_info[0] = select_process.result.ProcessName;
                StringBuilder new_lock_info = new StringBuilder();
                for (int i = 0; i < lock_info.Length; i++)
                {
                    new_lock_info.Append(lock_info[i]);
                    new_lock_info.Append(",");
                }
                new_lock_info.Remove(new_lock_info.Length-1, 1);
                info.gameSetting.lock_message = new_lock_info.ToString();
                SettingList.Items[select] = info;
                SaveALoad.Save(info.gameSetting,SoftWare_GlobalVar.GetIns().setting.datapath+info.Name+"\\gamesetting");
            };
            this.ShowDialog();
        }
        private void Fix_NotFind(object sender, RoutedEventArgs e)
        {
            this.Hide();
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            if(!String.IsNullOrEmpty(info.Path))
            Process.Start(info.Path);
            FixForLockingMessage fixForLockingMessage = new FixForLockingMessage();
            if (fixForLockingMessage.ShowDialog()==true) {
                int select = SettingList.SelectedIndex;
                SettingList.SelectedIndex = -1;
                int id=0;
                WindowOperation.GetWindowThreadProcessId(fixForLockingMessage.hwnd,out id);
                info.gameSetting.lock_message = Process.GetProcessById(id).ProcessName+"," + fixForLockingMessage.classname;
                SettingList.Items[select] = info;
                SaveALoad.Save(info.gameSetting, SoftWare_GlobalVar.GetIns().setting.datapath + info.Name + "\\gamesetting");
            }
            this.ShowDialog();

        }
        private void Setting_Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save=new SaveFileDialog();
            save.Filter = "配置文件|*.tgame";
            if (save.ShowDialog() == true)
            {
                GameInfo info = ((GameInfo)SettingList.SelectedValue);
                ZipFile.CreateFromDirectory(SoftWare_GlobalVar.GetIns().setting.datapath+info.Name,save.FileName);
            }
        }
        private void Clear_NotFind(object sender, RoutedEventArgs e)
        {
            int select = SettingList.SelectedIndex;
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            SettingList.SelectedIndex = -1;
            string[] lock_info = info.gameSetting.lock_message.Split(",");
            lock_info[1] = "";
            StringBuilder new_lock_info = new StringBuilder();
            for (int i = 0; i < lock_info.Length; i++)
            {
                new_lock_info.Append(lock_info[i]);
                new_lock_info.Append(",");
            }
            new_lock_info.Remove(new_lock_info.Length - 1, 1);
            info.gameSetting.lock_message = new_lock_info.ToString();
            SettingList.Items[select] = info;
            SaveALoad.Save(info.gameSetting, SoftWare_GlobalVar.GetIns().setting.datapath + info.Name + "\\gamesetting");
        }
        private void Clear_Lunch(object sender, RoutedEventArgs e)
        {
            int select = SettingList.SelectedIndex;
            GameInfo info = ((GameInfo)SettingList.SelectedValue);
            SettingList.SelectedIndex = -1;
            info.gameSetting.lock_message = ",";
            SettingList.Items[select] = info;
            SaveALoad.Save(info.gameSetting, SoftWare_GlobalVar.GetIns().setting.datapath + info.Name + "\\gamesetting");
        }
    }
}
