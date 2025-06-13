using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WpfApp1
{
    public delegate bool EnumWindow_CallBack(IntPtr hwm, int id);
    struct WindowOperation_Par
    {
        public bool notitle;
        public bool settop;
        public bool noactive;
        public void ReturnStyle(ref int style)
        {
            if (notitle) style |= 0x80;
            if (noactive) style |= 0x08000000;
        }
    }
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    internal static class WindowOperation
    {

        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int HWND_TOPMOST = -1;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int GWL_EXSTYLE = -20;
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return Environment.Is64BitProcess
                ? GetWindowLong64(hWnd, nIndex)
                : GetWindowLong32(hWnd, nIndex);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return Environment.Is64BitProcess
                ? SetWindowLong64(hWnd, nIndex, dwNewLong)
                : SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool refreash = true);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int nMaxCount);
        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(out POINT lpPoint);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, ref RECT lpRect);
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr h);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindow_CallBack callback,int process_id);
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //调用下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT Point);
        [DllImport("user32")]
        public static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport("User32.DLL")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        public const uint OCR_NORMAL = 32512;

        [DllImport("User32.DLL")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        public static string GetClassNameEEX(IntPtr intPtr, int MaxCount = 30)
        {
            StringBuilder className = new StringBuilder();
            int count = GetClassName(intPtr, className, MaxCount);
            if (count > MaxCount)
            {
                MaxCount = count;
                className.Clear();
                count = GetClassName(intPtr, className, MaxCount);
            }
            className.Capacity = count;
            return className.ToString();
        }
        public static string GetWindowTitleEEX(IntPtr intPtr)
        {
            StringBuilder title = new StringBuilder();
            int MaxCount = GetWindowTextLength(intPtr);
            GetWindowText(intPtr, title, MaxCount);
            return title.ToString();
        }
        public static bool IsFullScreen(IntPtr window)
        {
            RECT r = default;
            bool full = false;
            GetWindowRect(GetForegroundWindow(), ref r);
            if (r.Left != 0 || r.Top != 0)
            {
                full = false;
            }
            else
            {
                int w = GetSystemMetrics(0);
                int h = GetSystemMetrics(1);
                full = ((r.Right - r.Left) == w && (r.Bottom - r.Top) == h);
            }
            return full;
        }
        public static void SetWindow(WindowOperation_Par _Par, IntPtr target)
        {
            var exstyle = GetWindowLong(target, GWL_EXSTYLE);
            int target_style = exstyle.ToInt32();
            _Par.ReturnStyle(ref target_style);
            SetWindowLong(target, GWL_EXSTYLE, new IntPtr(target_style));
            if (_Par.settop)
                SetWindowPos(target, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
        public static void AddWindowStyle(int style, IntPtr target)
        {
            var exstyle = GetWindowLong(target, GWL_EXSTYLE);

            SetWindowLong(target, GWL_EXSTYLE, new IntPtr(style));
        }
        public static void SetWindowStyle(int style, IntPtr target)
        {
            SetWindowLong(target, GWL_EXSTYLE, new IntPtr(style));
        }
        public static IntPtr GetWindowStyle(IntPtr target)
        {
            return GetWindowLong(target, GWL_EXSTYLE);


        }
        public static void SetTop(IntPtr target)
        {
            SetWindowPos(target, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
    }
}