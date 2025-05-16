using System;
using System.IO;
using System.Security.Cryptography;

public static class SecureDelete
{
    // Overwrites a file with random data and then deletes it
    public static void SecureDeleteFile(string filePath, int passes = 1)
    {
        if (!File.Exists(filePath))
            return;

        FileInfo fileInfo = new FileInfo(filePath);
        long length = fileInfo.Length;

        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
        {
            byte[] buffer = new byte[4096];
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int pass = 0; pass < passes; pass++)
                {
                    fs.Position = 0;
                    long remaining = length;
                    while (remaining > 0)
                    {
                        int toWrite = (int)Math.Min(buffer.Length, remaining);
                        rng.GetBytes(buffer, 0, toWrite);
                        fs.Write(buffer, 0, toWrite);
                        remaining -= toWrite;
                    }
                    fs.Flush(true);
                }
            }
        }

        // Optionally, reset file times and rename before deletion for extra privacy
        File.SetLastWriteTimeUtc(filePath, DateTime.UnixEpoch);
        File.SetCreationTimeUtc(filePath, DateTime.UnixEpoch);
        File.SetLastAccessTimeUtc(filePath, DateTime.UnixEpoch);

        File.Delete(filePath);
    }

    // Recursively secure delete a directory and its contents
    public static void SecureDeleteDirectory(string dirPath, int passes = 3)
    {
        if (!Directory.Exists(dirPath))
            return;

        foreach (string file in Directory.GetFiles(dirPath))
        {
            SecureDeleteFile(file, passes);
        }

        foreach (string subdir in Directory.GetDirectories(dirPath))
        {
            SecureDeleteDirectory(subdir, passes);
        }
        File.SetAttributes(dirPath, FileAttributes.Normal);
        Directory.Delete(dirPath, false); // Remove the (now empty) directory
    }
}
