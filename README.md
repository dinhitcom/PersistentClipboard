# Persistent Clipboard (Text-Only Version)
<p align="center">
  <img src="assets/logo-transparent.png" alt="Persistent Clipboard Logo" width="200"/>
</p>

## Overview

Persistent Clipboard is a lightweight Windows application designed to provide a secondary, persistent clipboard for text. This version of the app specifically handles text-only clipboard operations and allows users to save and retrieve text between sessions, making it ideal for users who often need to store and paste recurring text snippets.

## Features

- **Text Persistence:** Automatically saves the most recent text copied to the clipboard.
- **Customizable Hotkeys:** Use keyboard shortcuts to copy and paste from the persistent clipboard.
- **Tray Icon Support:** Minimizes to the system tray for easy access, with an option to exit from the tray menu.
- **Cross-Session Clipboard:** Clipboard data is stored in a consistent location, even across reboots, so that you can always access your last copied text.

## How It Works

### Copying Text to the Clipboard

When you press **Ctrl + Shift + C** or **Ctrl + Alt + C**, the following actions occur:
- The application simulates a regular **Ctrl + C** (normal copy) operation to copy the currently selected text to the clipboard.
- In addition to the regular copy, the application saves the copied text to persistent storage (a file called `clipboard.txt` stored in `%APPDATA%\PersistentClipboard\`).
- This means that the copied text is saved not only to the clipboard but also to a persistent file, so you can access it later, even after the computer is restarted.

If you press **Ctrl + C**, it behaves like a normal copy operation and does **not** store the value to the persistent clipboard.

### Pasting from the Clipboard

When you press **Ctrl + V**, it pastes the most recent copied value from the clipboard. This is **not** the value stored in persistent storage, but the latest copied value from the standard clipboard.

### Pasting from the Persistent Clipboard

When you press **Ctrl + Shift + V** or **Ctrl + Alt + V**, the application will:
- Paste the last value saved in the **persistent clipboard** (the value saved to `clipboard.txt`).
- After pasting, the value from the persistent clipboard is **set as the last copied value** on the system clipboard. This allows you to quickly reuse the persistent value in other applications.

This ensures that when you paste, you have control over whether you're using the normal clipboard (via **Ctrl + V**) or the persistent clipboard (via **Ctrl + Shift + V** or **Ctrl + Alt + V**).

### File Storage

The persistent clipboard data (text) is stored in the following directory:

`%APPDATA%\PersistentClipboard\clipboard.txt`

This allows the clipboard data to be saved across system reboots and application restarts. You can manually edit the stored clipboard file if necessary, though the application automatically manages it.

### System Tray

The application runs in the system tray, allowing you to access it easily without cluttering your taskbar.

- Right-click the tray icon to view the options.
- Select **Exit** to close the application.

## Download

You can **download the latest prebuilt version** from the [Releases section](https://github.com/dinhitcom/PersistentClipboard/releases) on GitHub. Just download the `.zip` or `.exe` from the latest release, extract it (if needed), and run the `PersistentClipboard.exe` file.

> No need to build it yourself unless you want to modify or contribute to the code.

## Installation (Optional for Developers)

### Prerequisites

- .NET Framework 4.5 or later
- Windows 7 or later

### How to Build and Run

1. Clone or download the repository.
2. Open the solution file (`PersistentClipboard.sln`) in Visual Studio.
3. Build the project using Visual Studio.
4. Run the application (`PersistentClipboard.exe`).
   
   The application will minimize to the system tray. You can then use the hotkeys to copy and paste text.

## Switching to Other Versions (Branches)

This version of Persistent Clipboard only handles text-based data. If you are interested in a version that supports other types of data (such as images or files), you can switch to another branch in this repository that includes these features.

To switch branches, follow these steps:

1. Open your terminal or Git client.
2. Navigate to the repository directory.
3. Use the following command to check out the branch that includes support for more data types:

   ```bash
   git checkout [branch-name]
## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For issues, enhancements, or contributions, please open an issue on the GitHub repository or contact the project maintainers.
