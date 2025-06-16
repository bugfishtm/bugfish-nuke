# Bugfish Nuke 


> ⚠️ Do not use this tool for hacking, unauthorized access, or any criminal activity. Misuse may result in data loss, system inaccessibility, or legal consequences. Always comply with local laws and use only on systems you own or are authorized to manage.

## 🔍 Introduction

> [!NOTE]
> No new features are planned for this project at this time.

> [!TIP]
> This project is actively maintained, with regular updates and prompt fixes for reported issues.

Bugfish Nuke is a powerful emergency data deletion tool for Windows, designed to help you quickly and securely erase sensitive files, user data, and system traces. Whether you need to protect your privacy or respond to a critical situation, Bugfish Nuke offers fast, configurable, and reliable data destruction.

![Image](./_screenshots/software.png)

## ✨ Features

This tool provides instant, secure erasure of user data, application traces, and sensitive files with a single click. It uses advanced deletion methods, overwriting files rather than simply moving them to the recycle bin, and allows users to set the number of overwrite passes for each operation—balancing speed and security according to their needs. For added flexibility, users can integrate their own scripts, such as automatically dismounting encrypted volumes during the cleanup process.

An optional, advanced feature lets users corrupt Windows login files after deletion, effectively locking out further access to the system (intended for emergency scenarios only). To enhance the user experience, the tool can play a custom or built-in music track during deletion, providing an audible signal when the process is complete. Before any action is executed, users receive a clear overview of what will happen, and can configure post-deletion behaviors such as auto-closing the tool or forcing a system restart. Direct access to tutorials and help resources is also available for guidance and support.

**Warning:** Use the "Corrupt Windows System" option only in extreme situations. This action is irreversible and will require a full system reinstall.

| Name | Shortcode | Description (from code logic) |
|---|---|---|
| [Software] Brave: Close and Delete User Data | brave | Kill process: `brave`. Securely delete `%LocalAppData%\BraveSoftware\Brave-Browser\User Data` |
| [Software] Cyberduck: Close and Delete User Data | cyberduck | Kill process: `Cyberduck`. Securely delete `%AppData%\Cyberduck` |
| [Software] Discord: Close and Delete User Data | discord | Kill process: `Discord`. Securely delete `%AppData%\discord` |
| [Software] Dropbox: Close and Delete User Data | dropbox | Kill process: `Dropbox`. Securely delete `%AppData%\Dropbox` |
| [Software] Edge: Close and Delete User Data | edge | Kill process: `msedge`. Securely delete `%LocalAppData%\Microsoft\Edge\User Data` |
| [Software] FileZilla: Close and Delete User Data | filezilla | Kill process: `filezilla`. Securely delete `%AppData%\FileZilla` |
| [Software] Google Chrome: Close and Delete User Data | chrome | Kill process: `Chrome`. Securely delete `%LocalAppData%\Google\Chrome\User Data` |
| [Software] ICQ: Close and Delete User Data | icq | Kill process: `icq`. Securely delete `%AppData%\ICQ` |
| [Software] KeePass: Close and Delete User Data | keepass | Kill process: `KeePass`. Securely delete `%AppData%\KeePass` |
| [Software] Microsoft Outlook: Close and Delete User Data | outlook | Kill process: `OUTLOOK`. Securely delete `%LocalAppData%\Microsoft\Outlook` |
| [Software] Microsoft Teams: Close and Delete User Data | teams | Kill process: `Teams`. Securely delete `%AppData%\Microsoft\Teams` |
| [Software] Mozilla Firefox: Close and Delete User Data | firefox | Kill process: `firefox`. Securely delete `%AppData%\Mozilla\Firefox\Profiles` |
| [Software] Mozilla Thunderbird: Close and Delete User Data | thunderbird | Kill process: `thunderbird`. Securely delete `%AppData%\Thunderbird\Profiles` |
| [Software] Native Access: Close and Delete User Data | nativeaccess | Kill process: `NativeAccess`. Securely delete `%AppData%\NativeAccess` |
| [Software] Nextcloud: Close and Delete User Data | nextcloud | Kill process: `Nextcloud`. Securely delete `%AppData%\Nextcloud` |
| [Software] OneDrive: Close and Delete User Data | onedrive | Kill process: `OneDrive`. Securely delete `%LocalAppData%\Microsoft\OneDrive` |
| [Software] OBS: Close and Delete User Data | obs | Kill process: `OBS`. Securely delete `%AppData%\obs-studio` |
| [Software] Opera: Close and Delete User Data | opera | Kill process: `opera`. Securely delete `%AppData%\Opera Software\Opera Stable` |
| [Software] Opera GX: Close and Delete User Data | operagx | Kill process: `opera`. Securely delete `%AppData%\Opera Software\Opera GX Stable` |
| [Software] Signal: Close and Delete User Data | signal | Kill process: `Signal`. Securely delete `%AppData%\Signal` |
| [Software] Skype: Close and Delete User Data | skype | Kill process: `skype`. Securely delete `%AppData%\Skype` |
| [Software] Slack: Close and Delete User Data | slack | Kill process: `slack`. Securely delete `%AppData%\Slack` |
| [Software] Steam: Close and Delete User Data | steam | Kill process: `Steam`. Securely delete `%AppData%\Steam` and `%LocalAppData%\Steam` |
| [Software] Telegram: Close and Delete User Data | telegram | Kill processes: `Telegram`, `TelegramDesktop`. Securely delete `%AppData%\Telegram Desktop` |
| [Software] Tor Browser: Close and Delete User Data | tor | Kill process: `firefox` (Tor uses Firefox). Securely delete `%AppData%\Tor Browser` |
| [Software] Unity Hub: Close and Delete User Data | unityhub | Kill process: `Unity Hub`. Securely delete `%AppData%\UnityHub` |
| [Software] VeraCrypt: Close and Delete User Data | veracrypt | Kill process: `VeraCrypt`. Securely delete `%AppData%\VeraCrypt` |
| [Software] Viber: Close and Delete User Data | viber | Kill process: `Viber`. Securely delete `%AppData%\ViberPC` |
| [Software] Vivaldi: Close and Delete User Data | vivaldi | Kill process: `vivaldi`. Securely delete `%LocalAppData%\Vivaldi\User Data` |
| [Software] WhatsApp: Close and Delete User Data | whatsapp | Kill process: `WhatsApp`. Securely delete `%AppData%\WhatsApp` |
| [Software] WinSCP: Close and Delete User Data | winscp | Kill process: `WinSCP`. Securely delete `%AppData%\WinSCP` |
| [Software] Zoom: Close and Delete User Data | zoom | Kill process: `Zoom`. Securely delete `%AppData%\Zoom` |
| [Windows] Authenticator: Close and Delete Data | winauth | Kill process: `WinAuth`. Securely delete `%AppData%\WinAuth` |
| [Windows] BitLocker Recovery Keys: Delete | securekey-bitlocker | Securely delete `%ProgramData%\Microsoft\Protect\Recovery` |
| [Windows] Clipboard: Clear | windows_clipboard | Calls `OpenClipboard`, `EmptyClipboard`, then `CloseClipboard` to clear clipboard contents |
| [Windows] Credential Manager: Clear and Delete Login and Auth Keys | clear_all_creds | Clear the internal stored credentials in the windows credential manager. |
| [Windows] DirectX Shader Cache: Clear | windows_directx_shader_cache | Securely delete `%LocalAppData%\D3DSCache` |
| [Windows] DNS Cache: Clear | windows_dns_cache | Runs `ipconfig /flushdns` to clear DNS resolver cache |
| [Windows] Driver Install Logs: Clear | windows_driver_install_logs | Securely delete `%SystemRoot%\inf\setupapi.dev.log` and `%SystemRoot%\inf\setupapi.app.log` |
| [Windows] Explorer: Clear Most Recently Used List | windows_explorer_mru | Clears `RecentDocs` registry key under `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer` |
| [Windows] Explorer: Clear Thumbnail Cache | windows_thumbnail_cache | Securely delete `%LocalAppData%\Microsoft\Windows\Explorer\thumbcache_*.db` |
| [Windows] Event Logs: Clear | windows_event_logs | Runs `wevtutil cl` for each log in `wevtutil el` to clear all Windows Event Logs |
| [Windows] Error Reporting: Clear | windows_error_reporting | Securely delete `%ProgramData%\Microsoft\Windows\WER` |
| [Windows] Event Trace Logs: Clear | windows_event_trace_logs | Securely delete `%SystemRoot%\System32\LogFiles\WMI` |
| [Windows] Machine-Level Crypto-Keys: Delete | securekey-machine | Securely delete `%ProgramData%\Microsoft\Crypto\RSA\MachineKeys` |
| [Windows] Font Cache: Clear | windows_font_cache | Securely delete `%LocalAppData%\FontCache` |
| [Windows] Log Files: Clear | windows_log_files | Securely delete `%SystemRoot%\Logs` and `%SystemRoot%\System32\LogFiles` |
| [Windows] Memory Dumps: Clear | windows_memory_dumps | Securely delete `%SystemRoot%\MEMORY.DMP` and `%SystemRoot%\Minidump` |
| [Windows] Office: Recent File History | office-recent-files | Clears recent files list in Office registry under `HKCU\Software\Microsoft\Office\*\*\File MRU` |
| [Windows] Prefetch Data: Clear | windows_prefetch | Securely delete `%SystemRoot%\Prefetch` |
| [Windows] System Restore Points: Clear | windows_system_restore_cleanup | Runs `vssadmin delete shadows /all /quiet` to delete all restore points |
| [Windows] Recent Documents List: Clear | windows_recent_documents | Securely delete `%AppData%\Microsoft\Windows\Recent` |
| [Windows] Recent File List: Clear | windows_recent | Securely delete `%AppData%\Microsoft\Windows\Recent` |
| [Windows] Trash Bin: Clear | windows_trash | Runs `rd /s /q %systemdrive%\$Recycle.Bin` to empty Recycle Bin |
| [Windows] User SSH Key Folder: Delete | securekey-ssh | Securely delete `%UserProfile%\.ssh` |
| [Windows] User EFS Keys: Delete | securekey-efs-user | Securely delete `%AppData%\Microsoft\Crypto\RSA` |
| [Windows] Update Cache: Clear | windows_update_cleanup | Securely delete `%SystemRoot%\SoftwareDistribution\Download` |
| [Windows] User Assist History: Clear | windows_userassist | Clears `UserAssist` registry keys under `HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist` |
| [Windows] Visual Studio 2022: Clear Settings and History | empty_vs2022_settings | Empty VS2022 `ApplicationPrivateSettings.xml` files. |
| [Windows] Web Cache: Clear | windows_webcache | Securely delete `%LocalAppData%\Microsoft\Windows\WebCache` |
| [Windows] WebDav Cache: Clear | windows_webdav_cache | Securely delete `%LocalAppData%\Microsoft\Windows\WebCache` |

## 🖱️ Usage Information

- Copy the prefered version folder from _releases/_executable/VERSION to your computer.
- Ensure all files in the source folder remain together.
- Run bugfish-nuke.exe.
- Use the UI to set up which applications and folders you want to be deleted in an emergency.
- When needed, activate the nuke function.

## 🛡️ Security Notes

- **Overwriting:** Files are overwritten according to your settings, making recovery nearly impossible.
- **Recycle Bin:** Emptied using Windows default; secure overwrite for the bin may be added in future versions.
- **Custom Scripts:** Scripts run independently and can be used to automate additional emergency tasks.
- **File Deletion:** Files are securely overwritten (unless you set passes to 0), making recovery extremely unlikely.
- **Music Player:** It provides an audible signal when the deletion process is finished, useful if you need to leave your computer during an emergency wipe.
- **Use with Caution:** Bugfish Nuke is designed for emergency situations. Use with care, especially the destructive system options.

## ❓ Support Channels

If you encounter any issues or have questions while using this software, feel free to contact us:

- **GitHub Issues** is the main platform for reporting bugs, asking questions, or submitting feature requests: [https://github.com/bugfishtm/bugfish-nuke/issues](https://github.com/bugfishtm/bugfish-nuke/issues)
- **Discord Community** is available for live discussions, support, and connecting with other users: [Join us on Discord](https://discord.com/invite/xCj7AEMmye)  
- **Email support** is recommended only for urgent security-related issues: [security@bugfish.eu](mailto:security@bugfish.eu)
- **Tutorial Video** for a full walkthrough, at https://www.youtube.com/live/loCPh5M96ko.


## 📢 Spread the Word

Help us grow by sharing this project with others! You can:  

* **Tweet about it** – Share your thoughts on [Twitter/X](https://twitter.com) and link us!  
* **Post on LinkedIn** – Let your professional network know about this project on [LinkedIn](https://www.linkedin.com).  
* **Share on Reddit** – Talk about it in relevant subreddits like [r/programming](https://www.reddit.com/r/programming/) or [r/opensource](https://www.reddit.com/r/opensource/).  
* **Tell Your Community** – Spread the word in Discord servers, Slack groups, and forums.  

## 📁 Repository Structure 

This table provides an overview of key files and folders related to the repository. Click on the links to access each file for more detailed information. If certain folders are missing from the repository, they are irrelevant to this project.

|Document Type|Description|
|----|-----|
| .github | Folder with github setup files. |
| [.github/CODE_OF_CONDUCT.md](./.github/CODE_OF_CONDUCT.md) | The community guidelines. |
| _changelogs | Folder for changelogs. |
| _images | Folder for project images. |
| _packages | Folder for installable packages mostly for suitefish-cms. |
| _releases | Folder for releases. |
| _screenshots | Folder with project screenshots. |
| _source | Folder with the source code. |
| _videos | Folder with videos related to the project. |
| .gitattributes | Repository setting file. Only for development purposes. |
| .gitignore | Repository ignore file. Only for development purposes. |
| docs | Folder for Github Pages Documentation. |
| README.md | Readme of this project. You are currently looking at this file. |
| repository_reset.bat | File to reset this repository. Only for development purposes. |
| repository_update.bat | File to update this repository. Only for development purposes. |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Information for contributors. | 
| [CHANGELOG.md](CHANGELOG.md) | Information about changelogs. | 
| [SECURITY.md](SECURITY.md) | How to handle security issues. |
| [LICENSE.md](LICENSE.md) | License of this project. |

## 📑 Changelog Information

Refer to the `_changelogs` folder for detailed insights into the changes made across different versions. The changelogs are available in **HTML format** within this folder, providing a structured record of updates, modifications, and improvements over time. Additionally, **GitHub Releases** follow the same structure and also include these changelogs for easy reference.

## 🌱 Contributing to the Project

I am excited that you're considering contributing to our project! Here are some guidelines to help you get started.

**How to Contribute**

1. Fork the repository to create your own copy.
2. Create a new branch for your work (e.g., `feature/my-feature`).
3. Make your changes and ensure they work as expected.
4. Run tests to confirm everything is functioning correctly.
5. Commit your changes with a clear, concise message.
6. Push your branch to your forked repository.
7. Submit a pull request with a detailed description of your changes.
8. Reference any related issues or discussions in your pull request.

**Coding Style**

- Keep your code clean and well-organized.
- Add comments to explain complex logic or functions.
- Use meaningful and consistent variable and function names.
- Break down code into smaller, reusable functions and components.
- Follow proper indentation and formatting practices.
- Avoid code duplication by reusing existing functions or modules.
- Ensure your code is easily readable and maintainable by others.

## 🤝 Community Guidelines

We’re on a mission to create groundbreaking solutions, pushing the boundaries of technology. By being here, you’re an integral part of that journey. 

**Positive Guidelines:**
- Be kind, empathetic, and respectful in all interactions.
- Engage thoughtfully, offering constructive, solution-oriented feedback.
- Foster an environment of collaboration, support, and mutual respect.

**Unacceptable Behavior:**
- Harassment, hate speech, or offensive language.
- Personal attacks, discrimination, or any form of bullying.
- Sharing private or sensitive information without explicit consent.

Let’s collaborate, inspire one another, and build something extraordinary together!

## 🛡️ Warranty and Security

I take security seriously and appreciate responsible disclosure. If you discover a vulnerability, please follow these steps:

- **Do not** report it via public GitHub issues or discussions. Instead, please contact the [security@bugfish.eu](mailto:security@bugfish.eu) email address directly.   
- Provide as much detail as possible, including a description of the issue, steps to reproduce it, and its potential impact.  

I aim to acknowledge reports within **2–4 weeks** and will update you on our progress once the issue is verified and addressed.

This software is provided as-is, without any guarantees of security, reliability, or fitness for any particular purpose. We do not take responsibility for any damage, data loss, security breaches, or other issues that may arise from using this software. By using this software, you agree that We are not liable for any direct, indirect, incidental, or consequential damages. Use it at your own risk.

## 📜 License Information

The license for this software can be found in the [LICENSE.md](LICENSE.md) file. Third-party licenses are located in the ./_licenses folder. The software may also include additional licensed software or libraries.

🐟 Bugfish 
