<p align="center">
  <img src="assets/logo-transparent.png" alt="Persistent Clipboard Logo" width="150"/>
</p>

# 📋 Persistent Clipboard
**Persistent Clipboard** is a lightweight Windows app that gives you a secondary clipboard that remembers copied content — even after it's overwritten by a new copy.

This version includes support for multiple data types (not just text).  
If you're looking for the simpler text-only version, [see below](#text-only-version).

---

## ⚙️ How It Works

Persistent Clipboard runs in the background and hooks into hotkeys to manage a second clipboard:

### 🧠 Persistent Hotkeys:
| Hotkey                  | Action                                                                 |
|-------------------------|------------------------------------------------------------------------|
| `Ctrl + Shift + C` or `Ctrl + Alt + C` | Copies current selection (just like normal copy) and also stores it to a **persistent clipboard** |
| `Ctrl + Shift + V` or `Ctrl + Alt + V` | Pastes the content from the **persistent clipboard**, and sets it as the current clipboard         |

### 📋 Regular Clipboard:
- `Ctrl + C` and `Ctrl + V` work normally.
- Persistent Clipboard does **not** interfere with your regular clipboard unless you use the special hotkeys above.
### 📁 File Storage

The persistent clipboard data (text) is stored in the following directory:

`%APPDATA%\PersistentClipboard\clipboard.txt`

This allows the clipboard data to be saved across system reboots and application restarts. You can manually edit the stored clipboard file if necessary, though the application automatically manages it.

### 💻 System Tray

The application runs in the system tray, allowing you to access it easily without cluttering your taskbar.

- Right-click the tray icon to view the options.
- Select **Exit** to close the application.
### 🔄 Auto Start on Windows
To make Persistent Clipboard start automatically when you log in:

- Press `Win + R` to open the Run dialog.
- Type `shell:startup` and hit Enter. This opens your Startup folder.
- Copy the `PersistentClipboard.exe` file into this folder.
---

## 🔽 Downloads

Go to the [**Releases**](https://github.com/dinhitcom/PersistentClipboard/releases) page to get the latest version.

| File                     | Description                                 |
|--------------------------|---------------------------------------------|
| `PersistentClipboard.exe` | Standalone executable – run without installing |
| `PersistentClipboard-installer.zip` file              | Installer package for Windows (recommended) |
| `PersistentClipboard-text.exe` | Prebuilt **text-only** version                          |

---

## 📝 Text-Only Version

Want just a minimal version that handles **text only**?

- ✅ [Switch to the `text` branch](https://github.com/dinhitcom/PersistentClipboard/tree/text) to view the code
- 🛠️ The `text` branch only supports plain text, perfect for lightweight use cases
- 📥 Prebuilt version available in the [Releases](hhttps://github.com/dinhitcom/PersistentClipboard/releases) as `PersistentClipboard-text.exe`

---

## 🛠️ Installation (Optional for Developers)

### Prerequisites

- .NET Framework 4.8 or later
- Windows 10 or later

### How to Build and Run

1. Clone or download the repository.
2. Open the solution file (`PersistentClipboard.sln`) in Visual Studio.
3. Build the project using Visual Studio.
4. Run the application (`PersistentClipboard.exe`).
   
   The application will minimize to the system tray. You can then use the hotkeys to copy and paste text.


### Switching to Other Versions (Branches)

This version of Persistent Clipboard only handles text-based data. If you are interested in a version that supports other types of data (such as images or files), you can switch to another branch in this repository that includes these features.

To switch branches, follow these steps:

1. Open your terminal or Git client.
2. Navigate to the repository directory.
3. Use the following command to check out the branch that includes support for more data types:

   ```bash
   git checkout [branch-name]
---
## 🖼️ Logo

The app logo is included as part of the UI and tray icon. You can customize it by replacing the embedded `.ico` resource or modifying the project assets.

---

## 🤝 Contributions

Feel free to fork or contribute! Bug fixes, feature improvements, or UI suggestions are welcome.

---

## 🧾 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 💬 Contact
For issues, enhancements, or contributions, please open an issue on the GitHub repository or contact the project maintainers.

