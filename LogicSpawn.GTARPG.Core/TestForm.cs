//using System;
//using System.Drawing;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;
//
//namespace LogicSpawn.GTARPG.Core
//{
//    public class TestForm : Form
//    {
//        [DllImport("user32.dll")]
//        static extern IntPtr GetForegroundWindow();
//
//        [DllImport("user32.dll")]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
//
//        [StructLayout(LayoutKind.Sequential)]
//        public struct RECT
//        {
//            public int Left;        // x position of upper-left corner
//            public int Top;         // y position of upper-left corner
//            public int Right;       // x position of lower-right corner
//            public int Bottom;      // y position of lower-right corner
//        }
//
//        Rectangle myRect = new Rectangle();
//
//        private IntPtr GTAHandle;
//        private IntPtr CurHandle;
//
//        private Color _transparencyCol = Color.LimeGreen;
//
//        public TestForm()
//        {
//            MouseEnter += OnMouseEnter;
//            MouseLeave += OnMouseLeave;
//            Cursor.Hide();
//
//        }
//
//        private void OnMouseLeave(object sender, EventArgs eventArgs)
//        {
//            this.Cursor = Cursors.WaitCursor;
//            Cursor.Show();
//        }
//
//        private void OnMouseEnter(object sender, EventArgs eventArgs)
//        {
//            this.Cursor = null;
//            Cursor.Hide();
//        }
//
//        // This example creates a PictureBox control on the form and draws to it. 
//        // This example assumes that the Form_Load event handler method is 
//        // connected to the Load event of the form. 
//        private PictureBox pictureBox1 = new PictureBox();
//
//        protected override void OnLoad(EventArgs e)
//        {
//            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
//            this.BackColor = _transparencyCol;
//            this.TransparencyKey = _transparencyCol;
//        }
//        public void Init(IntPtr handle)
//        {
//            this.FormBorderStyle = FormBorderStyle.None; 
//
//            Text = "RPGMod UI";
//            GTAHandle = handle;
//
//            RECT rct;
//
//            if (!GetWindowRect(new HandleRef(this, GTAHandle), out rct))
//            {
//                MessageBox.Show("ERROR");
//                return;
//            }
//
//            var l = rct.Left;
//            var to = rct.Top;
//            var w = rct.Right - rct.Left + 1;
//            var h = rct.Bottom - rct.Top + 1;
//
//            Location = new Point(l, to);
//            Size = new Size(w,h);
//
//            this.Parent = Control.FromHandle(handle);
//            // Dock the PictureBox to the form and set its background to white.
//            pictureBox1.Dock = DockStyle.Fill;
//            pictureBox1.BackColor = _transparencyCol;
//            // Connect the Paint event of the PictureBox to the event handler method.
//            pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
//
//            var t = new System.Timers.Timer {Interval = 500};
//            t.Elapsed += TOnTick;
//            t.Start();
//
//            // Add the PictureBox control to the Form. 
//            this.Controls.Add(pictureBox1);
//        }
//
//        private void TOnTick(object sender, EventArgs eventArgs)
//        {
//            CurHandle = GetForegroundWindow();
//            if (CurHandle == GTAHandle)
//            {
//                TopMost = true;
//            }
//            else
//            {
//                TopMost = false;
//            }
//
//            RECT rct;
//
//            if (!GetWindowRect(new HandleRef(this, GTAHandle), out rct))
//            {
//                MessageBox.Show("ERROR");
//                return;
//            }
//
//            var l = rct.Left;
//            var to = rct.Top;
//            var w = rct.Right - rct.Left + 1;
//            var h = rct.Bottom - rct.Top + 1;
//
//            Location = new Point(l, to);
//            Size = new Size(w, h);
//        }
//        
//        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
//        {
//            // Create a local version of the graphics object for the PictureBox.
//            Graphics g = e.Graphics;
//
//            // Draw a string on the PictureBox.
//            //g.DrawString(CurHandle + "  " + GTAHandle,new Font("Arial", 10), System.Drawing.Brushes.White, new Point(30, 30));
//            // Draw a line in the PictureBox.
//            //g.DrawLine(System.Drawing.Pens.Red, pictureBox1.Left, pictureBox1.Top,pictureBox1.Right, pictureBox1.Bottom);
//
//            RPG.UIHandler.DrawUI(g);
//        }
////
////
////
////        public enum GWL
////        {
////            ExStyle = -20
////        }
////
////        public enum WS_EX
////        {
////            Transparent = 0x20,
////            Layered = 0x80000
////        }
////
////        public enum LWA
////        {
////            ColorKey = 0x1,
////            Alpha = 0x2
////        }
////
////        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
////        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);
////
////        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
////        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);
////
////        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
////        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);
////
////        protected override void OnShown(EventArgs e)
////        {
////            base.OnShown(e);
////            int wl = GetWindowLong(this.Handle, GWL.ExStyle);
////            wl = wl | 0x80000 | 0x20;
////            SetWindowLong(this.Handle, GWL.ExStyle, wl);
////            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA.Alpha);
////        }
//
//        protected override void OnPaintBackground(PaintEventArgs e)
//        {
//            e.Graphics.FillRectangle(Brushes.LimeGreen, e.ClipRectangle);
//        }
//    }
//}