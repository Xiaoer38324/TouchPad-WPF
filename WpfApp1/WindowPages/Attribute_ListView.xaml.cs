using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Asset;
using WpfApp1.Attributes;
using WpfApp1.Logic.UILogic;
using WpfApp1.Touch;

namespace WpfApp1
{
    /// <summary>
    /// Attribute_ListView.xaml 的交互逻辑
    /// </summary>
    public partial class Attribute_ListView : Window
    {
        ScaleTransform sclae;
        GeneralTransform BoardToUi;
        double scalefactor = 1;
        bool skiponece = false;//用代码设置也会触发，所以跳过一次
        ImageSource background;
        public Attribute_ListView()
        {
            InitializeComponent();

        }
        private void Attr_Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            TransformGroup x = Com_Editor_Ui.GetValue(Control.RenderTransformProperty) as TransformGroup;
            sclae = x.Children[0] as ScaleTransform;
            GridScale.AddHandler(Slider.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(GridScale_ValueChanged));
           //LoadFile(@"E:\data\tptest\xxc.tcom");
           // ActiveUI();
            // Canvas.SetLeft(Com_Editor_Ui,-Com_Editor_Ui_Borders.ActualWidth);
            //  Canvas.SetTop(Com_Editor_Ui, -Com_Editor_Ui_Borders.ActualHeight);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

        }
        private bool LoadFile(string path)
        {
            Editor_GobalVar.GetIns().ClearAll();
            Editor_GobalVar.GetIns().file = new EditorSaveFile(path);
            Editor_GobalVar.GetIns().imagecache = new Editor_BitmapImageCache();
            Editor_GobalVar.GetIns().logic = new EditorUi_Logic();
            Editor_GobalVar.GetIns().logic.SetCanvas(Com_Editor_Ui, Com_Editor_Ui_Borders);
            Editor_GobalVar.GetIns().assetm = new Asset.AssetManager(path.Substring(0,path.LastIndexOf('\\')));
            AttributeManager Manager = new AttributeManager(W_ScrollViewer);
            Editor_GobalVar.GetIns().logic.AddListern(Manager);
            Editor_GobalVar.GetIns().file.AddOperation(Editor_GobalVar.GetIns().assetm);
            Editor_GobalVar.GetIns().file.AddOperation(Manager);
            Editor_GobalVar.GetIns().file.AddOperation(Editor_GobalVar.GetIns().logic);
            if (Editor_GobalVar.GetIns().file.Load() == false)
            {
                MessageBox.Show("加载失败！");
                return false;
            }
            Editor_GobalVar.GetIns().logic.FixUnKnowSetting();
            ReSizeGridWindow();
            Editor_GobalVar.GetIns().logic.ActiveUI();
            CopyCon(null,null);
            background = ((ImageBrush)Com_Editor_Ui.Background).ImageSource;
            TheWindow.AddHandler(Canvas.SizeChangedEvent, new SizeChangedEventHandler(TheWindow_SizeChanged));
            EnableBackGround.IsChecked = (bool)Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.EnableBackground);
            EnableBackGround_Click(null,null);
            return true;
        }
        private void ReSizeGridWindow()
        {
            uint[] size = (uint[])(Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.Size));
            double w = size[0];
            double h = size[1];
            double scale = (w / h);
            h = Com_Editor_Ui_Borders.ActualHeight;
            scalefactor = h * scale / size[0];
            Grid_Window.ColumnDefinitions[0].Width = new GridLength(Com_Editor_Ui_Borders.ActualHeight*scale, GridUnitType.Star);
            Grid_Window.ColumnDefinitions[2].Width = new GridLength(Grid_Window.ActualWidth - (Com_Editor_Ui_Borders.ActualHeight * scale), GridUnitType.Star);
            Com_Editor_Ui.Width = size[0];
            Com_Editor_Ui.Height = size[1];
            Com_Editor_Ui.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Com_Editor_Ui.Arrange(new Rect(0, 0, Com_Editor_Ui.DesiredSize.Width, Com_Editor_Ui.DesiredSize.Height));
            
        }
        private void Save(string path)
        {
            Editor_GobalVar.GetIns().file.Save();
        }
        private void Com_Editor_Ui_Loaded(object sender, RoutedEventArgs e)
        {
            //Com_Editor_Ui.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //Com_Editor_Ui.Arrange(new Rect(0, 0, Com_Editor_Ui.DesiredSize.Width, Com_Editor_Ui.DesiredSize.Height));
           
        }
        private void MenuItem_CreateDef_Click(object sender, RoutedEventArgs e)
        {
            Con_CreateInfo con_CreateInfo = new Con_CreateInfo();
            con_CreateInfo.Owner = this;
            if (con_CreateInfo.ShowDialog() == true)
            {
                Control con = _Ins_Creator.CreateCon(con_CreateInfo.type);
                Editor_GobalVar.GetIns().logic.AddControl(con, new ConBaseInfo(con_CreateInfo.name, con_CreateInfo.remarks, con_CreateInfo.type));
            };
        }
        private void SearchCon(object sender, RoutedEventArgs e)
        {
            Con_Search search = new Con_Search();
            search.Owner = this;
            search.SetSource(Editor_GobalVar.GetIns().logic.GetConInfo());
           
            search.ShowDialog();
        }
        private void DeleteCon (object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (!Editor_GobalVar.GetIns().logic.DeleteActived()) MessageBox.Show("当前没有选中控件", "警告");
            }
        }
        private void CopyCon(object sender, RoutedEventArgs e)
        {
            Com_Editor_Ui.SetValue(Canvas.LeftProperty, 0.0);
            Com_Editor_Ui.SetValue(Canvas.TopProperty, 0.0);
            sclae.ScaleX = 1*scalefactor;
            sclae.ScaleY = 1 * scalefactor;
            skiponece = true;
            GridScale.Value = 1;
        }
        private void ChangeConInfo(object sender, RoutedEventArgs e)
        {
            ConBaseInfo info = Editor_GobalVar.GetIns().logic.GetActiveInfo().conBaseInfo;
            if (info.type == EControlType.None) return;
            Con_CreateInfo con_ChangeInfo = new Con_CreateInfo();
            con_ChangeInfo.Owner = this;
            con_ChangeInfo.SetTitle("修改控件标识");
            con_ChangeInfo.DisableType();
            con_ChangeInfo._Name.Text = Editor_GobalVar.GetIns().logic.GetActiveInfo().conBaseInfo.name;
            con_ChangeInfo._Reamrk.Text=Editor_GobalVar.GetIns().logic.GetActiveInfo().conBaseInfo.remarks;
            if (con_ChangeInfo.ShowDialog() == true)
            {
                Editor_GobalVar.GetIns().logic.SetActivedName(con_ChangeInfo.name);
            }

        }
        private void MenuItem_ResourceManager_Click(object sender, RoutedEventArgs e)
        {
            ResourceWindow w = new ResourceWindow();
            w.Owner = this;
            w.SetImageCache(Editor_GobalVar.GetIns().imagecache);
            w.SetSource(Editor_GobalVar.GetIns().assetm);
            w.SetMode(false);
            w.ShowDialog();
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Com_Editor_Setting setting = (Com_Editor_Setting)(Editor_GobalVar.GetIns().logic.GetSetting(EdiorSetting.ALL));
            uint[] oldvalute = new uint[] { setting.size[0], setting.size[1] };
            setting.size = oldvalute;
            Editor_Setting settingwindow = new Editor_Setting();
            settingwindow.SetSetingData(setting);
            settingwindow.Owner = this;
            settingwindow.ShowDialog();
            Editor_GobalVar.GetIns().logic.SetSetting(EdiorSetting.ALL, settingwindow.com_Editor_Setting);
            if (settingwindow.sizechange)
            {
                ReSizeGridWindow();
                CopyCon(null,null);
            }
                

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (Editor_GobalVar.GetIns().file != null)
            {
                TransformGroup x = Com_Editor_Ui.GetValue(Control.RenderTransformProperty) as TransformGroup;
                ScaleTransform sclae = x.Children[0] as ScaleTransform;
                Editor_GobalVar.GetIns().logic.SetSetting(EdiorSetting.LayoutPos, new double[] { (double)Com_Editor_Ui.GetValue(Canvas.LeftProperty), (double)Com_Editor_Ui.GetValue(Canvas.TopProperty) });
                Editor_GobalVar.GetIns().logic.SetSetting(EdiorSetting.Layoutscale, new double[] { sclae.ScaleX, sclae.ScaleY});
                if (Editor_GobalVar.GetIns().file.Save() == false) MessageBox.Show("保存失败");
            }
                
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdialog = new OpenFileDialog();
            fdialog.Filter = "布局文件|*.tcom;";
            if (fdialog.ShowDialog()==true)
            {
                
                if (LoadFile(fdialog.FileName))
                    ActiveUI();
                else
                    Close();
            }
        }
        private void ActiveUI()
        {
            Function.IsEnabled = true;
            File_Output.Visibility = Visibility.Visible;
            File_Input.Visibility = Visibility.Visible;
            File_Save.Visibility = Visibility.Visible;
            File_Setting.Visibility = Visibility.Visible;
            Resource.Visibility = Visibility.Visible;
            GridFunction.IsEnabled = true;
            File_Open.Visibility = Visibility.Collapsed;
        }
        private void DisableUI()
        {
            Function.IsEnabled = false;
            File_Output.Visibility = Visibility.Collapsed;
            File_Input.Visibility = Visibility.Collapsed;
            File_Save.Visibility = Visibility.Collapsed;
            File_Setting.Visibility = Visibility.Collapsed;
            Resource.Visibility = Visibility.Collapsed;
            GridFunction.IsEnabled = false;
        }
        public void StartWithSelection()
        {
            //Open_Click(null,null);
            
            this.ShowDialog();
        }
        Point point;
        double x = 0, y = 0;
        private void TheWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scalefactor *= e.NewSize.Width / e.PreviousSize.Width;
        }
        private void GridScale_MouseEnter(object sender, MouseEventArgs e)
        {
            point = new Point((double)Com_Editor_Ui.GetValue(Canvas.LeftProperty), (double)Com_Editor_Ui.GetValue(Canvas.TopProperty));
            Rect xp = sclae.TransformBounds(new Rect(0, 0, 1, 1));
            point.X += xp.X;
            point.Y += xp.Y;
            x = -point.X + Com_Editor_Ui_Borders.ActualWidth / 2;
            y = -point.Y + Com_Editor_Ui_Borders.ActualHeight / 2;
        }
        private void EnableBackGround_Click(object sender, RoutedEventArgs e)
        {
            Editor_GobalVar.GetIns().logic.SetSetting(EdiorSetting.EnableBackground, EnableBackGround.IsChecked);
            if (EnableBackGround.IsChecked == true)
            {
                ((ImageBrush)(Com_Editor_Ui.Background)).ImageSource = background;
            }
            else
            {
                background = ((ImageBrush)(Com_Editor_Ui.Background)).ImageSource;
                ((ImageBrush)(Com_Editor_Ui.Background)).ImageSource = null;
            }
        }
        private void Con_SelectionBox_Click(object sender, RoutedEventArgs e)
        {
            Editor_GobalVar.GetIns().logic.StartBoxSelect(true);
        }
        private void Ins_SelectionBox_Click(object sender, RoutedEventArgs e)
        {

            Editor_GobalVar.GetIns().logic.StartBoxSelect(false);
        }
        private void Sele_SelectionBox_Click(object sender, RoutedEventArgs e)
        {

            Editor_GobalVar.GetIns().logic.StartSeleSelect();
        }
        private void File_Input_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdialog = new OpenFileDialog();
            fdialog.Filter = "组件文件|*.tcon,*.tcom;";
            if (fdialog.ShowDialog() == true)
            {
                if (Editor_GobalVar.GetIns().file.Push(fdialog.FileName) == false)
                {
                    MessageBox.Show("加载错误");
                };
              
            }
        }
        //添加控件更新
        private void File_Output_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sdialog = new SaveFileDialog();
            sdialog.Filter = "布局文件|*.tlay;";
            if (sdialog.ShowDialog() == true)
            {
                AssetManager manager = Editor_GobalVar.GetIns().assetm;
                Save_Click(null, null);
                string root= sdialog.FileName.Substring(0, sdialog.FileName.LastIndexOf('\\')+1);
                Directory.CreateDirectory(root+"temp");
                string[] used = AssetManager.GetUsedResource(Editor_GobalVar.GetIns().file.control_data);
                string id;
                Dictionary<string, string> idtomd5 = new Dictionary<string, string>();
                for(int i = 0; i < used.Length; i++)
                {
                    id = manager.GetMd5ByIdentification(used[i]);
                    File.Copy(root + "Asset\\" +id, root + "temp\\" + id);
                    idtomd5.Add(used[i],id);
                }
                List<object> copy_data = new List<object>();
                List<string> name = new List<string>();
                var all = Editor_GobalVar.GetIns().file.LoadWithNoCall();
                var temp= all.control_data;
                Type type;
                foreach(uint i in temp.Keys)
                {
                    type = temp[i].GetType();
                    if (type == typeof(Button_Attribute))
                    {
                        Button_Attribute attr = ((Button_Attribute)temp[i]);
                        if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.background)))
                            attr.background = idtomd5[attr.background];
                        if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.mask)))
                            attr.mask = idtomd5[attr.mask];
                    }
                    else if (type == typeof(Joy_Attribute))
                    {
                        Joy_Attribute attr = ((Joy_Attribute)temp[i]);
                        if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.background)))
                            attr.background = idtomd5[attr.background];
                        if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.gmask)))
                            attr.gmask = idtomd5[attr.gmask];
                        if (String.IsNullOrEmpty(AssetManager.IsSystemAsset(attr.gsource)))
                            attr.gsource = idtomd5[attr.gsource];
                    }
                    copy_data.Add(temp[i]);
                    name.Add(all.control_info.Find(o=>{ return o.con_id == i; }).name);
                }
                LayoutInfo info = new LayoutInfo()
                {
                    attributes = copy_data,
                    width = all.setting.size[0],
                    height = all.setting.size[1],
                    names = name
            };
                SaveALoad.Save(info, root + "temp\\layout");
                id = sdialog.FileName.Substring(sdialog.FileName.LastIndexOf('\\') + 1);
                if (!id.EndsWith(".tlay")) id +=".tlay";
                if(File.Exists(root + id))
                    File.Delete(root + id);
                ZipFile.CreateFromDirectory(root+"temp",root+id);
                sdialog = null;
                Directory.Delete(root + "temp",true);
                GC.Collect();
            }
        }
        private void GridScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (skiponece) { skiponece = false; return; }
            //int xx=(int)(Com_Editor_Ui_Borders.ActualWidth / 2 * (e.OldValue - e.NewValue));
            //int yy = (int)(Com_Editor_Ui_Borders.ActualHeight / 2 * (e.OldValue - e.NewValue));
            //double x =xx;
            //double y =yy;
            //Com_Editor_Ui.SetValue(Canvas.LeftProperty,(double)Com_Editor_Ui.GetValue(Canvas.LeftProperty)+x);
            //Com_Editor_Ui.SetValue(Canvas.TopProperty, (double)Com_Editor_Ui.GetValue(Canvas.TopProperty) + y);
            point = new Point((double)Com_Editor_Ui.GetValue(Canvas.LeftProperty), (double)Com_Editor_Ui.GetValue(Canvas.TopProperty));
            //Rect xp = xx.TransformBounds(new Rect(0, 0, 1, 1));
            //point.X += xp.X;
            //point.Y += xp.Y;
            Com_Editor_Ui.SetValue(Canvas.LeftProperty, point.X + x * (e.OldValue - e.NewValue));
            Com_Editor_Ui.SetValue(Canvas.TopProperty, point.Y + y * (e.OldValue - e.NewValue));
            sclae.ScaleX = e.NewValue * scalefactor;
            sclae.ScaleY = e.NewValue * scalefactor;
        }
    }
}
