//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Windows.Forms;
//using GTA;
//
//namespace LogicSpawn.GTARPG.Core
//{
//    public class Test : Script
//    {
//
//        private bool draw; 
//        private IntPtr windowHandle;
//        public Test()
//        {
//            Tick += OnTick;
//            Interval = 0;
//            KeyDown += OnKeyDown;
//
//            Process[] processes = Process.GetProcessesByName("GTA5");
//            windowHandle = new IntPtr();
//            foreach (Process p in processes)
//            {
//                windowHandle = p.MainWindowHandle;
//            }
//
//        }
//
//        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
//        {
//            if(keyEventArgs.KeyCode == Keys.B)
//            {
//                draw = !draw;
//            }
//        }
//
//        private void OnTick(object sender, EventArgs eventArgs)
//        {
//            if (!draw) return;
//
//            Wait(1000);
//            draw = false;
//            var f = new Form();
//            TestForm form = new TestForm();
//            form.Init(windowHandle);
//            new Thread(() => form.ShowDialog()).Start();
//        }
//    }
//
//
//}