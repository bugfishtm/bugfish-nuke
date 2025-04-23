#include <windows.h>
#include <shellapi.h>
#include <shlobj.h>
#include <filesystem>
#include <iostream>
#include <string>
#include <vector>
#include <sstream>
#include <cstdlib>

#pragma comment(lib, "Shell32.lib")

bool DeleteDirectoryRecursively(const std::wstring& dir_path) {
    WIN32_FIND_DATAW find_data;
    HANDLE hFind = INVALID_HANDLE_VALUE;
    std::wstring spec = dir_path + L"\\*";
    hFind = FindFirstFileW(spec.c_str(), &find_data);
    if (hFind == INVALID_HANDLE_VALUE) return false;

    do {
        const std::wstring item = find_data.cFileName;
        if (item == L"." || item == L"..") continue;
        std::wstring full_path = dir_path + L"\\" + item;
        if (find_data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
            // Recursively delete subdirectory
            DeleteDirectoryRecursively(full_path);
        }
        else {
            // Delete file
            SetFileAttributesW(full_path.c_str(), FILE_ATTRIBUTE_NORMAL);
            DeleteFileW(full_path.c_str());
        }
    } while (FindNextFileW(hFind, &find_data));
    FindClose(hFind);

    // Remove the now-empty directory
    SetFileAttributesW(dir_path.c_str(), FILE_ATTRIBUTE_NORMAL);
    return RemoveDirectoryW(dir_path.c_str()) != 0;
}

bool IsUserAdmin() {
    BOOL isAdmin = FALSE;
    PSID adminGroup = NULL;
    SID_IDENTIFIER_AUTHORITY ntAuthority = SECURITY_NT_AUTHORITY;
    if (AllocateAndInitializeSid(&ntAuthority, 2,
        SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS,
        0, 0, 0, 0, 0, 0, &adminGroup)) {
        CheckTokenMembership(NULL, adminGroup, &isAdmin);
        FreeSid(adminGroup);
    }
    return isAdmin;
}

void KillProcess(const std::wstring& processName) {
    std::wstringstream cmd;
    cmd << L"taskkill /IM " << processName << L" /F >nul 2>&1";
    _wsystem(cmd.str().c_str());
}

bool DirectoryExists(const std::wstring& dir) {
    DWORD attrib = GetFileAttributesW(dir.c_str());
    return (attrib != INVALID_FILE_ATTRIBUTES && (attrib & FILE_ATTRIBUTE_DIRECTORY));
}

void DeleteDirectory(const std::wstring& dir) {
    if (!DirectoryExists(dir)) return;
    SHFILEOPSTRUCTW fileOp = { 0 };
    std::wstring from = dir + L'\0'; // Double null-terminated
    fileOp.wFunc = FO_DELETE;
    fileOp.pFrom = from.c_str();
    fileOp.fFlags = FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_SILENT;
    SHFileOperationW(&fileOp);
}

void ClearRecycleBin() {
    SHEmptyRecycleBinW(NULL, NULL, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
}

std::wstring GetEnvVar(const wchar_t* var) {
    wchar_t buf[MAX_PATH];
    DWORD len = GetEnvironmentVariableW(var, buf, MAX_PATH);
    if (len == 0 || len > MAX_PATH) return L"";
    return buf;
}

int main() {

    std::wcout << L"Starting cleanup process...\n\n";

    if (!IsUserAdmin()) {
        std::wcout << L"This script requires administrator privileges. Please run as administrator.\n";
        system("pause");
        return 1;
    }

    // === Nextcloud ===
    std::wcout << L"Closing Nextcloud client...\n";
    KillProcess(L"nextcloud.exe");

    std::wcout << L"Deleting Nextcloud configuration and cache files...\n";
    std::wstring nextcloudConfig = GetEnvVar(L"APPDATA") + L"\\Nextcloud";
    std::wstring nextcloudCache = GetEnvVar(L"LOCALAPPDATA") + L"\\Nextcloud";
    if (DirectoryExists(nextcloudConfig)) {
        DeleteDirectory(nextcloudConfig);
        std::wcout << L"Nextcloud configuration files deleted.\n";
    }
    else {
        std::wcout << L"No Nextcloud configuration files found.\n";
    }
    if (DirectoryExists(nextcloudCache)) {
        DeleteDirectory(nextcloudCache);
        std::wcout << L"Nextcloud cache files deleted.\n";
    }
    else {
        std::wcout << L"No Nextcloud cache files found.\n";
    }
    std::wcout << L"\n";

    // === Steam ===
    std::wcout << L"Closing Steam client...\n";
    KillProcess(L"steam.exe");

    std::wcout << L"Deleting Steam login credentials...\n";
    std::wstring steamLocal = GetEnvVar(L"LOCALAPPDATA") + L"\\Steam";
    std::wstring steamUserdata = GetEnvVar(L"PROGRAMFILES(X86)") + L"\\Steam\\userdata";
    if (DirectoryExists(steamLocal)) {
        DeleteDirectory(steamLocal);
        std::wcout << L"Steam local app data deleted.\n";
    }
    else {
        std::wcout << L"No Steam local app data found.\n";
    }
    if (DirectoryExists(steamUserdata)) {
        DeleteDirectory(steamUserdata);
        std::wcout << L"Steam user data deleted.\n";
    }
    else {
        std::wcout << L"No Steam user data found.\n";
    }
    std::wcout << L"\n";

    // === Telegram ===
    std::wcout << L"Closing Telegram client...\n";
    KillProcess(L"Telegram.exe");

    std::wcout << L"Deleting Telegram login credentials...\n";
    std::wstring telegramData = GetEnvVar(L"USERPROFILE") + L"\\AppData\\Roaming\\Telegram Desktop\\tdata";
    if (DirectoryExists(telegramData)) {
        DeleteDirectory(telegramData);
        std::wcout << L"Telegram login credentials deleted.\n";
    }
    else {
        std::wcout << L"No Telegram login credentials found.\n";
    }
    std::wcout << L"\n";

    // === Discord ===
    std::wcout << L"Closing Discord client...\n";
    KillProcess(L"Discord.exe");

    std::wcout << L"Deleting Discord login credentials...\n";
    std::wstring discordAppData = GetEnvVar(L"APPDATA") + L"\\discord";
    std::wstring discordLocal = GetEnvVar(L"LOCALAPPDATA") + L"\\discord";
    if (DirectoryExists(discordAppData)) {
        DeleteDirectory(discordAppData);
        std::wcout << L"Discord configuration files deleted.\n";
    }
    else {
        std::wcout << L"No Discord configuration files found.\n";
    }
    if (DirectoryExists(discordLocal)) {
        DeleteDirectory(discordLocal);
        std::wcout << L"Discord local app data deleted.\n";
    }
    else {
        std::wcout << L"No Discord local app data found.\n";
    }
    std::wcout << L"\n";

    // Delete a specific folder before empty recycle bin
    std::wstring folder = L"D:\\FOLDERNAME";
    if (DeleteDirectoryRecursively(folder)) {
        std::wcout << L"Deleted successfully.\n";
    }
    else {
        std::wcout << L"Failed to delete.\n";
    }

    // === Clear Recycle Bin ===
    std::wcout << L"Clearing Recycle Bin...\n";
    ClearRecycleBin();
    std::wcout << L"All Recycle Bins cleared.\n\n";

	// Print a Message
    std::wcout << L"All specified applications closed, related login information deleted, and Recycle Bin cleared.\n";
    
    // -- Restart the Computer
    system("C:\\Windows\\System32\\shutdown /r /t 0");
    system("pause");
    return 0;
}
