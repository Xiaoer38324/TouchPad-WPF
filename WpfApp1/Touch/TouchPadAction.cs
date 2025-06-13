using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using static WpfApp1.WindowOperation;

namespace WpfApp1.Touch
{
    internal class TouchPadAction
    {  
        WinEventDelegate move,active= null;
        IntPtr game= IntPtr.Zero, touch = IntPtr.Zero;
        IntPtr movehook, activehook;
        WindowHideAndShow window;
        Thread moving;
        RECT r = default;
        RECT pos = default;
        bool fullscreen = false;
        bool continemoving = false;
        public Action sizechange_end;
        public TouchPadAction(IntPtr game,IntPtr touch,WindowHideAndShow window,bool fullscreen=false)
        {
            this.game = game;
            this.touch = touch;
            this.window = window;
            this.fullscreen = fullscreen;
            int pid;
            GetWindowThreadProcessId(game, out pid);
            move = new WinEventDelegate(Move);
            movehook = SetWinEventHook(0x000A, 0x000B, IntPtr.Zero, move, (uint)pid, 0, 0);
            if (GetWindowRect(game, ref pos))
            {
                if (GetClientRect(game, ref r))
                {
                    MoveWindow(touch, pos.Left, pos.Bottom - r.Bottom, r.Right - r.Left, (r.Bottom - r.Top));
                }
            }
            active = new WinEventDelegate(Active);
            activehook = SetWinEventHook(0x0003, 0x0003, IntPtr.Zero, active, 0, 0, 0);
        }
        void MovingWindows()
        {
            while (continemoving)
            {
                if (GetWindowRect(game, ref pos))
                {
                    // MoveWindow(xxc, pos.Left, pos.Top ,pos.Right - pos.Left, ((int)(1 * (pos.Bottom - pos.Top))));

                    if (GetClientRect(game, ref r))
                    {
                        MoveWindow(touch, pos.Left, pos.Bottom - r.Bottom, r.Right - r.Left, (r.Bottom - r.Top));

                    }
                }
                Thread.Sleep(100);
            }
        }
        void StartMove()
        {
            if ( moving == null||!moving.IsAlive)
            {
                
                moving = new Thread(MovingWindows);
                moving.Start();
            }
            continemoving = true;
        }
        void EndMove()
        {
            continemoving = false;
        }
        void SetTopMost()
        {
            RECT r=default;
            Thread.Sleep(3000);
            SetWindowPos(touch, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            GetWindowRect(game, ref r);
            MoveWindow(touch, 0, 0, r.Right - r.Left, r.Bottom - r.Top);
        }
        void Active(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hWinEventHook == activehook)
            {
                IntPtr now = GetForegroundWindow();
                if (now != game && now != touch)
                {
                    window.DoHide();
                }
                else
                {
                    window.DoShow();
                    SetForegroundWindow(game);
                    if (fullscreen)
                    {
                        var thread = new Thread(SetTopMost);
                        thread.Start();
                        }
                    }
                }
            }
        void Close(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            
        }
        void Move(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {

           
            if (hWinEventHook == movehook)
            {
                if (eventType == 10)
                {
                    StartMove();
                }
                else
                {
                    EndMove();
                    sizechange_end?.Invoke();
                }
            }
        }
        internal void SetGameWindow(IntPtr game, bool fullscreen)
        {
            if (game != IntPtr.Zero)
            {
                this.game = game;
            }
            if (this.fullscreen == fullscreen) return;
            else
            {
                if (fullscreen)
                {
                    UnhookWinEvent(movehook);
                    this.fullscreen = fullscreen;
                }
                else
                {
                    int pid;
                    GetWindowThreadProcessId(game, out pid);
                    move = new WinEventDelegate(Move);
                    movehook = SetWinEventHook(0x000A, 0x000B, IntPtr.Zero, move, (uint)pid, 0, 0);

                }
            }
        }
        internal void Close()
        {
            UnhookWinEvent(activehook);
            if (!fullscreen)
                UnhookWinEvent(movehook);
        }
        
    }
}
