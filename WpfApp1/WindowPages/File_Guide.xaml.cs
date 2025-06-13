using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsInput;
using WpfApp1.Attributes;
using WpfApp1.Touch;

namespace WpfApp1.WindowPages
{
    public enum Guide_Command
    {
        ReGame,ReKeyMap,ReCheck,Guide
    }
    /// <summary>
    /// File_Guide.xaml 的交互逻辑
    /// </summary>
    public partial class File_Guide : Window
    {
        struct StageUiALogic{
           internal Guide_Stage stage;
           internal Panel ui;

            public StageUiALogic(Guide_Stage stage, Panel ui)
            {
                this.stage = stage;
                this.ui = ui;
            }
        }
        public File_Guide()
        {
            InitializeComponent();
           
        }
        internal GameSetting gameSetting;
        public string file_name = "";
        public string file_oldname = null;
        List<StageUiALogic> Stages;
        Dictionary<int, Func<object,bool>> PostProcess;
        int stage = -1;
        bool autosave = true;//设置为false，将不会检查文件的合理性，也不会进行转换，也不会自动保存。ui所填即所得。
        internal void Start(Guide_Command command,string name,GameSetting setting=default,bool autosave=true)
        {
            Stages = new List<StageUiALogic>();
          PostProcess=new Dictionary<int, Func<object, bool>>();
            this.gameSetting = setting;
            file_name = name;
            file_oldname = file_name;
           this.autosave = autosave;
            switch (command)
            {
                case Guide_Command.ReKeyMap:
                    Previous.Visibility = Visibility.Hidden;
                    Next.Content = "完成";
                    Stages.Add(new StageUiALogic(new Guide_KeyMap(new Control[] {KeyMap_Name},new Panel[] {KeyCanvas,KeyMapArea}),Mapping));
                    break;
                case Guide_Command.ReGame:
                    HelpMessage.IsEnabled = false;
                    Previous.Visibility = Visibility.Hidden;
                    Next.Content = "完成";
                    Stages.Add(new StageUiALogic(new Guide_GameAHelp(new Control[] {GameName,GamePath,HelpMessage,manual_locking,SelectGame},file_name),SelectGameAToDo));
                    PostProcess.Add(0, delegate (object name)
                    {
                        file_name = name as string;
                        if(Directory.Exists(SoftWare_GlobalVar.GetIns().setting.datapath + file_name)&&!file_name.Equals(file_oldname))
                        {
                            MessageBox.Show("名称冲突，请重新命名。");
                            return false;
                        }
                        else if (!VailedFolderName(file_name))
                        {

                            MessageBox.Show("非法字符，请重新命名。\n/ \\ : * \" < > | ？");
                            return false;
                        }
                        return true;
                    });
                    break;
                case Guide_Command.ReCheck:
                    Previous.Visibility = Visibility.Hidden;
                    Next.Content = "完成";
                    Stages.Add(new StageUiALogic(new Guide_Checks(new Control[] { allow_attribute,allow_transarency,cover_mode,half_screen,LayoutFile,KeyMapFile,ImportFile,launcher },null),LayoutAndMapping));
                    break;
                case Guide_Command.Guide:
                    Stages.Add(new StageUiALogic(new Guide_GameAHelp(new Control[] { GameName, GamePath, HelpMessage,manual_locking, SelectGame }, file_name), SelectGameAToDo));
                    Stages.Add(new StageUiALogic(new Guide_Checks(new Control[] { allow_attribute, allow_transarency, cover_mode, half_screen, LayoutFile, KeyMapFile, ImportFile, launcher }, null), LayoutAndMapping));
                    Stages.Add(new StageUiALogic(new Guide_KeyMap(new Control[] { KeyMap_Name }, new Panel[] { KeyCanvas, KeyMapArea }), Mapping));
                    PostProcess.Add(0, delegate (object name)
                    {
                        file_name = name as string;
                        if (Directory.Exists(SoftWare_GlobalVar.GetIns().setting.datapath + file_name) && !file_name.Equals(file_oldname))
                        {
                            MessageBox.Show("名称冲突，请重新命名。");
                            return false;
                        }
                        return true;
                    });
                    break;
            }
            Next.Click += delegate (object obj, RoutedEventArgs e)
            {
                if(!NextPage()) return;
                if (stage>=Stages.Count)
                {
                    try
                    {
                        //最后一个布局的处理，放这里了。不再
                        if (!autosave) { DialogResult = true; Close(); return; }
                        //完成处理，检验+文件处理。
                        if (CheckVailable(gameSetting) && File.Exists(gameSetting.layout))
                        {
                            
                            string filepath = SoftWare_GlobalVar.GetIns().setting.datapath + file_name;
                            if (String.IsNullOrEmpty(file_oldname))//说明是新建的引导，肯定是没有这个名字
                            {
                                Directory.CreateDirectory(filepath);
                            }
                            else if (!file_oldname.Equals(file_name))//说明改名字了
                            {
                                Directory.Move(SoftWare_GlobalVar.GetIns().setting.datapath + file_oldname, filepath);//重命名！
                            }
                            filepath = filepath + "\\layout";
                            if (!File.Exists(filepath))
                            {
                                File.Copy(gameSetting.layout, filepath);
                                gameSetting.layout = filepath;
                            }
                            filepath = SoftWare_GlobalVar.GetIns().setting.datapath + file_name + "\\gamesetting";
                            if (File.Exists(filepath))
                                File.Delete(filepath);
                            SaveALoad.Save(gameSetting, filepath);
                            DialogResult = true;
                           
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("核验数据错误，请确保相关文件依然存在并且软件有权限读写磁盘");
                        DialogResult = false;
                    }
                    Close();

                }
            };
            Previous.Click += delegate (object obj, RoutedEventArgs e)
            {
                if (stage != 0)
                    PrePage();
            };
            //刚开始是什么都没有为-1.所以Next以后要把上一步重新隐藏
            
            Previous.Visibility = Visibility.Collapsed;
            ShowDialog();
        }
        bool CheckVailable(GameSetting setting)
        {
            if (!String.IsNullOrEmpty(setting.layout) && !String.IsNullOrEmpty(setting.keymap))
            {
                if (!setting.manuallocking)
                {
                    if (File.Exists(setting.gamepath))
                        return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        bool NextPage()
        {
           
            if (stage < Stages.Count&&stage>=0)
            {
                object obj;
                Stages[stage].stage.Close(ref gameSetting,out obj);
                if (PostProcess.ContainsKey(stage))
                {
                    if (PostProcess[stage](obj)==false) {
                        return false;
                    };
                }
            }
            if(stage == Stages.Count - 1) { stage++;return true; }
            if (stage + 1 < Stages.Count)
            {
                JumpTo(++stage);
                Stages[stage].stage.Start(gameSetting);
            }
            if (stage == Stages.Count - 1) Next.Content = "完成";
            Previous.Visibility=Visibility.Visible;
            return true;
        }
        void PrePage()
        {
            if (stage >= 0)
            {
                object obj;
                Stages[stage].stage.Close(ref gameSetting, out obj);
                if (PostProcess.ContainsKey(stage))
                {
                    PostProcess[stage]?.Invoke(obj);
                }
            }
            JumpTo(--stage);
            Next.Content = "下一步";
            if (stage == 0) Previous.Visibility = Visibility.Collapsed;
        }
        void JumpTo(int index)
        {
            SelectGameAToDo.Visibility = Visibility.Collapsed;
            SelectGameAToDo.IsEnabled = false;
            LayoutAndMapping.Visibility = Visibility.Collapsed;
            LayoutAndMapping.IsEnabled = false;
            Mapping.Visibility = Visibility.Collapsed;
            Mapping.IsEnabled = false;
            Stages[index].ui.Visibility = Visibility.Visible;
            Stages[index].ui.IsEnabled = true;
           
        }
        EventHandler rendering;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SelectGame.Click += delegate (object sender, RoutedEventArgs e)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "游戏程序|*.exe";
                if (open.ShowDialog()==true)
                {
                    GamePath.Text= open.FileName;
                }
            };
            if (Stages.Count >= 0)
            {

                KeyCanvas.Visibility = Visibility.Visible;
                NextPage();
                if (Stages.Count == 1)
                {
                    Previous.Visibility = Visibility.Hidden;
                }
            }
        }
        private bool VailedFolderName(string name)
        {
            return !(name.Contains('/') || name.Contains('\\') || name.Contains(':') || name.Contains('*') || name.Contains('\"') || name.Contains('<') || name.Contains('>') || name.Contains('|') || name.Contains('?'));
        }
    }
    //。。。。。。。。。。。。。。。。。。基类
    abstract class Guide_Stage
    {
        protected GameSetting gameSetting;
        public abstract void  Start(GameSetting setting);//主要在这里填写数据
        public  abstract bool Close(ref GameSetting setting,out object obj);//关闭窗口，把数据送出来。
        public Guide_Stage(Control[] controls, object par) {
        
        }
    }
    //具体阶段的逻辑类。
    class Guide_GameAHelp : Guide_Stage
    {
        TextBox GameName,GamePath,HelpMessage;
        CheckBox manual_locking;
        Button SelectGame;
        public Guide_GameAHelp(Control[] controls, object par) : base(controls, par)
        {
            if (controls == null)
            {
                throw new ArgumentNullException("wrong control");
            }
            GameName = controls[0] as TextBox;
            GamePath = controls[1] as TextBox;
            HelpMessage = controls[2] as TextBox;
            manual_locking = controls[3] as CheckBox;
            SelectGame = controls[4] as Button;
            manual_locking.Click += delegate (object sender, RoutedEventArgs e)
            {
                if (manual_locking.IsChecked==true)
                {
                    GamePath.IsEnabled = false;
                    SelectGame.IsEnabled = false;
                }
                else
                {
                    GamePath.IsEnabled = true;
                    SelectGame.IsEnabled = true;
                }
            };
            GameName.Text = par as string;
        }

        public override bool Close(ref GameSetting setting, out object obj)
        {
            gameSetting.helper = HelpMessage.Text;
            if (!gameSetting.manuallocking)
            {
                if (File.Exists(GamePath.Text))
                {
                    gameSetting.gamepath = GamePath.Text;
                    setting = gameSetting;
                    obj = GameName.Text;
                    return true;
                }
                else
                {
                    MessageBox.Show("游戏程序路径无效");
                    obj = null;
                    return false;
                }
            }
            obj = GameName.Text;
            return true;
        }

        public override void Start(GameSetting setting)
        {
           
           this.gameSetting= setting;
            manual_locking.IsChecked = gameSetting.manuallocking;
            GamePath.Text = gameSetting.gamepath;
            HelpMessage.Text = gameSetting.helper;
        }
    }
    class Guide_Checks : Guide_Stage
    {
        CheckBox allow_attribute, allow_transarency, cover_mode, half_screen, launcher;
        TextBox layout_file, keymap_file;
        string tgame_path = null;
        Button import;

        public Guide_Checks(Control[] controls, object par) : base(controls, par)
        {
            allow_attribute = controls[0] as CheckBox;
            allow_transarency = controls[1] as CheckBox;
            cover_mode = controls[2] as CheckBox;
            half_screen = controls[3] as CheckBox;
            layout_file = controls[4] as TextBox;
            keymap_file = controls[5] as TextBox;
            import = controls[6] as Button;
            launcher = controls[7] as CheckBox;
            import.Click += delegate (object sender, RoutedEventArgs e)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "按键文件|*.tkey|布局文件|*.tlay;";
                if (open.ShowDialog() == true)
                {
                    if (open.FileName.EndsWith("tkey"))
                    {
                        keymap_file.Text = gameSetting.keymap = File.ReadAllText(open.FileName);
                    }
                    else if (open.FileName.EndsWith("tlay"))
                    {
                        layout_file.Text = open.FileName;
                    }
                }
            };
        }

        public override bool Close(ref GameSetting setting, out object obj)
        {
            gameSetting.attribute = (allow_attribute.IsChecked == true ? true : false);
            gameSetting.allowtransparency = (allow_transarency.IsChecked == true ? true : false);
            gameSetting.covermode = (cover_mode.IsChecked == true ? true : false);
            gameSetting.halfscreenmode = (half_screen.IsChecked == true ? true : false);
            gameSetting.launcher = (launcher.IsChecked == true ? true:false);
            if (gameSetting.allowtransparency || gameSetting.covermode)
            {
                gameSetting.transparency = true;
            }
            gameSetting.layout = layout_file.Text;
            gameSetting.keymap = keymap_file.Text;
            setting = gameSetting;
            obj = null;
            return true;
        }
        public override void Start(GameSetting setting)
        {
            this.gameSetting = setting;
           
            allow_attribute.IsChecked = gameSetting.attribute;
            allow_transarency.IsChecked= gameSetting.allowtransparency;
            cover_mode.IsChecked = gameSetting.covermode;
            half_screen.IsChecked = gameSetting.halfscreenmode;
            layout_file.Text = gameSetting.layout ;
            keymap_file.Text=gameSetting.keymap;
            if (cover_mode.IsChecked == true)
            {
                allow_transarency.Visibility = Visibility.Visible;
            }
            else
                allow_transarency.Visibility = Visibility.Collapsed;
            cover_mode.Checked += delegate (object sender, RoutedEventArgs e)
            {
                if (cover_mode.IsChecked == true)
                {
                    allow_transarency.Visibility = Visibility.Visible;
                }
                else
                    allow_transarency.Visibility = Visibility.Collapsed;
            };

          
        }
    }
    class Guide_KeyMap : Guide_Stage
    {
        Dictionary<Control, string> contostr;
        Dictionary<string, VirtualKeyCode[]> keys;
        Dictionary<EControlType, KeyRegisterItem> views;
        EControlType now_type;
        string now_name;
        Canvas KeyCanvas;
        Grid KeyMapArea;
        GroupBox KeyMap_Name;
        int control_difference = 0;
        bool check_difference=false;//用于开启差别检测，检测映射是否正常。
        public Guide_KeyMap(Control[] controls, object par) : base(controls, par)
        {
            Panel[] panel = (Panel[])par;
            KeyCanvas = panel[0] as Canvas;
            KeyMapArea = panel[1] as Grid;
            KeyMap_Name = controls[0] as GroupBox;
        }

        void Control_Click(object obj, MouseButtonEventArgs e)
        {
            if(now_type!=EControlType.None)
            views[now_type].GetPanel().Visibility=Visibility.Collapsed;
            if(!String.IsNullOrEmpty(now_name))
            keys[now_name] = views[now_type].GetKey();
            now_name = contostr[(Control)obj];
            now_type = ((TouchActivator)obj).GetECT();
            if (!views.ContainsKey(now_type))
            {
                views.Add(now_type,SpawnView(now_type));
                KeyMapArea.Children.Add(views[now_type].GetPanel());
            }
            views[now_type].GetPanel().Visibility = Visibility.Visible;
            views[now_type].Clear();
            if (keys[now_name] != null)
                views[now_type].Reset(keys[now_name]);
            KeyMap_Name.Header = now_name;
        }
        bool Add(Control con, object obj)
        {
            string name = (string)obj;
            con.MouseDown += Control_Click;
            if (check_difference && !keys.ContainsKey((name)))
            {
                control_difference++;
            }
            if(!keys.ContainsKey(name))
            keys.Add(name, null);
            contostr.Add(con, name);
            KeyCanvas.Children.Add(con);  
            return true;
        }
        public override bool Close(ref GameSetting setting, out object obj)
        {
            if (now_type != EControlType.None)
                views[now_type].GetPanel().Visibility = Visibility.Collapsed;
            if (!String.IsNullOrEmpty(now_name))
                keys[now_name] = views[now_type].GetKey();
            gameSetting.keymap = SaveALoad.CovertoJson(keys);
            setting= gameSetting;
            obj = null;
            return true;
        }
        //控件更新
        KeyRegisterItem SpawnView(EControlType type)
        {
            switch (type)
            {
                case EControlType.Button:
                    return new Button_KeyResiger();
                case EControlType.Joy:
                    return new Joy_KeyResiger();
            }
            return null;
        }
        private void Start()
        {
           
        }
        public override void Start(GameSetting setting)
        {
            views = new Dictionary<EControlType, KeyRegisterItem>();
            contostr = new Dictionary<Control, string>();
            if (!setting.keymap.Equals(""))
            {
                check_difference = true;
            }
            if (setting.keymap.Equals(""))
                keys = new Dictionary<string, VirtualKeyCode[]>();
            else
            {
                keys = SaveALoad.LoadByCode<Dictionary<string, VirtualKeyCode[]>>(setting.keymap);
                if (keys == null)
                {
                    MessageBox.Show("映射文件检测有错误，或许该返回上一步检查");
                    return;
                }
            }
            this.gameSetting = setting;
            int mappingnumber = keys.Count;
            LayoutInfo info;
            Touch_BitmapImageCache cache;
            if (!File.Exists(gameSetting.layout)) return;
            new Touch_LoadAndSave(gameSetting.layout).Load(out cache, out info);
            Editor_GobalVar.GetIns().imagecache = cache;
            if (info.names == null)
            {
                MessageBox.Show("布局文件检测有错误，或许该返回上一步检查");
                return;
            }
            //这里没办法直接获取到实际的高度和宽度，所以直接直接用resize更新。
            Touch_Normalization normal = new Touch_Normalization(Add, info, KeyCanvas.ActualWidth, KeyCanvas.ActualHeight);
            normal.Operation(true, true);
            KeyCanvas.SizeChanged += delegate (object sender, SizeChangedEventArgs e)
            {
                normal.ReSize(KeyCanvas, e.NewSize.Width, e.NewSize.Height);
            };
            if (check_difference)
            {
                MessageBox.Show("布局文件一共有:" + info.names.Count + "个控件。\n映射文件一共有有" + mappingnumber + "个文件。\n有" + control_difference + "个控件没有匹配上。");
            }
        }
    }
}
