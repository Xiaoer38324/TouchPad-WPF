using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Asset;

namespace WpfApp1
{
    /// <summary>
    /// ResourceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceWindow : Window
    {
        struct NOL
        {
            public bool op;
            public string message;

            public NOL(bool op, string message)
            {
                this.op = op;
                this.message = message;
            }
        }
        public string path;
        List<NOL> infos;
        BitmapImageCache cache;
        AssetManager manager;
        bool mode = false;//true 管理服务，false选择服务
        public ResourceWindow()
        {
            InitializeComponent();
        }
        public void SetImageCache(BitmapImageCache cache)
        {
            this.cache = cache;
        }
        private void Name_Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ResourceList.Items != null)
                ResourceList.Items.Refresh();
        }
        
        public void SetSource(AssetManager assetManager)
        {
            this.manager = assetManager;
            this.infos = new List<NOL>();
            foreach (AssetFileInfo info in assetManager.GetResourceList())
            {
                ResourceList.Items.Add(info);
            }
            ResourceList.Items.Filter = Filter;
            ResourceList.Items.IsLiveFiltering = true;
            }
        private bool Filter(object obj)
        {
            AssetFileInfo info = (AssetFileInfo)obj;
            if (Type_Filter.SelectedValue == null||(string)Type_Filter.SelectedValue==Enum.GetName(AssetFileType.UNDEF) || Enum.GetName(info.type) == (string)Type_Filter.SelectedValue)
            {
                if (info.ident.Contains(Name_Filter.Text) || String.IsNullOrEmpty(Name_Filter.Text))
                {
                    return true;
                }
            }
            return false;
        }
        public void SetMode(bool mode)
        {
            this.mode = mode;
            if (mode) { path = ""; SelectFunction.Visibility = Visibility.Visible; }
            else { ManagerFunction.Visibility = Visibility.Visible;  }
        }
        private void Type_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResourceList.Items != null)
                ResourceList.Items.Refresh();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Type_Filter.ItemsSource = Enum.GetNames(typeof(AssetFileType));
            Type_Filter.SelectedIndex = 0;
        }
        private bool ExitIdent(string indent)
        {
            foreach (AssetFileInfo info in ResourceList.Items.Cast<AssetFileInfo>())
            {
                if (info.ident == indent) return true;
            }
            return false;
        }
        private void AddAsset(string filepath,string ident)
        {
            if (ExitIdent(ident))
            {
                ResourceIdent resourceIdent = new ResourceIdent();
                resourceIdent.Owner = this;
                resourceIdent.SetInfo(filepath, ident);
                resourceIdent.ShowDialog();
                if (resourceIdent.DialogResult == true)
                {
                    AddAsset(filepath,resourceIdent.Name.Text);
                }
            }
            else
            {

                infos.Add(new NOL(true, ident + "||" + filepath));
                ResourceList.Items.Add(new AssetFileInfo(ident, AssetFileType.IMAGE, "待计算"));
            }
        }
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdialog = new OpenFileDialog();
            fdialog.Multiselect = true;

               fdialog.Filter = "图片|*.png;*.jpg;*.jpeg";
           
            fdialog.FilterIndex = 0;
            fdialog.Filter = "";
            if (fdialog.ShowDialog()==true)
            {
               for(int i = 0; i < fdialog.FileNames.Length;i++)
                {
                    AddAsset(fdialog.FileNames[i], fdialog.FileNames[i].Substring(fdialog.FileNames[i].LastIndexOf('\\') + 1));
                }
            }

        } 

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            AssetFileInfo info= (AssetFileInfo)ResourceList.SelectedValue;
            ResourceList.Items.Remove(info);
            infos.Add(new NOL(false, info.ident));
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i=0;i<infos.Count;i++)
            {
                if (infos[i].op) {
                    string[] data = infos[i].message.Split("||");
                    manager.TryAddAsset(data[1], data[0]);
                }
                else
                {
                    manager.Remove(infos[i].message);
                }
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(null,null);
            if (ResourceList.Items.Count== 1)
            {
                path = ((AssetFileInfo)(ResourceList.Items[0])).ident;
                DialogResult = true;
                Close();
            }
            else if(ResourceList.SelectedValue==null)
             {
                MessageBox.Show("你必须选定一个文件");
            }
            else
            {
                path = ((AssetFileInfo)(ResourceList.SelectedValue)).ident;
                DialogResult = true;
                Close();
            }
          
        }
        string oldimage="";
        private void ResourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResourceList.SelectedValue == null) return;
            AssetFileInfo x = (AssetFileInfo)ResourceList.SelectedValue;
            PreView.Source=cache.GetBitmap(x.ident,oldimage);
        }
    }
}
