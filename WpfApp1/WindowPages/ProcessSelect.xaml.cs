using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1

{
    struct ListItme_ImageAText
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Process Handle;

        public ListItme_ImageAText(string name, string path, Process Handle)
        {
            Name = name;
            Path = path;
            this.Handle = Handle;
        }
    }
   
    /// <summary>
    /// ProcessSelect.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessSelect : Window
    {
        public Process result=null;
        public ProcessSelect()
        {
            InitializeComponent();
        }

        private void ProcessList_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.MainWindowHandle != IntPtr.Zero)
                        ProcessList.Items.Add(new ListItme_ImageAText(p.ProcessName, p.MainModule.FileName, p));
                }
                catch (Win32Exception we)
                {

                }
               
            }
            ProcessList.MouseDoubleClick += delegate (object obj, MouseButtonEventArgs e)
            {
                ListBoxItem item;
                if (ListBox_DoubleClick.DoubleClick(ProcessList,out item))
                {
                    result = ((ListItme_ImageAText)item.DataContext).Handle;
                    DialogResult = true;
                    Close();
                }
            };
        }
    }
}
