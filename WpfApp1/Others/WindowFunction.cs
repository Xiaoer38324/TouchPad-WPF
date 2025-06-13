using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class WindowFunction
    {
        IntPtr lock_window=IntPtr.Zero;
        Task lock_task=null;
        ManualResetEvent resetEvent;
        CancellationToken token;
        CancellationTokenSource tokenSource;
        public  WindowFunction()
        {
             tokenSource = new CancellationTokenSource();
             token = tokenSource.Token;
            lock_task = new Task(Locking,token);
            resetEvent = new ManualResetEvent(true);
            
            
        }
        void Locking()
        {  
            while (!token.IsCancellationRequested) 
            {
                resetEvent.WaitOne();
                if (lock_window == IntPtr.Zero) break;
                WindowOperation.SetTop(lock_window);
                Task.Delay(3000);

            }
            
        }
        public bool IsForeground(IntPtr window)
        {
            return window == lock_window;
        }
        public void StopForeground()
        {
            KeepForeground(IntPtr.Zero);
        }
        public void KeepForeground(IntPtr window)
        {
            if (window == lock_window) return;
            else if (lock_task.Status== TaskStatus.Running) { resetEvent.Reset(); }
            lock_window = window;
            if (window != IntPtr.Zero)
            {
                resetEvent.Set();
                if (lock_task.Status == TaskStatus.Created) lock_task.Start();
            }
            else {
                tokenSource.Cancel();
                lock_task.Wait();
                lock_task.Dispose();
                 tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                lock_task = new Task(Locking, token);
            }
        }
        ~WindowFunction() {
            if (lock_window!=IntPtr.Zero) {
                StopForeground();
            }
        }
    }
}
