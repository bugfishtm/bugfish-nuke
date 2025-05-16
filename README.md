# Bugfish Nuke 

## 🔍 Introduction

Bugfish Nuke is a powerful emergency data deletion tool for Windows, designed to help you quickly and securely erase sensitive files, user data, and system traces. Whether you need to protect your privacy or respond to a critical situation, Bugfish Nuke offers fast, configurable, and reliable data destruction.

![Image](./_screenshots/software.png)

## ✨ Features

- Instantly erase user data, application traces, and sensitive files in a single click.
- Secure deletion: Files are overwritten, not just moved to the recycle bin.
- Clear clipboard contents
- Flush DNS cache
- Erase event logs
- Delete most recently used (MRU) lists
- Remove thumbnail caches
- Empty the recycle bin (system default)
- Set overwrite passes (1 or more) for each deletion:
  - 1 pass: Fast, effective for most cases
  - 3+ passes: Higher security, slower
  - **Warning:** 0 passes disables overwriting and is NOT recommended
- Add your own scripts (e.g., batch files) to run alongside the deletion process
	- Example: Dismount VeraCrypt volumes automatically
- Optional: Corrupt Windows login files after deletion to prevent further access
	- **Use with caution:** This will render Windows unbootable and require reinstallation
- Play a custom or built-in music track during deletion
- Music stops when deletion is complete-useful as an audible signal if you step away
- Overview of selected actions before launch
- Settings for post-deletion behavior: auto-close, force restart, etc.
- Direct links to tutorials and help resources

## 🛡️ Security Notes

- **Overwriting:** Files are overwritten according to your settings, making recovery nearly impossible.
- **Recycle Bin:** Emptied using Windows default; secure overwrite for the bin may be added in future versions.
- **Custom Scripts:** Scripts run independently and can be used to automate additional emergency tasks.
- **File Deletion:** Files are securely overwritten (unless you set passes to 0), making recovery extremely unlikely.
- **Music Player:** It provides an audible signal when the deletion process is finished, useful if you need to leave your computer during an emergency wipe.
- **Use with Caution:** Bugfish Nuke is designed for emergency situations. Use with care, especially the destructive system options.

![Image](./_screenshots/deletion.png)


> **Warning:** Use the "Corrupt Windows System" option only in extreme situations. This action is irreversible and will require a full system reinstall.


## 🖱️ Usage Information

- Copy the prefered version folder from _releases/_executable/VERSION to your computer.
- Ensure all files in the source folder remain together-do not move or delete any files.
- Run bugfish-nuke.exe.
- Use the UI to set up which applications and folders you want to be deleted in an emergency.
- When needed, activate the nuke function.

## ❓ Support Channels

If you encounter any issues or have questions while using this software, feel free to contact us:

- **GitHub Issues** is the main platform for reporting bugs, asking questions, or submitting feature requests: [https://github.com/bugfishtm/bugfish-nuke/issues](https://github.com/bugfishtm/bugfish-nuke/issues)
- **Discord Community** is available for live discussions, support, and connecting with other users: [Join us on Discord](https://discord.com/invite/nqe2HSuKee)  
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
