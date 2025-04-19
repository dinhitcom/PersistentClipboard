using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public partial class Form1 : Form
    {
        const int HOTKEY_ID_COPY = 1;
        const int HOTKEY_ID_PASTE = 2;
        const int HOTKEY_ID_COPY_ALT = 3;
        const int HOTKEY_ID_PASTE_ALT = 4;

        const int MOD_ALT = 0x0001;
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
        private string clipboardFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadEmbeddedResources()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream("PersistentClipboard.icon-transparent.ico"))
            {
                if (stream != null)
                    trayIcon.Icon = new Icon(stream);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "PersistentClipboard");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            clipboardFilePath = Path.Combine(appFolder, "clipboard.txt");

            RegisterHotKey(this.Handle, HOTKEY_ID_COPY, MOD_CONTROL | MOD_SHIFT, (int)Keys.C);
            RegisterHotKey(this.Handle, HOTKEY_ID_PASTE, MOD_CONTROL | MOD_SHIFT, (int)Keys.V);
            RegisterHotKey(this.Handle, HOTKEY_ID_COPY_ALT, MOD_CONTROL | MOD_ALT, (int)Keys.C);
            RegisterHotKey(this.Handle, HOTKEY_ID_PASTE_ALT, MOD_CONTROL | MOD_ALT, (int)Keys.V);


            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "Persistent Clipboard";
            LoadEmbeddedResources();

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                if (id == HOTKEY_ID_COPY || id == HOTKEY_ID_COPY_ALT)
                {
                    SendKeys.SendWait("^c");
                    System.Threading.Thread.Sleep(100);

                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            persistentClipboard = Clipboard.GetText(); 
                            File.WriteAllText(clipboardFilePath, persistentClipboard);
                        }
                    }
                    catch { }
                }
                else if (id == HOTKEY_ID_PASTE || id == HOTKEY_ID_PASTE_ALT)
                {
                    if (persistentClipboard != null || File.Exists(clipboardFilePath))
                        //if (persistentClipboard != null)
                    {
                        try
                        {
                            persistentClipboard = File.ReadAllText(clipboardFilePath);
                            Clipboard.SetText(persistentClipboard);
                        }
                        catch { }

                        SendKeys.SendWait("^v");
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
            UnregisterHotKey(this.Handle, HOTKEY_ID_COPY_ALT);
            UnregisterHotKey(this.Handle, HOTKEY_ID_PASTE_ALT);
            base.OnFormClosing(e);
        }

    }
}
