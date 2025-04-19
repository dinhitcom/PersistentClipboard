using Newtonsoft.Json;
using PersistentClipboard.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        private object persistentClipboard = null;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private string clipboardDirectoryPath;
        private string clipboardFilePath;
        private string clipboardFilesDirectory;

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

        private void LoadPersistentClipboard()
        {
            if (persistentClipboard == null && File.Exists(clipboardFilePath))
            {
                try
                {
                    var json = File.ReadAllText(clipboardFilePath);
                    var clipboardData = JsonConvert.DeserializeObject<ClipboardData>(json);
                    persistentClipboard = clipboardData;
                }
                catch { }
            }
        }
        private void CleanOldFiles()
        {
            if (persistentClipboard != null) {
                try
                {
                    var persistentClipboardData = (ClipboardData)persistentClipboard;
                    string[] files = Directory.GetFiles(clipboardFilesDirectory);
                    List<string> filesToKeep = new List<string>();

                    if (persistentClipboardData.Type == ClipboardDataType.FileDropList)
                    {
                        filesToKeep.AddRange(persistentClipboardData.Data);
                    }

                    foreach (string file in files)
                    {
                        if (!filesToKeep.Contains(file))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
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
            clipboardDirectoryPath = appFolder;
            clipboardFilePath = Path.Combine(clipboardDirectoryPath, "clipboard.txt");
            clipboardFilesDirectory = Path.Combine(clipboardDirectoryPath, "files");
            Directory.CreateDirectory(clipboardFilesDirectory);
            LoadPersistentClipboard();
            CleanOldFiles();

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
                            var text = Clipboard.GetText();
                            var clipboardData = new ClipboardData
                            {
                                Type = ClipboardDataType.Text,
                                Data = new string[] { text }
                            };

                            persistentClipboard = clipboardData;
                            File.WriteAllText(clipboardFilePath, JsonConvert.SerializeObject(clipboardData));
                        }
                        else if (Clipboard.ContainsImage())
                        {
                            Image clipboardImage = Clipboard.GetImage();
                            string imagePath = Path.Combine(clipboardDirectoryPath, "clipboard.png");
                            clipboardImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                            var clipboardData = new ClipboardData
                            {
                                Type = ClipboardDataType.Image,
                                Data = new string[] { imagePath }
                            };

                            persistentClipboard = clipboardData;
                            File.WriteAllText(clipboardFilePath, JsonConvert.SerializeObject(clipboardData));
                        }
                        else if (Clipboard.ContainsFileDropList())
                        {
                            var filePaths = Clipboard.GetFileDropList();
                            List<string> files = new List<string>();
                            foreach (var filePath in filePaths)
                            {
                                string destFile = Path.Combine(clipboardFilesDirectory, Path.GetFileName(filePath));
                                File.Copy(filePath, destFile, true);
                                files.Add(destFile);
                            }

                            var clipboardData = new ClipboardData
                            {
                                Type = ClipboardDataType.FileDropList,
                                Data = files.ToArray()
                            };
                            persistentClipboard = clipboardData;
                            File.WriteAllText(clipboardFilePath, JsonConvert.SerializeObject(clipboardData));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error when copy to persistent clipboard: " + ex.Message);
                    }
                }
                else if (id == HOTKEY_ID_PASTE || id == HOTKEY_ID_PASTE_ALT)
                {
                    if (persistentClipboard != null)
                    {
                        var persistentClipboardData = (ClipboardData)persistentClipboard;

                        try
                        {
                            if (persistentClipboardData.Type == ClipboardDataType.Text)
                            {
                                Clipboard.SetText(persistentClipboardData.Data[0]);
                            }
                            else if (persistentClipboardData.Type == ClipboardDataType.Image)
                            {
                                Image img = Image.FromFile(persistentClipboardData.Data[0]);
                                Clipboard.SetImage(img);
                            }
                            else if (persistentClipboardData.Type == ClipboardDataType.FileDropList)
                            {

                                StringCollection filePaths = new StringCollection();
                                filePaths.AddRange(persistentClipboardData.Data);
                                Clipboard.SetFileDropList(filePaths);
                            }
                        }
                        catch (Exception ex) {
                            MessageBox.Show("Error when pasting from persistent clipboard: " + ex.Message);
                        }

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
