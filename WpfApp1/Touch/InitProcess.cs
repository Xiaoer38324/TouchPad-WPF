using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;
using WpfApp1;
using WpfApp1.WindowPages;

namespace WpfApp1.Touch
{
    internal class InitProcess
    {
        GameSetting setting;
        public InitProcess(GameSetting setting)
        {
            this.setting = setting;
        }
        internal TouchPad pad;
        int finder_times_magnification = 1, finder_interval_magnification=1;
        private bool Start(GameSetting setting)
        {
            if (setting.launcher)
            {
                finder_times_magnification = 3;
                finder_interval_magnification = 3;
            }
            IntPtr gamewindow = IntPtr.Zero;
            bool fullscreen = false;
            if (!setting.manuallocking)
            {
                if (String.IsNullOrEmpty(setting.gamepath) && File.Exists(setting.gamepath))
                    return false;
                gamewindow = FindWindow();
            }
            else
            {
                ProcessSelect pro_select = new ProcessSelect();
                if (pro_select.ShowDialog() == true)
                {
                    string[] lock_info = setting.lock_message.Split(",");
                    if (lock_info.Length > 1 && !String.IsNullOrEmpty(lock_info[1]))
                    {
                        gamewindow = FindWindowInProcess(lock_info[1],pro_select.result.Id);
                    }
                    else
                    {
                        gamewindow = pro_select.result.MainWindowHandle;
                    }
                    
                }
            }
            if (gamewindow == IntPtr.Zero) return false;
            fullscreen = WindowOperation.IsFullScreen(gamewindow);
            Touch_LoadAndSave save = new Touch_LoadAndSave(setting.layout);
            Touch_BitmapImageCache cache;
            LayoutInfo info;
            if (!save.Load(out cache, out info)) return false;
            Dictionary<string, VirtualKeyCode[]> keys = SaveALoad.LoadByCode<Dictionary<string, VirtualKeyCode[]>>(setting.keymap);
            pad = new TouchPad();
            pad.Init(info, cache, gamewindow, keys, new PadParameter(setting.covermode, setting.attribute, setting.transparency, setting.allowtransparency, fullscreen));
            pad.Show();
            return true;
        }
        internal bool Start()
        {
            return Start(setting);
        }
        //改变了搜索思路。只有4种情况
        /*
         1:直接启动，找到。
         2:通过启动器间接启动，并且启动器本身就是个软件。//这个只能通过用户在其他页面自己设置。
         3：通过exe启动，但是这个exe不是软件，关闭的非常快
         4：就是找不到，得让用户手动锁定
         */
        //又改了，因为异步性，所以难以捕捉推进。所以上面那个方案被抛弃了。
        /*
         现在只有3种。
          1.正常启动
          2.通过exe启动，但是这个exe不是软件，关闭的非常快
          3.直接通过暴力搜索。
        流程：没有修复信息。1检测失败-》2检测失败-》手动锁定。
        流程：有修复信息。1检测成功，判断是否为目标-否》迭代暴力筛选。/
         */
        private IntPtr FindWindow()
        {
            IntPtr gamehandle = IntPtr.Zero;
            Process pro;
            pro = Process.Start(setting.gamepath);
            //第一个数据是ProcessName，第二个是窗口类名
            string[] lock_info = setting.lock_message.Split(",");
            if (lock_info.Length > 1 && !lock_info[1].Equals(""))
            {
                if (pro.WaitForExit(2000)|| !pro.ProcessName.Equals(lock_info[0]))
                {     //找不到转跳，或者是货不对板
                    gamehandle = GetWindowByLoop(lock_info[0], lock_info[1]);
                }
                else
                {
                    InitFinderVar(lock_info[1]);
                    gamehandle = FindWindowInProcess(lock_info[1],pro.Id);
                }
            }
            else
            {
                if (pro.WaitForExit(1000))//1s内退出
                {
                    pro = GetNext(pro.Id);
                    if (pro != null)
                        gamehandle = pro.MainWindowHandle;
                }
                gamehandle = pro.MainWindowHandle;
                if (gamehandle ==IntPtr.Zero)//进入手动选择
                {
                    FixForLockingMessage fixForLockingMessage = new FixForLockingMessage();
                    if (fixForLockingMessage.ShowDialog() == true)
                    {
                        gamehandle = fixForLockingMessage.hwnd;
                    }
                }
            }
            return gamehandle;
        }
        void InitFinderVar(string win_classname)
        {
            window_classname = new StringBuilder();
            callback = delegate (IntPtr ptr, int id)
            {
                window_classname.Clear();
                int size = WindowOperation.GetClassName(ptr, window_classname, 30);
                window_classname.Capacity = size;
                int now_id = 0;
                WindowOperation.GetWindowThreadProcessId(ptr, out now_id);
                if (now_id == id)
                {
                    if (window_classname.ToString().Equals(win_classname))
                    {
                        enum_windowptr = ptr;
                        return false;
                    }

                }
                return true;
            };
            
        }
        IntPtr GetWindowByLoop(string process_name,string win_classname)
        {
            InitFinderVar(win_classname);
            IntPtr result=IntPtr.Zero;
           
            Task finder = new Task(() =>
            {
                int times = -1;
                Process[] processes;
                do
                {
                    processes = Process.GetProcessesByName(process_name);
                    times++;
                    Thread.Sleep(1000 * finder_interval_magnification);
                }
                while (times < 60 * finder_times_magnification && processes.Length <= 0);
                int id = processes[0].Id;
                while (times<60*finder_times_magnification&&result==IntPtr.Zero)
                {
                   result=FindWindowInProcess(win_classname,id);
                   Thread.Sleep(1000*finder_interval_magnification);
                   times++;
                }
            });
            finder.Start();
            finder.Wait();
            return result;
        }
        Process GetNext(int pid)
        {
            PerformanceCounter pc = new PerformanceCounter("Process", "Creating Process Id");
            foreach (Process p in Process.GetProcesses())
            {

                pc.InstanceName = p.ProcessName;
                if (pid == (int)pc.RawValue)
                {
                    return p;
                }
            }
            return null;
        }
        StringBuilder window_classname;
        IntPtr enum_windowptr=IntPtr.Zero;
        EnumWindow_CallBack callback;
        IntPtr FindWindowInProcess(string classname,int process_id)
        {
            enum_windowptr = IntPtr.Zero;
            window_classname.Clear();
            WindowOperation.EnumWindows(callback,process_id);
            return enum_windowptr;
        }
    }
}
