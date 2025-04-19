using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public partial class Form1 : Form
    {
        const int HOTKEY_ID_COPY = 1;
        const int HOTKEY_ID_PASTE = 2;
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private string persistentClipboard = "";
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, HOTKEY_ID_COPY, MOD_CONTROL | MOD_SHIFT, (int)Keys.C);
            RegisterHotKey(this.Handle, HOTKEY_ID_PASTE, MOD_CONTROL | MOD_SHIFT, (int)Keys.V);

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Persistent Clipboard";
            trayIcon.Icon = new Icon("icon-transparent.ico"); 

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                if (id == HOTKEY_ID_COPY)
                {
                    SendKeys.SendWait("^c");
                    System.Threading.Thread.Sleep(150);

                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            persistentClipboard = Clipboard.GetText();
                            File.WriteAllText("clipboard.txt", persistentClipboard);
                        }
                    }
                    catch { }
                }
                else if (id == HOTKEY_ID_PASTE)
                {
                    if (File.Exists("clipboard.txt"))
                    {
                        try
                        {
                            persistentClipboard = File.ReadAllText("clipboard.txt");
                            Clipboard.SetText(persistentClipboard);
                        }
                        catch { }

                        SendKeys.SendWait("^v");
                        System.Threading.Thread.Sleep(150); 
                    }
                }

            }

            base.WndProc(ref m);
        }
        private void OnExitClicked(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_COPY);
            UnregisterHotKey(this.Handle, HOTKEY_ID_PASTE);
            base.OnFormClosing(e);
        }

    }
}
