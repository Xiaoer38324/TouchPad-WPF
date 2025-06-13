using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Resources;
using System.Windows.Shapes;
using WindowsInput;
using WpfApp1;
using WpfApp1.Asset;
using WpfApp1.Logic.UILogic;
using WpfApp1.Properties;
using WpfApp1.Touch;

namespace WpfApp1.WindowPages
{
    struct GameInfo
    {
        public GameInfo(string path, string name, GameSetting gameSetting)
        {
            Path = path;
            Name = name;
            this.gameSetting=gameSetting;
        }
        public GameSetting gameSetting;
        public string Path { get; set; }
        public string Name { get; set;}
    }
    /// <summary>
    /// MainWin.xaml 的交互逻辑
    /// </summary>
    public partial class MainWin : Window
    {
        public MainWin()
        {
            InitializeComponent();
        }
        void GetGameSettingFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "应用程序|*.exe";
            dialog.Title = "选择游戏";
           
                if (dialog.ShowDialog() == true)
                {

                    OpenFileDialog gdialog = new OpenFileDialog();
                    gdialog.Filter = "游戏配置|*.tgame";
                    if (gdialog.ShowDialog() == true)
                    {
                        string dirpath = SoftWare_GlobalVar.GetIns().setting.datapath + dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                        if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
                        else
                        {
                            int i = 0;
                            string newdir = dirpath;
                            do
                            {
                                newdir = dirpath;
                                newdir += i;
                                i++;
                            }
                            while (Directory.Exists(newdir));
                            dirpath = newdir;
                        }
                        System.IO.Compression.ZipFile.ExtractToDirectory(gdialog.FileName, dirpath);
                      GameSetting  setting = SaveALoad.Load<GameSetting>(dirpath + "\\gamesetting");
                        setting.layout = dirpath + "\\layout";
                        setting.gamepath = dialog.FileName;
                        SaveALoad.Save(setting, dirpath + "\\gamesetting");
                    }
                   
                
            };
           
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("读取配置？\n是：从文件读取一个配置\n否：创建新配置", "选择", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes) {
                GetGameSettingFromFile();
            }
            else if (result == MessageBoxResult.No)
            {
                GameRegister gameRegister = new GameRegister();
                if (gameRegister.ShowDialog() == true)
                {
                    GameSetting setting = gameRegister.GetSetting();
                    string name = setting.gamepath.Substring(setting.gamepath.LastIndexOf('\\') + 1).Replace(".exe", "");
                    int i = 0;
                    string newdir = datapath + "\\" + name;
                    do
                    {
                        newdir = datapath + "\\" + name;
                        newdir += i;
                        i++;
                    } while (Directory.Exists(newdir));
                    name = newdir;
                    if (!Directory.Exists(name)) Directory.CreateDirectory(name);
                    File.Copy(setting.layout, name + "\\layout");
                    setting.layout = name + "\\layout";
                    SaveALoad.Save(setting, name + "\\gamesetting");
                }
            }

            FlashList();
        }
        InitProcess iprocess=null;
        private void StartACloseGame(object sender, MouseButtonEventArgs e)
        {
            if (iprocess != null)
            {
                iprocess.pad.DoClose();
                startgame.Content = "启动游戏";
                iprocess = null;
            }
            else if (gamelist.SelectedIndex >= 0)
            {
                GameSetting setting = ((GameInfo)gamelist.SelectedValue).gameSetting;
                iprocess = new InitProcess(setting);
                bool success=iprocess.Start();
                if (!success)
                {
                    iprocess = null;
                    MessageBox.Show("启动失败咯，有以下情况:\n1:用户自己取消。\n2:找不到游戏窗口,请尝试以下方法。\n2.1进行搜索修复。\n2.2搜索数据有误，请尝试清空搜索修复。\n2.3设置为手动锁定模式。");
                    
                }
                else
                {
                    startgame.Content = "关闭布局";
                }
            }
        }
        string datapath;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string currentdir = System.IO.Directory.GetCurrentDirectory();
            if (!Directory.Exists(currentdir + "\\cache\\")) Directory.CreateDirectory(currentdir + "\\cache\\");
            if (!Directory.Exists(currentdir + "\\games\\")) Directory.CreateDirectory(currentdir + "\\games\\");
            SoftWare_GlobalVar.GetIns().PutSetting(new SoftWare_Setting() { cache=currentdir+"\\cache\\",datapath=currentdir+"\\games\\"});
            datapath = SoftWare_GlobalVar.GetIns().setting.datapath;
            gamelist.SelectionChanged += delegate (object sender, SelectionChangedEventArgs e)
            {
                if(gamelist.SelectedValue!=null)
                Describe.Text = ((GameInfo)gamelist.SelectedValue).gameSetting.helper;
             
            };
            FlashList();

        }

        private void FlashList()
        {
            gamelist.Items.Clear();
            string[] games = Directory.GetDirectories(datapath);
            GameSetting setting;
            for (int i = 0; i < games.Length; i++)
            {
                setting = SaveALoad.Load<GameSetting>(games[i]+"\\gamesetting");
                if(!setting.gamepath.Equals(""))
                    gamelist.Items.Add(new GameInfo(setting.gamepath,games[i].Substring(games[i].LastIndexOf('\\')+1),setting));
                else
                    gamelist.Items.Add(new GameInfo("", games[i].Substring(games[i].LastIndexOf('\\') + 1), setting));
            }
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("创建工程？\n是：创建工程\n否：打开工程","选择",MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
            
                dialog.Filter = "布局文件|*.tcom"; 
                if (dialog.ShowDialog() == true)
                {
                    Uri uri = new Uri(@"/Resource/layoutfile.tcom", UriKind.Relative);//这个就是所以的pack uri。

                    StreamResourceInfo info = Application.GetResourceStream(uri);
                    Stream s = info.Stream;
                    byte[] buffer = new byte[308];
                    s.Read(buffer, 0, 308);
                    if (!dialog.FileName.EndsWith(".tcom")) dialog.FileName += ".tcom";
                    File.WriteAllBytes(dialog.FileName,buffer);
                   // File.Copy(@"pack://application:,,,/Resource/layoutfile.tcom", dialog.FileName);
                }
            }
            else if (result==MessageBoxResult.No)
            {
                Attribute_ListView editor = new Attribute_ListView();
                editor.ShowDialog();

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (iprocess != null) iprocess.pad.Close();
        }
      
        private void File_Manager_Click(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            File_Window file_Window = new File_Window();
            file_Window.Start(gamelist.Items);
            this.Show();
            gamelist.Items.Clear();
            foreach (var x in file_Window.SettingList.Items)
            {
                gamelist.Items.Add(x);
            }
        }

        private void Editor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Dictionary<uint, object> control_data=new Dictionary<uint, object>();
            //List<ConBaseInfo> control_info=new List<ConBaseInfo>();
            //Com_Editor_Setting setting2=new Com_Editor_Setting();
            //List<AssetFileInfo> resource=new List<AssetFileInfo>();
            //AllEditiorDataFileStruct all = new AllEditiorDataFileStruct(setting2, resource, control_data, control_info);
            //SaveALoad.Save(all, @"C:\Users\zeng\Desktop\CC.txt");
            this.Hide();
            Attribute_ListView editor_Window = new Attribute_ListView();
            editor_Window.StartWithSelection();
            this.Show();
        }
    }
}
