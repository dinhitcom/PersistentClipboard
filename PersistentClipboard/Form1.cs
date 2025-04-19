using PersistentClipboard.Model;
using PersistentClipboard.Utils;
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
        private const int HOTKEY_ID_COPY = 1;
        private const int HOTKEY_ID_PASTE = 2;
        private const int HOTKEY_ID_COPY_ALT = 3;
        private const int HOTKEY_ID_PASTE_ALT = 4;

        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private ClipboardData persistentClipboard = null;
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
                {
                    trayIcon.Icon = new Icon(stream);
                }
            }
        }

        private void LoadPersistentClipboard()
        {
            if (File.Exists(clipboardFilePath))
            {
                try
                {
                    string json = File.ReadAllText(clipboardFilePath);
                    persistentClipboard = JsonHelper.Deserialize<ClipboardData>(json);
                }
                catch { }
            }
        }

        private void CleanOldFiles()
        {
            if (persistentClipboard != null) {
                try
                {
                    var persistentClipboardData = persistentClipboard;
                    List<string> filesToKeep = new List<string>();

                    if (persistentClipboardData.Type == ClipboardDataType.FileDropList)
                    {
                        filesToKeep.AddRange(persistentClipboardData.Data);
                    }

                    FileHelper.ClearFiles(clipboardFilesDirectory, filesToKeep);
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "PersistentClipboard");
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);

            clipboardDirectoryPath = appFolder;
            clipboardFilePath = Path.Combine(appFolder, "clipboard.txt");
            clipboardFilesDirectory = Path.Combine(appFolder, "files");
            if (!Directory.Exists(clipboardFilesDirectory)) Directory.CreateDirectory(clipboardFilesDirectory);

            LoadPersistentClipboard();
            CleanOldFiles();

            RegisterHotKey(Handle, HOTKEY_ID_COPY, MOD_CONTROL | MOD_SHIFT, (int)Keys.C);
            RegisterHotKey(Handle, HOTKEY_ID_PASTE, MOD_CONTROL | MOD_SHIFT, (int)Keys.V);
            RegisterHotKey(Handle, HOTKEY_ID_COPY_ALT, MOD_CONTROL | MOD_ALT, (int)Keys.C);
            RegisterHotKey(Handle, HOTKEY_ID_PASTE_ALT, MOD_CONTROL | MOD_ALT, (int)Keys.V);

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Clear Storage", null, OnClearClicked);
            trayMenu.Items.Add("Exit", null, OnExitClicked);

            trayIcon = new NotifyIcon
            {
                Text = "Persistent Clipboard"
            };
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
                    HandleCopy();
                }
                else if (id == HOTKEY_ID_PASTE || id == HOTKEY_ID_PASTE_ALT)
                {
                    HandlePaste();
                }
            }

            base.WndProc(ref m);
        }

        private void HandleCopy()
        {
            SendKeys.SendWait("^c");
            System.Threading.Thread.Sleep(100);

            try
            {
                ClipboardData clipboardData = null;

                if (Clipboard.ContainsText())
                {
                    clipboardData = new ClipboardData
                    {
                        Type = ClipboardDataType.Text,
                        Data = new string[] { Clipboard.GetText() }
                    };
                }
                else if (Clipboard.ContainsImage())
                {
                    string imagePath = Path.Combine(clipboardDirectoryPath, "clipboard.png");
                    Clipboard.GetImage().Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    clipboardData = new ClipboardData
                    {
                        Type = ClipboardDataType.Image,
                        Data = new string[] { imagePath }
                    };
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    var fileDropList = Clipboard.GetFileDropList();
                    List<string> copiedFiles = new List<string>();

                    foreach (string filePath in fileDropList)
                    {
                        string destFile = Path.Combine(clipboardFilesDirectory, Path.GetFileName(filePath));
                        File.Copy(filePath, destFile, true);
                        copiedFiles.Add(destFile);
                    }

                    clipboardData = new ClipboardData
                    {
                        Type = ClipboardDataType.FileDropList,
                        Data = copiedFiles.ToArray()
                    };
                }

                if (clipboardData != null)
                {
                    persistentClipboard = clipboardData;
                    File.WriteAllText(clipboardFilePath, JsonHelper.Serialize(clipboardData));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when copying to persistent clipboard: " + ex.Message);
            }
        }

        private void HandlePaste()
        {
            if (persistentClipboard is ClipboardData clipboardData)
            {
                try
                {
                    switch (clipboardData.Type)
                    {
                        case ClipboardDataType.Text:
                            Clipboard.SetText(clipboardData.Data[0]);
                            break;

                        case ClipboardDataType.Image:
                            Clipboard.SetImage(Image.FromFile(clipboardData.Data[0]));
                            break;

                        case ClipboardDataType.FileDropList:
                            StringCollection filePaths = new StringCollection();
                            filePaths.AddRange(clipboardData.Data);
                            Clipboard.SetFileDropList(filePaths);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error when pasting from persistent clipboard: " + ex.Message);
                }

                SendKeys.SendWait("^v");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            try
            {
                FileHelper.ClearFiles(clipboardDirectoryPath);
                FileHelper.ClearFiles(clipboardFilesDirectory);
            } catch (Exception ex)
            {
                MessageBox.Show("Error when clear storage: " + ex.Message);
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, HOTKEY_ID_COPY);
            UnregisterHotKey(Handle, HOTKEY_ID_PASTE);
            UnregisterHotKey(Handle, HOTKEY_ID_COPY_ALT);
            UnregisterHotKey(Handle, HOTKEY_ID_PASTE_ALT);
            base.OnFormClosing(e);
        }
    }
}
